using System.Net.Http.Json;

namespace Content.Api.Services.Implementations;

public class NotificationClient : Interfaces.INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationClient> _logger;

    public NotificationClient(HttpClient httpClient, ILogger<NotificationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SendQueryReplyEmailAsync(
        string toEmail,
        string fullName,
        string querySubject,
        string replyMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var htmlBody = $@"
<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;"">
    <div style=""text-align: center; margin-bottom: 24px;"">
        <h1 style=""color: #0d9488; margin: 0;"">Give-AID Help Centre</h1>
        <p style=""color: #64748b; font-size: 14px; margin-top: 4px;"">Phản hồi yêu cầu hỗ trợ / thắc mắc</p>
    </div>
    <div style=""background-color: #f8fafc; border-left: 4px solid #0d9488; padding: 16px; margin-bottom: 20px;"">
        <p style=""color: #334155; margin: 0 0 8px 0;"">Xin chào <strong>{fullName}</strong>,</p>
        <p style=""color: #334155; margin: 0; line-height: 1.5;"">
            Ban quản trị Give-AID đã phản hồi về câu hỏi: <strong>{querySubject}</strong>
        </p>
    </div>
    <div style=""background-color: #ffffff; border: 1px solid #e2e8f0; padding: 16px; border-radius: 6px; margin-bottom: 20px;"">
        <h3 style=""color: #0f172a; margin: 0 0 8px 0; font-size: 15px;"">Nội dung phản hồi từ Admin:</h3>
        <p style=""color: #334155; margin: 0; line-height: 1.6; white-space: pre-wrap;"">{replyMessage}</p>
    </div>
    <p style=""color: #64748b; font-size: 13px; line-height: 1.5; margin-top: 24px;"">
        Nếu bạn có thêm câu hỏi hoặc cần hỗ trợ thêm, bạn có thể gửi tiếp yêu cầu tại mục Help Centre trên website Give-AID.
    </p>
</div>";

            var payload = new
            {
                ToEmail = toEmail,
                Subject = $"[Give-AID] Phản hồi thắc mắc: {querySubject}",
                Body = htmlBody,
                IsHtml = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/notifications/emails/send", payload, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể kết nối NotificationService khi gửi phản hồi câu hỏi cho {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendFriendInvitationEmailAsync(
        string toEmail,
        string senderName,
        string? message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customMsg = string.IsNullOrWhiteSpace(message) 
                ? "Hãy cùng tôi tham gia vào các hoạt động thiện nguyện ý nghĩa trên Give-AID nhé!" 
                : message;

            var htmlBody = $@"
<div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;"">
    <div style=""text-align: center; margin-bottom: 24px;"">
        <h1 style=""color: #0d9488; margin: 0;"">Give-AID</h1>
        <p style=""color: #64748b; font-size: 14px; margin-top: 4px;"">Lời Mời Chung Tay Vì Cộng Đồng</p>
    </div>
    <div style=""background-color: #f0fdfa; border-left: 4px solid #0d9488; padding: 16px; margin-bottom: 20px;"">
        <p style=""color: #334155; margin: 0 0 8px 0; font-size: 16px;"">
            Bạn nhận được lời mời từ <strong>{senderName}</strong>:
        </p>
        <blockquote style=""margin: 0; color: #0f172a; font-style: italic; font-size: 15px;"">
            ""{customMsg}""
        </blockquote>
    </div>
    <p style=""color: #334155; line-height: 1.6;"">
        Give-AID là nền tảng kết nối các nhà hảo tâm với các tổ chức phi chính phủ uy tín tại Việt Nam, mang lại sự minh bạch và hỗ trợ thiết thực cho trẻ em, giáo dục và người yếu thế.
    </p>
</div>";

            var payload = new
            {
                ToEmail = toEmail,
                Subject = $"[Give-AID] {senderName} mời bạn cùng tham gia hoạt động thiện nguyện",
                Body = htmlBody,
                IsHtml = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/notifications/emails/send", payload, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể kết nối NotificationService khi gửi email mời bạn bè tới {Email}", toEmail);
            return false;
        }
    }
}
