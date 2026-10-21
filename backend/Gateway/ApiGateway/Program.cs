using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình xác thực JWT tập trung tại Gateway
var jwtSecret = builder.Configuration["JwtOptions:Secret"] 
    ?? throw new InvalidOperationException("JwtOptions:Secret is not configured.");
var jwtIssuer = builder.Configuration["JwtOptions:Issuer"];
var jwtAudience = builder.Configuration["JwtOptions:Audience"];
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                success = false,
                message = "Gateway: Bạn chưa đăng nhập hoặc token không hợp lệ / đã hết hạn.",
                errors = new[] { "Unauthorized" }
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                success = false,
                message = "Gateway: Bạn không có quyền truy cập tài nguyên này.",
                errors = new[] { "Forbidden" }
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    };
});

// 2. Chính sách phân quyền tại Gateway
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// 3. Cấu hình YARP Reverse Proxy với Request Transforms (Mô hình 1: Gắn Header nội bộ)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            // Luôn loại bỏ các Header nội bộ giả mạo từ client bên ngoài (Chống Header Injection/Spoofing)
            transformContext.ProxyRequest.Headers.Remove("X-User-Id");
            transformContext.ProxyRequest.Headers.Remove("X-User-Email");
            transformContext.ProxyRequest.Headers.Remove("X-User-Roles");
            transformContext.ProxyRequest.Headers.Remove("X-User-Permissions");

            var user = transformContext.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                // Trích xuất UserId
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? user.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    transformContext.ProxyRequest.Headers.Add("X-User-Id", userId);
                }

                // Trích xuất Email
                var email = user.FindFirst(ClaimTypes.Email)?.Value 
                         ?? user.FindFirst("email")?.Value;
                if (!string.IsNullOrEmpty(email))
                {
                    transformContext.ProxyRequest.Headers.Add("X-User-Email", email);
                }

                // Trích xuất Roles
                var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (roles.Count > 0)
                {
                    transformContext.ProxyRequest.Headers.Add("X-User-Roles", string.Join(",", roles));
                }

                // Trích xuất Permissions
                var permissions = user.FindAll("permission").Select(c => c.Value).ToList();
                if (permissions.Count > 0)
                {
                    transformContext.ProxyRequest.Headers.Add("X-User-Permissions", string.Join(",", permissions));
                }
            }

            return ValueTask.CompletedTask;
        });
    });

// 4. OpenAPI / Swagger cho Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Kích hoạt Middleware Xác thực & Phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Endpoint kiểm tra sức khỏe của Gateway
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "ApiGateway",
    timestamp = DateTime.UtcNow
}));

// Định tuyến YARP
app.MapReverseProxy();

app.Run();
