using System.ComponentModel.DataAnnotations;

namespace Content.Api.DTOs;

public class AboutPageResponse
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? BannerUrl { get; set; }
    public bool IsPublished { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateAboutPageRequest
{
    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [MaxLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung không được để trống")]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? BannerUrl { get; set; }

    public bool IsPublished { get; set; } = true;
}

public class PartnerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePartnerRequest
{
    [Required(ErrorMessage = "Tên đối tác không được để trống")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Logo URL không được để trống")]
    [MaxLength(500)]
    public string LogoUrl { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class UpdatePartnerRequest
{
    [Required(ErrorMessage = "Tên đối tác không được để trống")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Logo URL không được để trống")]
    [MaxLength(500)]
    public string LogoUrl { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class GalleryResponse
{
    public int Id { get; set; }
    public int? ProgrammeId { get; set; }
    public string? ProgrammeTitle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateGalleryRequest
{
    public int? ProgrammeId { get; set; }

    [Required(ErrorMessage = "Tiêu đề ảnh không được để trống")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; } = 0;
}
