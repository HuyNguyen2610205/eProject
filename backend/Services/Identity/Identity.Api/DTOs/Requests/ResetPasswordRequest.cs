using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã xác thực không được để trống")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
