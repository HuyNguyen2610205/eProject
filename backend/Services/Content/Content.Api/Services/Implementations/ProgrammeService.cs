using Content.Api.Data;
using Content.Api.Data.Entities;
using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Services.Implementations;

public class ProgrammeService : IProgrammeService
{
    private readonly ContentDbContext _dbContext;
    private readonly ILogger<ProgrammeService> _logger;

    public ProgrammeService(ContentDbContext dbContext, ILogger<ProgrammeService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<List<ProgrammeResponse>>> GetAllProgrammesAsync(
        string? status = null,
        string? category = null,
        int? ngoId = null,
        bool? isFeatured = null)
    {
        var query = _dbContext.Programmes
            .AsNoTracking()
            .Include(p => p.Ngo)
            .Include(p => p.Registrations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(p => p.Status == status.Trim());
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category.Trim());
        }

        if (ngoId.HasValue && ngoId.Value > 0)
        {
            query = query.Where(p => p.NgoId == ngoId.Value);
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == isFeatured.Value);
        }

        var programmes = await query
            .OrderByDescending(p => p.StartDate)
            .Select(p => new ProgrammeResponse
            {
                Id = p.Id,
                NgoId = p.NgoId,
                NgoName = p.Ngo != null ? p.Ngo.Name : string.Empty,
                NgoLogoUrl = p.Ngo != null ? p.Ngo.LogoUrl : null,
                Title = p.Title,
                Description = p.Description,
                Category = p.Category,
                Location = p.Location,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                IsFeatured = p.IsFeatured,
                RegistrationsCount = p.Registrations.Count,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return ApiResponse<List<ProgrammeResponse>>.Ok(programmes);
    }

    public async Task<ApiResponse<ProgrammeResponse>> GetProgrammeByIdAsync(int id)
    {
        var programme = await _dbContext.Programmes
            .AsNoTracking()
            .Include(p => p.Ngo)
            .Include(p => p.Registrations)
            .Where(p => p.Id == id)
            .Select(p => new ProgrammeResponse
            {
                Id = p.Id,
                NgoId = p.NgoId,
                NgoName = p.Ngo != null ? p.Ngo.Name : string.Empty,
                NgoLogoUrl = p.Ngo != null ? p.Ngo.LogoUrl : null,
                Title = p.Title,
                Description = p.Description,
                Category = p.Category,
                Location = p.Location,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                IsFeatured = p.IsFeatured,
                RegistrationsCount = p.Registrations.Count,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (programme == null)
        {
            return ApiResponse<ProgrammeResponse>.Fail("Không tìm thấy chương trình.");
        }

        return ApiResponse<ProgrammeResponse>.Ok(programme);
    }

    public async Task<ApiResponse<ProgrammeResponse>> CreateProgrammeAsync(CreateProgrammeRequest request)
    {
        var ngoExists = await _dbContext.Ngos.AnyAsync(n => n.Id == request.NgoId && n.IsActive);
        if (!ngoExists)
        {
            return ApiResponse<ProgrammeResponse>.Fail("Tổ chức NGO chủ trì không tồn tại hoặc đã bị ẩn.");
        }

        var programme = new Programme
        {
            NgoId = request.NgoId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Category = request.Category?.Trim(),
            Location = request.Location?.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ImageUrl = request.ImageUrl?.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Upcoming" : request.Status.Trim(),
            IsFeatured = request.IsFeatured,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Programmes.Add(programme);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã tạo chương trình mới: {Title} (ID: {Id})", programme.Title, programme.Id);
        return await GetProgrammeByIdAsync(programme.Id);
    }

    public async Task<ApiResponse<ProgrammeResponse>> UpdateProgrammeAsync(int id, UpdateProgrammeRequest request)
    {
        var programme = await _dbContext.Programmes.FindAsync(id);
        if (programme == null)
        {
            return ApiResponse<ProgrammeResponse>.Fail("Không tìm thấy chương trình.");
        }

        var ngoExists = await _dbContext.Ngos.AnyAsync(n => n.Id == request.NgoId && n.IsActive);
        if (!ngoExists)
        {
            return ApiResponse<ProgrammeResponse>.Fail("Tổ chức NGO chủ trì không tồn tại hoặc đã bị ẩn.");
        }

        programme.NgoId = request.NgoId;
        programme.Title = request.Title.Trim();
        programme.Description = request.Description?.Trim();
        programme.Category = request.Category?.Trim();
        programme.Location = request.Location?.Trim();
        programme.StartDate = request.StartDate;
        programme.EndDate = request.EndDate;
        programme.ImageUrl = request.ImageUrl?.Trim();
        programme.Status = request.Status.Trim();
        programme.IsFeatured = request.IsFeatured;
        programme.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã cập nhật chương trình: {Title} (ID: {Id})", programme.Title, programme.Id);
        return await GetProgrammeByIdAsync(programme.Id);
    }

    public async Task<ApiResponse<bool>> DeleteProgrammeAsync(int id)
    {
        var programme = await _dbContext.Programmes.FindAsync(id);
        if (programme == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy chương trình.");
        }

        _dbContext.Programmes.Remove(programme);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã xóa chương trình: {Title} (ID: {Id})", programme.Title, programme.Id);
        return ApiResponse<bool>.Ok(true, "Xóa chương trình thành công.");
    }

    public async Task<ApiResponse<ProgrammeRegistrationResponse>> RegisterProgrammeAsync(
        int programmeId,
        RegisterProgrammeRequest request,
        Guid? userId = null)
    {
        var programme = await _dbContext.Programmes.FindAsync(programmeId);
        if (programme == null)
        {
            return ApiResponse<ProgrammeRegistrationResponse>.Fail("Chương trình không tồn tại.");
        }

        if (programme.Status == "Completed" || programme.Status == "Cancelled")
        {
            return ApiResponse<ProgrammeRegistrationResponse>.Fail("Chương trình này đã kết thúc hoặc bị hủy, không còn nhận đăng ký.");
        }

        var cleanEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Chống đăng ký trùng lặp nếu User đã đăng nhập
        if (userId.HasValue)
        {
            var alreadyRegisteredUser = await _dbContext.ProgrammeRegistrations
                .AnyAsync(r => r.ProgrammeId == programmeId && r.UserId == userId.Value);

            if (alreadyRegisteredUser)
            {
                return ApiResponse<ProgrammeRegistrationResponse>.Fail("Bạn đã đăng ký tham gia chương trình này trước đó.");
            }
        }

        // 2. Chống đăng ký trùng lặp theo Email
        var alreadyRegisteredEmail = await _dbContext.ProgrammeRegistrations
            .AnyAsync(r => r.ProgrammeId == programmeId && r.Email == cleanEmail);

        if (alreadyRegisteredEmail)
        {
            return ApiResponse<ProgrammeRegistrationResponse>.Fail("Địa chỉ email này đã được sử dụng để đăng ký chương trình.");
        }

        var registration = new ProgrammeRegistration
        {
            ProgrammeId = programmeId,
            UserId = userId,
            FullName = request.FullName.Trim(),
            Email = cleanEmail,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Motivation = request.Motivation?.Trim(),
            Status = "Pending",
            RegisteredAt = DateTime.UtcNow
        };

        _dbContext.ProgrammeRegistrations.Add(registration);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đăng ký chương trình thành công: {Email} cho Programme ID: {ProgrammeId}", cleanEmail, programmeId);

        var response = new ProgrammeRegistrationResponse
        {
            Id = registration.Id,
            ProgrammeId = registration.ProgrammeId,
            ProgrammeTitle = programme.Title,
            UserId = registration.UserId,
            FullName = registration.FullName,
            Email = registration.Email,
            Phone = registration.Phone,
            Motivation = registration.Motivation,
            Status = registration.Status,
            RegisteredAt = registration.RegisteredAt
        };

        return ApiResponse<ProgrammeRegistrationResponse>.Ok(response, "Đăng ký tham gia chương trình thành công! Chúng tôi sẽ liên hệ lại với bạn sớm nhất.");
    }

    public async Task<ApiResponse<List<ProgrammeRegistrationResponse>>> GetProgrammeRegistrationsAsync(int programmeId)
    {
        var registrations = await _dbContext.ProgrammeRegistrations
            .AsNoTracking()
            .Include(r => r.Programme)
            .Where(r => r.ProgrammeId == programmeId)
            .OrderByDescending(r => r.RegisteredAt)
            .Select(r => new ProgrammeRegistrationResponse
            {
                Id = r.Id,
                ProgrammeId = r.ProgrammeId,
                ProgrammeTitle = r.Programme != null ? r.Programme.Title : string.Empty,
                UserId = r.UserId,
                FullName = r.FullName,
                Email = r.Email,
                Phone = r.Phone,
                Motivation = r.Motivation,
                Status = r.Status,
                RegisteredAt = r.RegisteredAt
            })
            .ToListAsync();

        return ApiResponse<List<ProgrammeRegistrationResponse>>.Ok(registrations);
    }

    public async Task<ApiResponse<List<ProgrammeRegistrationResponse>>> GetUserRegistrationsAsync(Guid userId)
    {
        var registrations = await _dbContext.ProgrammeRegistrations
            .AsNoTracking()
            .Include(r => r.Programme)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAt)
            .Select(r => new ProgrammeRegistrationResponse
            {
                Id = r.Id,
                ProgrammeId = r.ProgrammeId,
                ProgrammeTitle = r.Programme != null ? r.Programme.Title : string.Empty,
                UserId = r.UserId,
                FullName = r.FullName,
                Email = r.Email,
                Phone = r.Phone,
                Motivation = r.Motivation,
                Status = r.Status,
                RegisteredAt = r.RegisteredAt
            })
            .ToListAsync();

        return ApiResponse<List<ProgrammeRegistrationResponse>>.Ok(registrations);
    }
}
