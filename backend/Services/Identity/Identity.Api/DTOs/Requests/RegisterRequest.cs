using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class RegisterRequest : IValidatableObject
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải có độ dài từ 2 đến 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    public string? ConfirmPassword { get; set; }

    [MaxLength(200, ErrorMessage = "Nghề nghiệp không vượt quá 200 ký tự")]
    public string? Occupation { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(ConfirmPassword) && !string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "Mật khẩu xác nhận không khớp",
                new[] { nameof(ConfirmPassword) }
            );
        }
    }
}
