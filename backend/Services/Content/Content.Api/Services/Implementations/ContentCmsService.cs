using Content.Api.Data;
using Content.Api.Data.Entities;
using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Services.Implementations;

public class ContentCmsService : IContentCmsService
{
    private readonly ContentDbContext _dbContext;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<ContentCmsService> _logger;

    public ContentCmsService(
        ContentDbContext dbContext,
        INotificationClient notificationClient,
        ILogger<ContentCmsService> logger)
    {
        _dbContext = dbContext;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    #region About Pages
    public async Task<ApiResponse<List<AboutPageResponse>>> GetAllAboutPagesAsync()
    {
        var pages = await _dbContext.AboutPages
            .AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderBy(a => a.Id)
            .Select(a => new AboutPageResponse
            {
                Id = a.Id,
                Slug = a.Slug,
                Title = a.Title,
                Content = a.Content,
                BannerUrl = a.BannerUrl,
                IsPublished = a.IsPublished,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return ApiResponse<List<AboutPageResponse>>.Ok(pages);
    }

    public async Task<ApiResponse<AboutPageResponse>> GetAboutPageBySlugAsync(string slug)
    {
        var page = await _dbContext.AboutPages
            .AsNoTracking()
            .Where(a => a.Slug == slug.ToLower().Trim() && a.IsPublished)
            .Select(a => new AboutPageResponse
            {
                Id = a.Id,
                Slug = a.Slug,
                Title = a.Title,
                Content = a.Content,
                BannerUrl = a.BannerUrl,
                IsPublished = a.IsPublished,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (page == null)
        {
            return ApiResponse<AboutPageResponse>.Fail("Không tìm thấy nội dung trang giới thiệu.");
        }

        return ApiResponse<AboutPageResponse>.Ok(page);
    }

    public async Task<ApiResponse<AboutPageResponse>> UpdateAboutPageAsync(string slug, UpdateAboutPageRequest request, Guid? updatedBy = null)
    {
        var page = await _dbContext.AboutPages
            .FirstOrDefaultAsync(a => a.Slug == slug.ToLower().Trim());

        if (page == null)
        {
            return ApiResponse<AboutPageResponse>.Fail("Không tìm thấy trang cần cập nhật.");
        }

        page.Title = request.Title.Trim();
        page.Content = request.Content;
        page.BannerUrl = request.BannerUrl?.Trim();
        page.IsPublished = request.IsPublished;
        page.UpdatedAt = DateTime.UtcNow;
        page.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã cập nhật nội dung trang About Us: {Slug}", page.Slug);

        var response = new AboutPageResponse
        {
            Id = page.Id,
            Slug = page.Slug,
            Title = page.Title,
            Content = page.Content,
            BannerUrl = page.BannerUrl,
            IsPublished = page.IsPublished,
            UpdatedAt = page.UpdatedAt
        };

        return ApiResponse<AboutPageResponse>.Ok(response, "Cập nhật nội dung trang thành công.");
    }
    #endregion

    #region Partners
    public async Task<ApiResponse<List<PartnerResponse>>> GetAllPartnersAsync(bool includeInactive = false)
    {
        var query = _dbContext.Partners.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        var partners = await query
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .Select(p => new PartnerResponse
            {
                Id = p.Id,
                Name = p.Name,
                LogoUrl = p.LogoUrl,
                Website = p.Website,
                Description = p.Description,
                DisplayOrder = p.DisplayOrder,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<PartnerResponse>>.Ok(partners);
    }

    public async Task<ApiResponse<PartnerResponse>> CreatePartnerAsync(CreatePartnerRequest request)
    {
        var partner = new Partner
        {
            Name = request.Name.Trim(),
            LogoUrl = request.LogoUrl.Trim(),
            Website = request.Website?.Trim(),
            Description = request.Description?.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Partners.Add(partner);
        await _dbContext.SaveChangesAsync();

        var response = new PartnerResponse
        {
            Id = partner.Id,
            Name = partner.Name,
            LogoUrl = partner.LogoUrl,
            Website = partner.Website,
            Description = partner.Description,
            DisplayOrder = partner.DisplayOrder,
            IsActive = partner.IsActive,
            CreatedAt = partner.CreatedAt
        };

        return ApiResponse<PartnerResponse>.Ok(response, "Thêm đối tác thành công.");
    }

    public async Task<ApiResponse<PartnerResponse>> UpdatePartnerAsync(int id, UpdatePartnerRequest request)
    {
        var partner = await _dbContext.Partners.FindAsync(id);
        if (partner == null)
        {
            return ApiResponse<PartnerResponse>.Fail("Không tìm thấy đối tác.");
        }

        partner.Name = request.Name.Trim();
        partner.LogoUrl = request.LogoUrl.Trim();
        partner.Website = request.Website?.Trim();
        partner.Description = request.Description?.Trim();
        partner.DisplayOrder = request.DisplayOrder;
        partner.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        var response = new PartnerResponse
        {
            Id = partner.Id,
            Name = partner.Name,
            LogoUrl = partner.LogoUrl,
            Website = partner.Website,
            Description = partner.Description,
            DisplayOrder = partner.DisplayOrder,
            IsActive = partner.IsActive,
            CreatedAt = partner.CreatedAt
        };

        return ApiResponse<PartnerResponse>.Ok(response, "Cập nhật đối tác thành công.");
    }

    public async Task<ApiResponse<bool>> DeletePartnerAsync(int id)
    {
        var partner = await _dbContext.Partners.FindAsync(id);
        if (partner == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy đối tác.");
        }

        _dbContext.Partners.Remove(partner);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Xóa đối tác thành công.");
    }
    #endregion

    #region Gallery
    public async Task<ApiResponse<List<GalleryResponse>>> GetAllGalleriesAsync(int? programmeId = null)
    {
        var query = _dbContext.Galleries
            .AsNoTracking()
            .Include(g => g.Programme)
            .AsQueryable();

        if (programmeId.HasValue && programmeId.Value > 0)
        {
            query = query.Where(g => g.ProgrammeId == programmeId.Value);
        }

        var galleries = await query
            .OrderBy(g => g.DisplayOrder)
            .ThenByDescending(g => g.CreatedAt)
            .Select(g => new GalleryResponse
            {
                Id = g.Id,
                ProgrammeId = g.ProgrammeId,
                ProgrammeTitle = g.Programme != null ? g.Programme.Title : null,
                Title = g.Title,
                ImageUrl = g.ImageUrl,
                Caption = g.Caption,
                DisplayOrder = g.DisplayOrder,
                CreatedAt = g.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<GalleryResponse>>.Ok(galleries);
    }

    public async Task<ApiResponse<GalleryResponse>> CreateGalleryItemAsync(CreateGalleryRequest request)
    {
        var gallery = new Gallery
        {
            ProgrammeId = request.ProgrammeId,
            Title = request.Title.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            Caption = request.Caption?.Trim(),
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Galleries.Add(gallery);
        await _dbContext.SaveChangesAsync();

        string? programmeTitle = null;
        if (gallery.ProgrammeId.HasValue)
        {
            programmeTitle = await _dbContext.Programmes
                .Where(p => p.Id == gallery.ProgrammeId.Value)
                .Select(p => p.Title)
                .FirstOrDefaultAsync();
        }

        var response = new GalleryResponse
        {
            Id = gallery.Id,
            ProgrammeId = gallery.ProgrammeId,
            ProgrammeTitle = programmeTitle,
            Title = gallery.Title,
            ImageUrl = gallery.ImageUrl,
            Caption = gallery.Caption,
            DisplayOrder = gallery.DisplayOrder,
            CreatedAt = gallery.CreatedAt
        };

        return ApiResponse<GalleryResponse>.Ok(response, "Thêm hình ảnh vào thư viện thành công.");
    }

    public async Task<ApiResponse<bool>> DeleteGalleryItemAsync(int id)
    {
        var gallery = await _dbContext.Galleries.FindAsync(id);
        if (gallery == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy hình ảnh.");
        }

        _dbContext.Galleries.Remove(gallery);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Xóa hình ảnh thành công.");
    }
    #endregion

    #region Invitations
    public async Task<ApiResponse<InvitationResponse>> SendFriendInvitationAsync(SendInvitationRequest request, Guid? senderUserId = null)
    {
        var invitation = new EmailInvitation
        {
            SenderUserId = senderUserId,
            SenderName = request.SenderName.Trim(),
            SenderEmail = request.SenderEmail.Trim().ToLowerInvariant(),
            RecipientEmail = request.RecipientEmail.Trim().ToLowerInvariant(),
            Message = request.Message?.Trim(),
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        _dbContext.EmailInvitations.Add(invitation);
        await _dbContext.SaveChangesAsync();

        // Gửi email thực sự qua NotificationService
        _ = Task.Run(async () =>
        {
            try
            {
                await _notificationClient.SendFriendInvitationEmailAsync(
                    invitation.RecipientEmail,
                    invitation.SenderName,
                    invitation.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ngầm khi gửi email mời bạn bè tới {Email}", invitation.RecipientEmail);
            }
        });

        var response = new InvitationResponse
        {
            Id = invitation.Id,
            SenderUserId = invitation.SenderUserId,
            SenderName = invitation.SenderName,
            SenderEmail = invitation.SenderEmail,
            RecipientEmail = invitation.RecipientEmail,
            Message = invitation.Message,
            Status = invitation.Status,
            SentAt = invitation.SentAt
        };

        return ApiResponse<InvitationResponse>.Ok(response, "Lời mời tham gia hoạt động từ thiện đã được gửi thành công!");
    }
    #endregion
}
