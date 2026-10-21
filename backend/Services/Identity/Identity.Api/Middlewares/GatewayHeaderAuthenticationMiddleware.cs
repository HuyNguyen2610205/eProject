using System.Security.Claims;

namespace Identity.Api.Middlewares;

/// <summary>
/// Middleware hỗ trợ mô hình phòng thủ theo chiều sâu (Defense-in-depth):
/// Nếu request đi qua ApiGateway và có gắn các Header nội bộ X-User-*,
/// middleware sẽ tự động tạo/bổ sung ClaimsPrincipal vào HttpContext.User
/// để các tầng Authorization, Controller và Service hạ nguồn hoạt động trong suốt.
/// </summary>
public class GatewayHeaderAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayHeaderAuthenticationMiddleware> _logger;

    public GatewayHeaderAuthenticationMiddleware(RequestDelegate next, ILogger<GatewayHeaderAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Kiểm tra sự hiện diện của Header X-User-Id từ Gateway
        if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) &&
            !string.IsNullOrWhiteSpace(userIdHeader))
        {
            var userId = userIdHeader.ToString().Trim();

            // Nếu người dùng chưa được authenticate bởi JwtBearer middleware (hoặc chạy trong mạng nội bộ)
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userId),
                    new("sub", userId)
                };

                // Đọc Email từ Header
                if (context.Request.Headers.TryGetValue("X-User-Email", out var emailHeader) &&
                    !string.IsNullOrWhiteSpace(emailHeader))
                {
                    var email = emailHeader.ToString().Trim();
                    claims.Add(new Claim(ClaimTypes.Email, email));
                    claims.Add(new Claim("email", email));
                }

                // Đọc Roles từ Header (danh sách phân tách bằng dấu phẩy)
                if (context.Request.Headers.TryGetValue("X-User-Roles", out var rolesHeader) &&
                    !string.IsNullOrWhiteSpace(rolesHeader))
                {
                    var roles = rolesHeader.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Đọc Permissions từ Header (danh sách phân tách bằng dấu phẩy)
                if (context.Request.Headers.TryGetValue("X-User-Permissions", out var permissionsHeader) &&
                    !string.IsNullOrWhiteSpace(permissionsHeader))
                {
                    var permissions = permissionsHeader.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var permission in permissions)
                    {
                        claims.Add(new Claim("permission", permission));
                    }
                }

                var identity = new ClaimsIdentity(claims, "GatewayHeader");
                context.User = new ClaimsPrincipal(identity);

                _logger.LogDebug("Đã thiết lập ClaimsPrincipal cho UserId {UserId} từ Header của ApiGateway.", userId);
            }
        }

        await _next(context);
    }
}
