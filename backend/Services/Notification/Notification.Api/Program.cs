using Notification.Api.Configurations;
using Notification.Api.Services.Implementations;
using Notification.Api.Services.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình SmtpOptions từ appsettings.json
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));

// 2. Đăng ký EmailSender Service
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();

// 3. Đăng ký Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Cấu hình HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Notification API");
        options.WithTheme(ScalarTheme.Moon);
    });
}

app.UseRouting();

// Map controllers
app.MapControllers();

app.Run();
