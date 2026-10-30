using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Api.Configurations;
using Notification.Api.DTOs.Requests;
using Notification.Api.Services.Interfaces;

namespace Notification.Api.Services.Implementations;

public class MailKitEmailSender : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;
    private readonly ILogger<MailKitEmailSender> _logger;

    public MailKitEmailSender(IOptions<SmtpOptions> smtpOptions, ILogger<MailKitEmailSender> logger)
    {
        _smtpOptions = smtpOptions.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailSendRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
            message.To.Add(MailboxAddress.Parse(request.ToEmail));
            message.Subject = request.Subject;

            var bodyBuilder = new BodyBuilder();
            if (request.IsHtml)
            {
                bodyBuilder.HtmlBody = request.Body;
            }
            else
            {
                bodyBuilder.TextBody = request.Body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Cho phép bỏ qua kiểm tra chứng chỉ SSL trong môi trường phát triển cục bộ nếu cần
            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var secureSocketOption = _smtpOptions.Port == 465 
                ? SecureSocketOptions.SslOnConnect 
                : (_smtpOptions.EnableStartTls ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.Auto);

            _logger.LogInformation("Đang kết nối SMTP Server: {Host}:{Port} (SSL: {EnableSsl})...", _smtpOptions.Host, _smtpOptions.Port, _smtpOptions.EnableSsl);
            await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, secureSocketOption, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_smtpOptions.Username) && !string.IsNullOrWhiteSpace(_smtpOptions.Password))
            {
                await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Gửi email thành công tới: {ToEmail} với tiêu đề: {Subject}", request.ToEmail, request.Subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi email qua SMTP tới {ToEmail}. Host: {Host}:{Port}", request.ToEmail, _smtpOptions.Host, _smtpOptions.Port);
            return false;
        }
    }

    public async Task<bool> SendVerificationEmailAsync(SendVerificationEmailRequest request, CancellationToken cancellationToken = default)
    {
        var fullName = string.IsNullOrWhiteSpace(request.FullName) ? "Quý khách" : request.FullName.Trim();
        var subject = "[eProject] Xác thực địa chỉ Email của bạn";
        var link = string.IsNullOrWhiteSpace(request.ConfirmationLink) 
            ? $"http://localhost:5169/api/auth/verify-email?token={Uri.EscapeDataString(request.Token)}" 
            : request.ConfirmationLink;

        var htmlBody = BuildVerificationHtmlTemplate(fullName, request.Token, link);

        var sendRequest = new EmailSendRequest
        {
            ToEmail = request.Email,
            Subject = subject,
            Body = htmlBody,
            IsHtml = true
        };

        return await SendEmailAsync(sendRequest, cancellationToken);
    }

    public async Task<bool> SendPasswordResetEmailAsync(SendPasswordResetEmailRequest request, CancellationToken cancellationToken = default)
    {
        var fullName = string.IsNullOrWhiteSpace(request.FullName) ? "Quý khách" : request.FullName.Trim();
        var subject = "[eProject] Hướng dẫn đặt lại mật khẩu tài khoản";
        var link = string.IsNullOrWhiteSpace(request.ResetLink) 
            ? $"http://localhost:5169/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(request.Token)}" 
            : request.ResetLink;

        var htmlBody = BuildPasswordResetHtmlTemplate(fullName, request.Token, link);

        var sendRequest = new EmailSendRequest
        {
            ToEmail = request.Email,
            Subject = subject,
            Body = htmlBody,
            IsHtml = true
        };

        return await SendEmailAsync(sendRequest, cancellationToken);
    }

    private static string BuildVerificationHtmlTemplate(string fullName, string token, string link)
    {
        return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Xác thực Email</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f6f8; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333333;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f4f6f8; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); overflow: hidden;"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color: #0f172a; padding: 30px; text-align: center;"">
                            <h1 style=""margin: 0; color: #38bdf8; font-size: 24px; font-weight: bold; letter-spacing: 1px;"">eProject Platform</h1>
                        </td>
                    </tr>
                    <!-- Body Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #1e293b; font-size: 20px; margin-top: 0;"">Chào mừng {fullName},</h2>
                            <p style=""font-size: 15px; line-height: 1.6; color: #475569;"">
                                Cảm ơn bạn đã đăng ký tài khoản tại hệ thống <strong>eProject</strong>. Để hoàn tất quy trình kích hoạt và bảo vệ tài khoản, vui lòng xác nhận địa chỉ email của bạn.
                            </p>
                            <!-- CTA Button -->
                            <div style=""text-align: center; margin: 30px 0;"">
                                <a href=""{link}"" target=""_blank"" style=""display: inline-block; background-color: #2563eb; color: #ffffff; text-decoration: none; font-size: 16px; font-weight: 600; padding: 14px 32px; border-radius: 6px; box-shadow: 0 2px 4px rgba(37, 99, 235, 0.2);"">
                                    Xác thực địa chỉ Email
                                </a>
                            </div>
                            <!-- Verification Code Fallback -->
                            <div style=""background-color: #f1f5f9; border-radius: 6px; padding: 16px; margin: 25px 0;"">
                                <p style=""margin: 0 0 8px 0; font-size: 13px; font-weight: bold; color: #475569; text-transform: uppercase;"">Mã xác thực (Token):</p>
                                <p style=""margin: 0; font-family: monospace; font-size: 14px; word-break: break-all; color: #0f172a; background-color: #e2e8f0; padding: 8px; border-radius: 4px;"">
                                    {token}
                                </p>
                            </div>
                            <p style=""font-size: 14px; line-height: 1.5; color: #64748b; margin-top: 25px;"">
                                Liên kết xác thực này có hiệu lực trong vòng <strong>24 giờ</strong>. Nếu bạn không tạo tài khoản trên eProject, vui lòng bỏ qua thư này một cách an toàn.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0; font-size: 12px; color: #94a3b8;"">
                            <p style=""margin: 0 0 5px 0;"">© {DateTime.UtcNow.Year} eProject Cloud Architecture. Mọi quyền được bảo lưu.</p>
                            <p style=""margin: 0;"">Đây là email tự động, vui lòng không trả lời trực tiếp email này.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string BuildPasswordResetHtmlTemplate(string fullName, string token, string link)
    {
        return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Đặt lại mật khẩu</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f6f8; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333333;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f4f6f8; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); overflow: hidden;"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color: #0f172a; padding: 30px; text-align: center;"">
                            <h1 style=""margin: 0; color: #38bdf8; font-size: 24px; font-weight: bold; letter-spacing: 1px;"">eProject Security</h1>
                        </td>
                    </tr>
                    <!-- Body Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #1e293b; font-size: 20px; margin-top: 0;"">Chào {fullName},</h2>
                            <p style=""font-size: 15px; line-height: 1.6; color: #475569;"">
                                Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản eProject của bạn. Hãy bấm vào nút bên dưới để tiến hành tạo mật khẩu mới:
                            </p>
                            <!-- CTA Button -->
                            <div style=""text-align: center; margin: 30px 0;"">
                                <a href=""{link}"" target=""_blank"" style=""display: inline-block; background-color: #ef4444; color: #ffffff; text-decoration: none; font-size: 16px; font-weight: 600; padding: 14px 32px; border-radius: 6px; box-shadow: 0 2px 4px rgba(239, 68, 68, 0.2);"">
                                    Đặt lại mật khẩu của bạn
                                </a>
                            </div>
                            <!-- Reset Token Monospace Block -->
                            <div style=""background-color: #fef2f2; border: 1px solid #fee2e2; border-radius: 6px; padding: 16px; margin: 25px 0;"">
                                <p style=""margin: 0 0 8px 0; font-size: 13px; font-weight: bold; color: #b91c1c; text-transform: uppercase;"">Mã Token đặt lại mật khẩu:</p>
                                <p style=""margin: 0; font-family: monospace; font-size: 14px; word-break: break-all; color: #991b1b; background-color: #ffffff; padding: 8px; border-radius: 4px; border: 1px solid #fecaca;"">
                                    {token}
                                </p>
                            </div>
                            <!-- Warning Box -->
                            <div style=""border-left: 4px solid #f59e0b; background-color: #fffbeb; padding: 12px 16px; border-radius: 0 4px 4px 0; margin-top: 20px;"">
                                <p style=""margin: 0; font-size: 13px; color: #92400e; line-height: 1.5;"">
                                    <strong>Cảnh báo an toàn:</strong> Nếu bạn không yêu cầu hành động này, vui lòng bỏ qua email. Mật khẩu hiện tại của bạn sẽ không bị thay đổi.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0; font-size: 12px; color: #94a3b8;"">
                            <p style=""margin: 0 0 5px 0;"">© {DateTime.UtcNow.Year} eProject Cloud Architecture. Mọi quyền được bảo lưu.</p>
                            <p style=""margin: 0;"">Đây là email bảo mật hệ thống gửi tự động.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
