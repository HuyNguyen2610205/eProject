using Donation.Api.Data;
using Donation.Api.Data.Entities;
using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;
using Donation.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Donation.Api.Services.Implementations;

public class CauseService : ICauseService
{
    private readonly DonationDbContext _dbContext;
    private readonly ILogger<CauseService> _logger;

    public CauseService(DonationDbContext dbContext, ILogger<CauseService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<List<CauseResponse>>> GetAllCausesAsync(bool includeInactive = false)
    {
        var query = _dbContext.Causes.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var causes = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CauseResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                IconUrl = c.IconUrl,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                DisplayOrder = c.DisplayOrder,
                TotalRaised = c.Donations.Where(d => d.Status == "Successful").Sum(d => d.Amount),
                TotalDonationsCount = c.Donations.Count(d => d.Status == "Successful"),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return ApiResponse<List<CauseResponse>>.Ok(causes);
    }

    public async Task<ApiResponse<CauseResponse>> GetCauseByIdOrSlugAsync(string idOrSlug)
    {
        var isId = int.TryParse(idOrSlug, out var id);

        var cause = await _dbContext.Causes
            .AsNoTracking()
            .Where(c => isId ? c.Id == id : c.Slug == idOrSlug.ToLower().Trim())
            .Select(c => new CauseResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                IconUrl = c.IconUrl,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                DisplayOrder = c.DisplayOrder,
                TotalRaised = c.Donations.Where(d => d.Status == "Successful").Sum(d => d.Amount),
                TotalDonationsCount = c.Donations.Count(d => d.Status == "Successful"),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (cause == null)
        {
            return ApiResponse<CauseResponse>.Fail("Không tìm thấy danh mục quyên góp.");
        }

        return ApiResponse<CauseResponse>.Ok(cause);
    }

    public async Task<ApiResponse<CauseResponse>> CreateCauseAsync(CreateCauseRequest request)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? GenerateSlug(request.Name)
            : request.Slug.ToLower().Trim();

        var exists = await _dbContext.Causes.AnyAsync(c => c.Slug == slug);
        if (exists)
        {
            return ApiResponse<CauseResponse>.Fail("Mã định danh (Slug) của danh mục đã tồn tại.");
        }

        var cause = new Cause
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description?.Trim(),
            IconUrl = request.IconUrl?.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Causes.Add(cause);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã tạo mới danh mục quyên góp: {Name} (ID: {Id})", cause.Name, cause.Id);

        var response = new CauseResponse
        {
            Id = cause.Id,
            Name = cause.Name,
            Slug = cause.Slug,
            Description = cause.Description,
            IconUrl = cause.IconUrl,
            ImageUrl = cause.ImageUrl,
            IsActive = cause.IsActive,
            DisplayOrder = cause.DisplayOrder,
            TotalRaised = 0,
            TotalDonationsCount = 0,
            CreatedAt = cause.CreatedAt
        };

        return ApiResponse<CauseResponse>.Ok(response, "Tạo danh mục quyên góp thành công.");
    }

    public async Task<ApiResponse<CauseResponse>> UpdateCauseAsync(int id, UpdateCauseRequest request)
    {
        var cause = await _dbContext.Causes.FindAsync(id);
        if (cause == null)
        {
            return ApiResponse<CauseResponse>.Fail("Không tìm thấy danh mục quyên góp.");
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? GenerateSlug(request.Name)
            : request.Slug.ToLower().Trim();

        var slugExists = await _dbContext.Causes.AnyAsync(c => c.Slug == slug && c.Id != id);
        if (slugExists)
        {
            return ApiResponse<CauseResponse>.Fail("Mã định danh (Slug) này đã được sử dụng bởi danh mục khác.");
        }

        cause.Name = request.Name.Trim();
        cause.Slug = slug;
        cause.Description = request.Description?.Trim();
        cause.IconUrl = request.IconUrl?.Trim();
        cause.ImageUrl = request.ImageUrl?.Trim();
        cause.IsActive = request.IsActive;
        cause.DisplayOrder = request.DisplayOrder;
        cause.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã cập nhật danh mục quyên góp: {Name} (ID: {Id})", cause.Name, cause.Id);

        var totalRaised = await _dbContext.Donations
            .Where(d => d.CauseId == id && d.Status == "Successful")
            .SumAsync(d => d.Amount);
        var donationCount = await _dbContext.Donations
            .Where(d => d.CauseId == id && d.Status == "Successful")
            .CountAsync();

        var response = new CauseResponse
        {
            Id = cause.Id,
            Name = cause.Name,
            Slug = cause.Slug,
            Description = cause.Description,
            IconUrl = cause.IconUrl,
            ImageUrl = cause.ImageUrl,
            IsActive = cause.IsActive,
            DisplayOrder = cause.DisplayOrder,
            TotalRaised = totalRaised,
            TotalDonationsCount = donationCount,
            CreatedAt = cause.CreatedAt,
            UpdatedAt = cause.UpdatedAt
        };

        return ApiResponse<CauseResponse>.Ok(response, "Cập nhật danh mục quyên góp thành công.");
    }

    public async Task<ApiResponse<bool>> ToggleCauseStatusAsync(int id)
    {
        var cause = await _dbContext.Causes.FindAsync(id);
        if (cause == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy danh mục quyên góp.");
        }

        cause.IsActive = !cause.IsActive;
        cause.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã đổi trạng thái danh mục {Id} sang {Status}", id, cause.IsActive ? "Hoạt động" : "Tạm ẩn");
        return ApiResponse<bool>.Ok(cause.IsActive, $"Đã {(cause.IsActive ? "kích hoạt" : "ẩn")} danh mục thành công.");
    }

    private static string GenerateSlug(string phrase)
    {
        var str = phrase.ToLower().Trim();
        str = Regex.Replace(str, @"[àáạảãâầấậẩẫăằắặẳẵ]", "a");
        str = Regex.Replace(str, @"[èéẹẻẽêềếệểễ]", "e");
        str = Regex.Replace(str, @"[ìíịỉĩ]", "i");
        str = Regex.Replace(str, @"[òóọỏõôồốộổỗơờớợởỡ]", "o");
        str = Regex.Replace(str, @"[ùúụủũưừứựửữ]", "u");
        str = Regex.Replace(str, @"[ỳýỵỷỹ]", "y");
        str = Regex.Replace(str, @"[đ]", "d");
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-");
        return str;
    }
}
