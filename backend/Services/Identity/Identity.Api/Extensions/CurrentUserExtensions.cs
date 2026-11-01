using System.Security.Claims;

namespace Identity.Api.Extensions;

/// <summary>
/// Các phương thức mở rộng hỗ trợ đọc nhanh thông tin người dùng từ ClaimsPrincipal hoặc Header nội bộ
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Lấy ID của người dùng hiện tại (Guid)
    /// </summary>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var idStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? principal.FindFirst("sub")?.Value;

        return Guid.TryParse(idStr, out var id) ? id : null;
    }

    /// <summary>
    /// Lấy Email của người dùng hiện tại
    /// </summary>
    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value
            ?? principal.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Lấy danh sách các vai trò (Roles) của người dùng hiện tại
    /// </summary>
    public static List<string> GetUserRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Lấy danh sách các quyền hạn (Permissions) của người dùng hiện tại
    /// </summary>
    public static List<string> GetUserPermissions(this ClaimsPrincipal principal)
    {
        return principal.FindAll("permission")
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Kiểm tra người dùng hiện tại có sở hữu quyền cụ thể hay không
    /// </summary>
    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        return principal.FindAll("permission")
            .Any(c => string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
    }
}
