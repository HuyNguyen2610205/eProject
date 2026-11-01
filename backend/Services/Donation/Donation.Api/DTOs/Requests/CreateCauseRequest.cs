using System.ComponentModel.DataAnnotations;

namespace Donation.Api.DTOs.Requests;

public class CreateCauseRequest
{
    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [MaxLength(150, ErrorMessage = "Tên danh mục không vượt quá 150 ký tự")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150, ErrorMessage = "Slug không vượt quá 150 ký tự")]
    public string? Slug { get; set; }

    [MaxLength(1000, ErrorMessage = "Mô tả không vượt quá 1000 ký tự")]
    public string? Description { get; set; }

    [MaxLength(500, ErrorMessage = "Đường dẫn Icon không vượt quá 500 ký tự")]
    public string? IconUrl { get; set; }

    [MaxLength(500, ErrorMessage = "Đường dẫn ảnh bìa không vượt quá 500 ký tự")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;
}

public class UpdateCauseRequest
{
    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [MaxLength(150, ErrorMessage = "Tên danh mục không vượt quá 150 ký tự")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150, ErrorMessage = "Slug không vượt quá 150 ký tự")]
    public string? Slug { get; set; }

    [MaxLength(1000, ErrorMessage = "Mô tả không vượt quá 1000 ký tự")]
    public string? Description { get; set; }

    [MaxLength(500, ErrorMessage = "Đường dẫn Icon không vượt quá 500 ký tự")]
    public string? IconUrl { get; set; }

    [MaxLength(500, ErrorMessage = "Đường dẫn ảnh bìa không vượt quá 500 ký tự")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;
}
