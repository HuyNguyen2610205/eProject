using System.Net.Http.Json;

namespace Identity.Api.Services.Implementations;

public class NotificationClient : Interfaces.INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationClient> _logger;

    public NotificationClient(HttpClient httpClient, ILogger<NotificationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SendVerificationEmailAsync(
        string email,
        string? fullName,
        string token,
        string? confirmationLink = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                email,
                fullName,
                token,
                confirmationLink
            };

            var response = await _httpClient.PostAsJsonAsync("api/notifications/emails/send-verification", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Gửi yêu cầu email xác thực sang NotificationService thành công cho: {Email}", email);
                return true;
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("NotificationService phản hồi lỗi khi gửi email xác thực cho {Email}. StatusCode: {StatusCode}, Body: {Body}",
                email, response.StatusCode, errorBody);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể kết nối tới NotificationService khi gửi email xác thực cho {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(
        string email,
        string? fullName,
        string token,
        string? resetLink = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                email,
                fullName,
                token,
                resetLink
            };

            var response = await _httpClient.PostAsJsonAsync("api/notifications/emails/send-password-reset", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Gửi yêu cầu email đặt lại mật khẩu sang NotificationService thành công cho: {Email}", email);
                return true;
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("NotificationService phản hồi lỗi khi gửi email đặt lại mật khẩu cho {Email}. StatusCode: {StatusCode}, Body: {Body}",
                email, response.StatusCode, errorBody);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể kết nối tới NotificationService khi gửi email đặt lại mật khẩu cho {Email}", email);
            return false;
        }
    }
}
