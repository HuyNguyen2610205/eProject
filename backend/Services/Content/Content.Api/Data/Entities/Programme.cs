using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Api.Data.Entities;

[Table("Programmes")]
public class Programme
{
    [Key]
    public int Id { get; set; }

    public int NgoId { get; set; }

    [ForeignKey(nameof(NgoId))]
    public virtual Ngo? Ngo { get; set; }

    [Required]
    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(250)]
    public string? Location { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Upcoming";

    public bool IsFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProgrammeRegistration> Registrations { get; set; } = new List<ProgrammeRegistration>();
    public virtual ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();
}

[Table("ProgrammeRegistrations")]
public class ProgrammeRegistration
{
    [Key]
    public int Id { get; set; }

    public int ProgrammeId { get; set; }

    [ForeignKey(nameof(ProgrammeId))]
    public virtual Programme? Programme { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(1000)]
    public string? Motivation { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
