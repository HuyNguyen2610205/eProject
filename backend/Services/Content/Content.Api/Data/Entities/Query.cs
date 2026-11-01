using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Api.Data.Entities;

[Table("Queries")]
public class Query
{
    [Key]
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(250)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Open"; // Open, InProgress, Resolved

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    public virtual ICollection<QueryReply> Replies { get; set; } = new List<QueryReply>();
}

[Table("QueryReplies")]
public class QueryReply
{
    [Key]
    public int Id { get; set; }

    public int QueryId { get; set; }

    [ForeignKey(nameof(QueryId))]
    public virtual Query? Query { get; set; }

    public Guid ResponderUserId { get; set; }

    [Required]
    public string ReplyMessage { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[Table("EmailInvitations")]
public class EmailInvitation
{
    [Key]
    public int Id { get; set; }

    public Guid? SenderUserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string SenderName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string SenderEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string RecipientEmail { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Sent"; // Sent, Failed

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
