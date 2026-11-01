using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1. Kiểm tra người dùng đã xác thực (Authenticated) hay chưa
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        // 2. Quyền SuperAdmin: Người dùng có vai trò Admin mặc định được toàn quyền
        if (context.User.IsInRole("Admin") || context.User.IsInRole("ADMIN"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 3. Kiểm tra User Claims có chứa claim "permission" trùng khớp requirement không
        var hasPermission = context.User.Claims.Any(c =>
            c.Type == "permission" &&
            string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
