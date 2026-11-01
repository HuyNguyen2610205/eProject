using System.Text.Json;

namespace Notification.Api.Middlewares;

public class InternalSecretMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expectedSecret;

    public InternalSecretMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _expectedSecret = configuration["InternalSecret"] ?? "eProject_Internal_Secret_Key_2026_Secure!";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        // Chỉ áp dụng bảo vệ cho các API gửi email
        if (path.StartsWith("/api/notifications/emails"))
        {
            if (!context.Request.Headers.TryGetValue("X-Internal-Secret", out var providedSecret) 
                || !string.Equals(providedSecret, _expectedSecret, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    success = false,
                    message = "Truy cập bị từ chối. API gửi email chỉ dành riêng cho giao tiếp nội bộ giữa các microservices.",
                    errors = new[] { "Unauthorized: Missing or invalid X-Internal-Secret header." }
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                return;
            }
        }

        await _next(context);
    }
}
