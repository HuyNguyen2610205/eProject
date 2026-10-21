using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTOs.Requests;

public class LogoutRequest
{
    [Required(ErrorMessage = "RefreshToken không được để trống")]
    public string RefreshToken { get; set; } = string.Empty;
}
