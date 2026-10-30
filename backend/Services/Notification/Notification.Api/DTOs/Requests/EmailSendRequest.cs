using System.ComponentModel.DataAnnotations;

namespace Notification.Api.DTOs.Requests;

public class EmailSendRequest
{
    [Required(ErrorMessage = "Email người nhận không được để trống")]
    [EmailAddress(ErrorMessage = "Email người nhận không hợp lệ")]
    public string ToEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tiêu đề email không được để trống")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung email không được để trống")]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; } = true;
}
