using System.Net.Http.Json;

namespace Donation.Api.Services.Implementations;

public class NotificationClient : Interfaces.INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationClient> _logger;

    public NotificationClient(HttpClient httpClient, ILogger<NotificationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SendDonationThankYouEmailAsync(
        string toEmail,
        string donorName,
        string transactionCode,
        decimal amount,
        string causeName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var htmlBody = $@"
<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;"">
    <div style=""text-align: center; margin-bottom: 24px;"">
        <h1 style=""color: #0d9488; margin: 0;"">Give-AID</h1>
        <p style=""color: #64748b; font-size: 14px; margin-top: 4px;"">Cổng Thông Tin & Quyên Góp Từ Thiện</p>
    </div>
    <div style=""background-color: #f0fdfa; border-left: 4px solid #0d9488; padding: 16px; margin-bottom: 20px;"">
        <h2 style=""color: #134e4a; margin: 0 0 8px 0; font-size: 18px;"">Thư Cảm Ơn Tấm Lòng Hảo Tâm</h2>
        <p style=""color: #334155; margin: 0; line-height: 1.5;"">
            Kính gửi <strong>{donorName}</strong>,<br/>
            Thay mặt những hoàn cảnh khó khăn, Give-AID chân thành cảm ơn sự đóng góp quý báu của bạn!
        </p>
    </div>
    <table style=""width: 100%; border-collapse: collapse; margin-bottom: 20px;"">
        <tr style=""border-bottom: 1px solid #e2e8f0;"">
            <td style=""padding: 10px 0; color: #64748b;"">Mã giao dịch:</td>
            <td style=""padding: 10px 0; text-align: right; font-weight: bold; color: #0f172a;"">{transactionCode}</td>
        </tr>
        <tr style=""border-bottom: 1px solid #e2e8f0;"">
            <td style=""padding: 10px 0; color: #64748b;"">Danh mục quyên góp:</td>
            <td style=""padding: 10px 0; text-align: right; font-weight: bold; color: #0f172a;"">{causeName}</td>
        </tr>
        <tr style=""border-bottom: 1px solid #e2e8f0;"">
            <td style=""padding: 10px 0; color: #64748b;"">Số tiền quyên góp:</td>
            <td style=""padding: 10px 0; text-align: right; font-weight: bold; color: #0d9488; font-size: 18px;"">{amount:N0} VNĐ</td>
        </tr>
        <tr>
            <td style=""padding: 10px 0; color: #64748b;"">Thời gian:</td>
            <td style=""padding: 10px 0; text-align: right; color: #0f172a;"">{DateTime.UtcNow:dd/MM/yyyy HH:mm} (UTC)</td>
        </tr>
    </table>
    <p style=""color: #64748b; font-size: 13px; line-height: 1.5; margin-top: 24px;"">
        Đây là giao dịch quyên góp mô phỏng trong môi trường thử nghiệm của Give-AID NGO Platform. Mọi thông tin đóng góp đều được lưu vết để phục vụ mục đích học tập và kiểm thử.
    </p>
</div>";

            var payload = new
            {
                ToEmail = toEmail,
                Subject = $"[Give-AID] Xác nhận quyên góp thành công - Mã GD: {transactionCode}",
                Body = htmlBody,
                IsHtml = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/notifications/emails/send", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Gửi email cảm ơn quyên góp thành công cho: {Email}", toEmail);
                return true;
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("NotificationService phản hồi lỗi khi gửi email quyên góp cho {Email}. StatusCode: {StatusCode}, Body: {Body}",
                toEmail, response.StatusCode, errorBody);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể kết nối tới NotificationService khi gửi email quyên góp cho {Email}", toEmail);
            return false;
        }
    }
}
