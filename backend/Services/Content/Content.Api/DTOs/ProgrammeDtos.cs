using System.ComponentModel.DataAnnotations;

namespace Content.Api.DTOs;

public class CreateProgrammeRequest : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng chọn tổ chức NGO chủ trì")]
    public int NgoId { get; set; }

    [Required(ErrorMessage = "Tiêu đề chương trình không được để trống")]
    [MaxLength(250, ErrorMessage = "Tiêu đề không vượt quá 250 ký tự")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(250)]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    public DateOnly EndDate { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Upcoming"; // Upcoming, Ongoing, Completed, Cancelled

    public bool IsFeatured { get; set; } = false;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", new[] { nameof(EndDate) });
        }
    }
}

public class UpdateProgrammeRequest : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng chọn tổ chức NGO chủ trì")]
    public int NgoId { get; set; }

    [Required(ErrorMessage = "Tiêu đề chương trình không được để trống")]
    [MaxLength(250, ErrorMessage = "Tiêu đề không vượt quá 250 ký tự")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(250)]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    public DateOnly EndDate { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Upcoming";

    public bool IsFeatured { get; set; } = false;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", new[] { nameof(EndDate) });
        }
    }
}

public class ProgrammeResponse
{
    public int Id { get; set; }
    public int NgoId { get; set; }
    public string NgoName { get; set; } = string.Empty;
    public string? NgoLogoUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int RegistrationsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RegisterProgrammeRequest
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(150, ErrorMessage = "Họ và tên không vượt quá 150 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(1000, ErrorMessage = "Lý do tham gia không vượt quá 1000 ký tự")]
    public string? Motivation { get; set; }
}

public class ProgrammeRegistrationResponse
{
    public int Id { get; set; }
    public int ProgrammeId { get; set; }
    public string ProgrammeTitle { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Motivation { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
