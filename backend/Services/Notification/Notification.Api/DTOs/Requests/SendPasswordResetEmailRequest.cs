using System.ComponentModel.DataAnnotations;

namespace Notification.Api.DTOs.Requests;

public class SendPasswordResetEmailRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    public string? FullName { get; set; }

    [Required(ErrorMessage = "Mã đặt lại mật khẩu không được để trống")]
    public string Token { get; set; } = string.Empty;

    public string? ResetLink { get; set; }
}
