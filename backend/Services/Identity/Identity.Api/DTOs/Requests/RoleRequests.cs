using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Tên vai trò không được để trống.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên vai trò từ 2 đến 50 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleRequest
{
    [Required(ErrorMessage = "Tên vai trò không được để trống.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên vai trò từ 2 đến 50 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    public List<string> Permissions { get; set; } = new();
}

public class UpdateRolePermissionsRequest
{
    public List<string> Permissions { get; set; } = new();
}

public class AssignUserRolesRequest
{
    [Required(ErrorMessage = "Danh sách vai trò không được để trống.")]
    public List<string> Roles { get; set; } = new();
}
