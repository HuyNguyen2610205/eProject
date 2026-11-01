using System.ComponentModel.DataAnnotations;

namespace Content.Api.DTOs;

public class CreateNgoRequest
{
    [Required(ErrorMessage = "Tên tổ chức NGO không được để trống")]
    [MaxLength(250, ErrorMessage = "Tên tổ chức không vượt quá 250 ký tự")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(250)]
    public string? Website { get; set; }

    [EmailAddress(ErrorMessage = "Email liên hệ không hợp lệ")]
    [MaxLength(256)]
    public string? ContactEmail { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    public List<int>? CauseIds { get; set; }
}

public class UpdateNgoRequest
{
    [Required(ErrorMessage = "Tên tổ chức NGO không được để trống")]
    [MaxLength(250, ErrorMessage = "Tên tổ chức không vượt quá 250 ký tự")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(250)]
    public string? Website { get; set; }

    [EmailAddress(ErrorMessage = "Email liên hệ không hợp lệ")]
    [MaxLength(256)]
    public string? ContactEmail { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    public List<int>? CauseIds { get; set; }
}

public class NgoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<NgoCauseItemResponse> Causes { get; set; } = new();
    public int TotalProgrammesCount { get; set; }
}

public class NgoCauseItemResponse
{
    public int CauseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
