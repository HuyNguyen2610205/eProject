using Content.Api.Data;
using Content.Api.Data.Entities;
using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Services.Implementations;

public class NgoService : INgoService
{
    private readonly ContentDbContext _dbContext;
    private readonly ILogger<NgoService> _logger;

    public NgoService(ContentDbContext dbContext, ILogger<NgoService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<List<NgoResponse>>> GetAllNgosAsync(int? causeId = null, bool includeInactive = false)
    {
        var query = _dbContext.Ngos
            .AsNoTracking()
            .Include(n => n.NgoCauses)
                .ThenInclude(nc => nc.Cause)
            .Include(n => n.Programmes)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(n => n.IsActive);
        }

        if (causeId.HasValue && causeId.Value > 0)
        {
            query = query.Where(n => n.NgoCauses.Any(nc => nc.CauseId == causeId.Value));
        }

        var ngos = await query
            .OrderBy(n => n.DisplayOrder)
            .ThenBy(n => n.Name)
            .Select(n => new NgoResponse
            {
                Id = n.Id,
                Name = n.Name,
                Description = n.Description,
                LogoUrl = n.LogoUrl,
                Website = n.Website,
                ContactEmail = n.ContactEmail,
                Phone = n.Phone,
                Address = n.Address,
                IsActive = n.IsActive,
                DisplayOrder = n.DisplayOrder,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                Causes = n.NgoCauses
                    .Where(nc => nc.Cause != null)
                    .Select(nc => new NgoCauseItemResponse
                    {
                        CauseId = nc.CauseId,
                        Name = nc.Cause!.Name,
                        Slug = nc.Cause.Slug
                    }).ToList(),
                TotalProgrammesCount = n.Programmes.Count
            })
            .ToListAsync();

        return ApiResponse<List<NgoResponse>>.Ok(ngos);
    }

    public async Task<ApiResponse<NgoResponse>> GetNgoByIdAsync(int id)
    {
        var ngo = await _dbContext.Ngos
            .AsNoTracking()
            .Include(n => n.NgoCauses)
                .ThenInclude(nc => nc.Cause)
            .Include(n => n.Programmes)
            .Where(n => n.Id == id)
            .Select(n => new NgoResponse
            {
                Id = n.Id,
                Name = n.Name,
                Description = n.Description,
                LogoUrl = n.LogoUrl,
                Website = n.Website,
                ContactEmail = n.ContactEmail,
                Phone = n.Phone,
                Address = n.Address,
                IsActive = n.IsActive,
                DisplayOrder = n.DisplayOrder,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                Causes = n.NgoCauses
                    .Where(nc => nc.Cause != null)
                    .Select(nc => new NgoCauseItemResponse
                    {
                        CauseId = nc.CauseId,
                        Name = nc.Cause!.Name,
                        Slug = nc.Cause.Slug
                    }).ToList(),
                TotalProgrammesCount = n.Programmes.Count
            })
            .FirstOrDefaultAsync();

        if (ngo == null)
        {
            return ApiResponse<NgoResponse>.Fail("Không tìm thấy tổ chức NGO.");
        }

        return ApiResponse<NgoResponse>.Ok(ngo);
    }

    public async Task<ApiResponse<NgoResponse>> CreateNgoAsync(CreateNgoRequest request)
    {
        var ngo = new Ngo
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(),
            Website = request.Website?.Trim(),
            ContactEmail = request.ContactEmail?.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            Address = request.Address?.Trim(),
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        if (request.CauseIds != null && request.CauseIds.Any())
        {
            var validCauseIds = await _dbContext.Causes
                .Where(c => request.CauseIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var causeId in validCauseIds)
            {
                ngo.NgoCauses.Add(new NgoCause { CauseId = causeId, CreatedAt = DateTime.UtcNow });
            }
        }

        _dbContext.Ngos.Add(ngo);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã thêm mới tổ chức NGO: {Name} (ID: {Id})", ngo.Name, ngo.Id);
        return await GetNgoByIdAsync(ngo.Id);
    }

    public async Task<ApiResponse<NgoResponse>> UpdateNgoAsync(int id, UpdateNgoRequest request)
    {
        var ngo = await _dbContext.Ngos
            .Include(n => n.NgoCauses)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (ngo == null)
        {
            return ApiResponse<NgoResponse>.Fail("Không tìm thấy tổ chức NGO.");
        }

        ngo.Name = request.Name.Trim();
        ngo.Description = request.Description?.Trim();
        ngo.LogoUrl = request.LogoUrl?.Trim();
        ngo.Website = request.Website?.Trim();
        ngo.ContactEmail = request.ContactEmail?.Trim().ToLowerInvariant();
        ngo.Phone = request.Phone?.Trim();
        ngo.Address = request.Address?.Trim();
        ngo.IsActive = request.IsActive;
        ngo.DisplayOrder = request.DisplayOrder;
        ngo.UpdatedAt = DateTime.UtcNow;

        if (request.CauseIds != null)
        {
            _dbContext.NgoCauses.RemoveRange(ngo.NgoCauses);
            var validCauseIds = await _dbContext.Causes
                .Where(c => request.CauseIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var causeId in validCauseIds)
            {
                ngo.NgoCauses.Add(new NgoCause { CauseId = causeId, CreatedAt = DateTime.UtcNow });
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã cập nhật tổ chức NGO: {Name} (ID: {Id})", ngo.Name, ngo.Id);
        return await GetNgoByIdAsync(ngo.Id);
    }

    public async Task<ApiResponse<bool>> ToggleNgoStatusAsync(int id)
    {
        var ngo = await _dbContext.Ngos.FindAsync(id);
        if (ngo == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy tổ chức NGO.");
        }

        ngo.IsActive = !ngo.IsActive;
        ngo.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<bool>.Ok(ngo.IsActive, $"Đã {(ngo.IsActive ? "kích hoạt" : "ẩn")} tổ chức NGO.");
    }
}
