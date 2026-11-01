namespace Identity.Api.Services.Interfaces;

public interface INotificationClient
{
    Task<bool> SendVerificationEmailAsync(string email, string? fullName, string token, string? confirmationLink = null, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetEmailAsync(string email, string? fullName, string token, string? resetLink = null, CancellationToken cancellationToken = default);
}
