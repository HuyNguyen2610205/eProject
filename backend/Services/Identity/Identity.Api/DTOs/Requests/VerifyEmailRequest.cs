using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class VerifyEmailRequest
{
    [Required(ErrorMessage = "UserId không được để trống")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Mã xác thực không được để trống")]
    public string Token { get; set; } = string.Empty;
}
