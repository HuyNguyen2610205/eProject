using System.ComponentModel.DataAnnotations;

namespace Content.Api.DTOs;

public class CreateQueryRequest
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(150, ErrorMessage = "Họ và tên không vượt quá 150 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [MaxLength(250, ErrorMessage = "Tiêu đề không vượt quá 250 ký tự")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung câu hỏi không được để trống")]
    public string Message { get; set; } = string.Empty;
}

public class ReplyQueryRequest
{
    [Required(ErrorMessage = "Nội dung phản hồi không được để trống")]
    public string ReplyMessage { get; set; } = string.Empty;

    public bool MarkAsResolved { get; set; } = false;
}

public class QueryResponse
{
    public int Id { get; set; }
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<QueryReplyResponse> Replies { get; set; } = new();
}

public class QueryReplyResponse
{
    public int Id { get; set; }
    public int QueryId { get; set; }
    public Guid ResponderUserId { get; set; }
    public string ReplyMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SendInvitationRequest
{
    [Required(ErrorMessage = "Email người nhận không được để trống")]
    [EmailAddress(ErrorMessage = "Email người nhận không hợp lệ")]
    [MaxLength(256)]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên người gửi không được để trống")]
    [MaxLength(150)]
    public string SenderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email người gửi không được để trống")]
    [EmailAddress(ErrorMessage = "Email người gửi không hợp lệ")]
    [MaxLength(256)]
    public string SenderEmail { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }
}

public class InvitationResponse
{
    public int Id { get; set; }
    public Guid? SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
