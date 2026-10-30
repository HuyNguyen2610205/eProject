namespace Identity.Api.DTOs.Responses;

public class UserProfileResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
