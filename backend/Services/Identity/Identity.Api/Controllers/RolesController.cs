using Identity.Api.Authorization;
using Identity.Api.Configurations;
using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;
using Identity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Lấy danh sách tất cả vai trò trong hệ thống (kèm số lượng users và permissions)
    /// </summary>
    [HttpGet]
    [HasPermission(AppPermissions.Roles.View)]
    [ProducesResponseType(typeof(ApiResponse<List<RoleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _roleService.GetAllRolesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết vai trò theo Id
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission(AppPermissions.Roles.View)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoleById([FromRoute] Guid id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Tạo vai trò mới và tùy chọn gán các permissions ban đầu
    /// </summary>
    [HttpPost]
    [HasPermission(AppPermissions.Roles.Create)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var result = await _roleService.CreateRoleAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật thông tin vai trò và danh sách permissions
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission(AppPermissions.Roles.Edit)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRole([FromRoute] Guid id, [FromBody] UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateRoleAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Xóa vai trò (chặn xóa vai trò mặc định hoặc đang có tài khoản sử dụng)
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission(AppPermissions.Roles.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Lấy ma trận cây danh mục toàn bộ permissions hệ thống để render UI checkbox
    /// </summary>
    [HttpGet("permissions-matrix")]
    [HasPermission(AppPermissions.Roles.View)]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionGroupResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissionsMatrix()
    {
        var result = await _roleService.GetPermissionsMatrixAsync();
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật ma trận permissions cho một vai trò
    /// </summary>
    [HttpPut("{id}/permissions")]
    [HasPermission(AppPermissions.Roles.Edit)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRolePermissions([FromRoute] Guid id, [FromBody] UpdateRolePermissionsRequest request)
    {
        var result = await _roleService.UpdateRolePermissionsAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Gán vai trò cho một tài khoản người dùng
    /// </summary>
    [HttpPost("assign-to-user/{userId}")]
    [HasPermission(AppPermissions.Users.AssignRole)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignUserRoles([FromRoute] Guid userId, [FromBody] AssignUserRolesRequest request)
    {
        var result = await _roleService.AssignUserRolesAsync(userId, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
