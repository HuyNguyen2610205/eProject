using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Api.Data.Entities;

[Table("Ngos")]
public class Ngo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(250)]
    public string? Website { get; set; }

    [MaxLength(256)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<NgoCause> NgoCauses { get; set; } = new List<NgoCause>();
    public virtual ICollection<Programme> Programmes { get; set; } = new List<Programme>();
}

[Table("NgoCauses")]
public class NgoCause
{
    public int NgoId { get; set; }
    [ForeignKey(nameof(NgoId))]
    public virtual Ngo? Ngo { get; set; }

    public int CauseId { get; set; }
    [ForeignKey(nameof(CauseId))]
    public virtual Cause? Cause { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[Table("Causes")]
public class Cause
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<NgoCause> NgoCauses { get; set; } = new List<NgoCause>();
}
