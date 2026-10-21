using Identity.Api.Data;
using Identity.Api.Data.Entities;
using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;
using Identity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppIdentityDbContext _dbContext;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        AppIdentityDbContext dbContext,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<List<RoleResponse>>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();

        var userRoleCounts = await _dbContext.UserRoles
            .GroupBy(ur => ur.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count);

        var result = roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name ?? string.Empty,
            Description = r.Description,
            UserCount = userRoleCounts.TryGetValue(r.Id, out var count) ? count : 0,
            Permissions = r.RolePermissions.Select(rp => rp.Permission.Code).ToList()
        }).ToList();

        return ApiResponse<List<RoleResponse>>.Ok(result, "Lấy danh sách vai trò thành công.");
    }

    public async Task<ApiResponse<RoleResponse>> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleManager.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
        {
            return ApiResponse<RoleResponse>.Fail("Không tìm thấy vai trò.");
        }

        var userCount = await _dbContext.UserRoles.CountAsync(ur => ur.RoleId == id);

        var response = new RoleResponse
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            UserCount = userCount,
            Permissions = role.RolePermissions.Select(rp => rp.Permission.Code).ToList()
        };

        return ApiResponse<RoleResponse>.Ok(response, "Lấy chi tiết vai trò thành công.");
    }

    public async Task<ApiResponse<RoleResponse>> CreateRoleAsync(CreateRoleRequest request)
    {
        var roleName = request.Name.Trim();
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return ApiResponse<RoleResponse>.Fail($"Vai trò '{roleName}' đã tồn tại.");
        }

        var newRole = new ApplicationRole(roleName, request.Description);
        var result = await _roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Tạo vai trò '{RoleName}' thất bại: {Errors}", roleName, string.Join("; ", errors));
            return ApiResponse<RoleResponse>.Fail("Tạo vai trò thất bại.", errors);
        }

        // Gán permissions nếu có
        if (request.Permissions.Any())
        {
            var validPermissions = await _dbContext.Permissions
                .Where(p => request.Permissions.Contains(p.Code))
                .ToListAsync();

            var rolePermissions = validPermissions.Select(p => new RolePermission
            {
                RoleId = newRole.Id,
                PermissionId = p.Id
            }).ToList();

            _dbContext.RolePermissions.AddRange(rolePermissions);
            await _dbContext.SaveChangesAsync();
        }

        _logger.LogInformation("Đã tạo vai trò mới thành công: '{RoleName}' (RoleId: {RoleId}) với {PermCount} quyền.", newRole.Name, newRole.Id, request.Permissions.Count);

        return await GetRoleByIdAsync(newRole.Id);
    }

    public async Task<ApiResponse<RoleResponse>> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return ApiResponse<RoleResponse>.Fail("Không tìm thấy vai trò.");
        }

        var newName = request.Name.Trim();
        var normalizedSystemRoles = new[] { "ADMIN", "USER" };

        // Không cho phép đổi tên vai trò hệ thống cốt lõi
        if (normalizedSystemRoles.Contains(role.NormalizedName) && !string.Equals(role.Name, newName, StringComparison.OrdinalIgnoreCase))
        {
            return ApiResponse<RoleResponse>.Fail($"Không thể đổi tên vai trò hệ thống '{role.Name}'.");
        }

        if (!string.Equals(role.Name, newName, StringComparison.OrdinalIgnoreCase))
        {
            if (await _roleManager.RoleExistsAsync(newName))
            {
                return ApiResponse<RoleResponse>.Fail($"Tên vai trò '{newName}' đã được sử dụng.");
            }
            role.Name = newName;
        }

        role.Description = request.Description;
        var updateResult = await _roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Cập nhật vai trò '{RoleId}' thất bại: {Errors}", id, string.Join("; ", errors));
            return ApiResponse<RoleResponse>.Fail("Cập nhật vai trò thất bại.", errors);
        }

        // Cập nhật lại danh sách permissions
        var existingRolePerms = await _dbContext.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
        _dbContext.RolePermissions.RemoveRange(existingRolePerms);

        if (request.Permissions.Any())
        {
            var validPermissions = await _dbContext.Permissions
                .Where(p => request.Permissions.Contains(p.Code))
                .ToListAsync();

            var newRolePerms = validPermissions.Select(p => new RolePermission
            {
                RoleId = id,
                PermissionId = p.Id
            }).ToList();

            _dbContext.RolePermissions.AddRange(newRolePerms);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Đã cập nhật thông tin vai trò '{RoleName}' (RoleId: {RoleId}).", role.Name, role.Id);
        return await GetRoleByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteRoleAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy vai trò.");
        }

        var normalizedSystemRoles = new[] { "ADMIN", "USER" };
        if (normalizedSystemRoles.Contains(role.NormalizedName))
        {
            return ApiResponse<bool>.Fail($"Không được phép xóa vai trò mặc định '{role.Name}'.");
        }

        var hasUsers = await _dbContext.UserRoles.AnyAsync(ur => ur.RoleId == id);
        if (hasUsers)
        {
            return ApiResponse<bool>.Fail("Không thể xóa vai trò đang có người dùng được gán.");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Xóa vai trò '{RoleName}' thất bại: {Errors}", role.Name, string.Join("; ", errors));
            return ApiResponse<bool>.Fail("Xóa vai trò thất bại.", errors);
        }

        _logger.LogInformation("Đã xóa vai trò '{RoleName}' (RoleId: {RoleId}) thành công.", role.Name, role.Id);
        return ApiResponse<bool>.Ok(true, $"Xóa vai trò '{role.Name}' thành công.");
    }

    public async Task<ApiResponse<List<PermissionGroupResponse>>> GetPermissionsMatrixAsync()
    {
        var allPermissions = await _dbContext.Permissions
            .OrderBy(p => p.GroupName)
            .ThenBy(p => p.Code)
            .ToListAsync();

        var grouped = allPermissions
            .GroupBy(p => p.GroupName)
            .Select(g => new PermissionGroupResponse
            {
                GroupName = g.Key,
                Permissions = g.Select(p => new PermissionResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    GroupName = p.GroupName,
                    Description = p.Description
                }).ToList()
            })
            .ToList();

        return ApiResponse<List<PermissionGroupResponse>>.Ok(grouped, "Lấy ma trận danh mục quyền thành công.");
    }

    public async Task<ApiResponse<bool>> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy vai trò.");
        }

        var existing = await _dbContext.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _dbContext.RolePermissions.RemoveRange(existing);

        if (request.Permissions.Any())
        {
            var validPermissions = await _dbContext.Permissions
                .Where(p => request.Permissions.Contains(p.Code))
                .ToListAsync();

            var newRolePerms = validPermissions.Select(p => new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id
            }).ToList();

            _dbContext.RolePermissions.AddRange(newRolePerms);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Đã cập nhật danh sách quyền cho vai trò '{RoleName}' (RoleId: {RoleId}): {Count} quyền.", role.Name, role.Id, request.Permissions.Count);
        return ApiResponse<bool>.Ok(true, $"Cập nhật phân quyền cho vai trò '{role.Name}' thành công.");
    }

    public async Task<ApiResponse<bool>> AssignUserRolesAsync(Guid userId, AssignUserRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy người dùng.");
        }

        // Kiểm tra danh sách role hợp lệ
        foreach (var roleName in request.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return ApiResponse<bool>.Fail($"Vai trò '{roleName}' không tồn tại trong hệ thống.");
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return ApiResponse<bool>.Fail("Gỡ bỏ vai trò cũ thất bại.");
        }

        var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Gán vai trò mới cho người dùng '{Email}' thất bại: {Errors}", user.Email, string.Join("; ", errors));
            return ApiResponse<bool>.Fail("Gán vai trò mới thất bại.", errors);
        }

        _logger.LogInformation("Đã cập nhật vai trò cho người dùng '{Email}' (UserId: {UserId}). Vai trò mới: [{Roles}].", user.Email, user.Id, string.Join(", ", request.Roles));
        return ApiResponse<bool>.Ok(true, $"Cập nhật vai trò cho người dùng '{user.Email}' thành công.");
    }
}
