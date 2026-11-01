using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;
}
