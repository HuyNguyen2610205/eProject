using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ và tên không vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [MaxLength(20, ErrorMessage = "Số điện thoại không vượt quá 20 ký tự")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Đường dẫn ảnh đại diện không đúng định dạng URL")]
    [MaxLength(500, ErrorMessage = "Đường dẫn ảnh đại diện không vượt quá 500 ký tự")]
    public string? AvatarUrl { get; set; }
}
