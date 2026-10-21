using System.Text;
using System.Text.Json;
using Identity.Api.Authorization;
using Identity.Api.Configurations;
using Identity.Api.Data;
using Identity.Api.Data.Entities;
using Identity.Api.DTOs.Responses;
using Identity.Api.Middlewares;
using Identity.Api.Services.Implementations;
using Identity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 0. Cấu hình Serilog từ appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// 1. Cấu hình DbContext với SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Cấu hình ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Cấu hình độ phức tạp mật khẩu
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Cấu hình người dùng & khóa tài khoản
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

// 3. Cấu hình JwtOptions & Authentication Middleware
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
var secretKey = Encoding.UTF8.GetBytes(jwtOptions.Secret);

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
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = !string.IsNullOrEmpty(jwtOptions.Issuer),
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtOptions.Audience),
        ValidAudience = jwtOptions.Audience,
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

            var response = ApiResponse<object>.Fail("Bạn chưa đăng nhập hoặc token không hợp lệ.");
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail("Bạn không có quyền thực hiện thao tác này.");
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// 4. Đăng ký Dependency Injection
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// 5. Cấu hình Controllers & OpenAPI & Chuẩn hóa lỗi Validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => 
                    string.IsNullOrWhiteSpace(err.ErrorMessage) ? "Dữ liệu không hợp lệ" : err.ErrorMessage))
                .ToList();

            var response = ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ.", errors);
            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddOpenApi();

// 6. Cấu hình Health Checks (kiểm tra Service & Database SQL Server)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppIdentityDbContext>(
        name: "sql_server_database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "ready" });

var app = builder.Build();

// 7. Global Exception Handling Middleware (Đặt ở đầu pipeline để bắt toàn bộ ngoại lệ)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Ghi log HTTP Requests qua Serilog
app.UseSerilogRequestLogging();

// 8. HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Identity & Auth API");
        options.WithTheme(ScalarTheme.Moon);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
// Tích hợp đọc Header nội bộ từ ApiGateway (Mô hình phòng thủ theo chiều sâu)
app.UseMiddleware<GatewayHeaderAuthenticationMiddleware>();
app.UseAuthorization();

// 9. Map Health Check Endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var isHealthy = report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
        context.Response.StatusCode = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

        var healthData = new
        {
            status = report.Status.ToString(),
            totalDuration = $"{report.TotalDuration.TotalMilliseconds:0.##} ms",
            checkedAt = DateTime.UtcNow,
            components = report.Entries.Select(e => new
            {
                component = e.Key,
                status = e.Value.Status.ToString(),
                duration = $"{e.Value.Duration.TotalMilliseconds:0.##} ms",
                description = e.Value.Description ?? (e.Value.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "Hoạt động bình thường" : "Gặp sự cố"),
                error = e.Value.Exception?.Message
            })
        };

        var response = isHealthy
            ? ApiResponse<object>.Ok(healthData, "Hệ thống và Cơ sở dữ liệu đang hoạt động bình thường.")
            : ApiResponse<object>.Fail("Một hoặc nhiều dịch vụ hệ thống đang gặp sự cố.", new List<string> { $"Trạng thái tổng thể: {report.Status}" });

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
});

app.MapControllers();

app.Run();
