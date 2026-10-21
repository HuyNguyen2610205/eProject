using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;

namespace Identity.Api.Services.Interfaces;

public interface IRoleService
{
    Task<ApiResponse<List<RoleResponse>>> GetAllRolesAsync();
    Task<ApiResponse<RoleResponse>> GetRoleByIdAsync(Guid id);
    Task<ApiResponse<RoleResponse>> CreateRoleAsync(CreateRoleRequest request);
    Task<ApiResponse<RoleResponse>> UpdateRoleAsync(Guid id, UpdateRoleRequest request);
    Task<ApiResponse<bool>> DeleteRoleAsync(Guid id);
    Task<ApiResponse<List<PermissionGroupResponse>>> GetPermissionsMatrixAsync();
    Task<ApiResponse<bool>> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request);
    Task<ApiResponse<bool>> AssignUserRolesAsync(Guid userId, AssignUserRolesRequest request);
}
