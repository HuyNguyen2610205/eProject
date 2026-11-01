using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Donation.Api.Data.Entities;

[Table("Donations")]
public class Donation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string TransactionCode { get; set; } = string.Empty;

    public int CauseId { get; set; }

    [ForeignKey(nameof(CauseId))]
    public virtual Cause? Cause { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string DonorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string DonorEmail { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DonorPhone { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "MockCard";

    [MaxLength(10)]
    public string? CardLastFour { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }
}
