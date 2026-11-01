using Notification.Api.DTOs.Requests;

namespace Notification.Api.Services.Interfaces;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(EmailSendRequest request, CancellationToken cancellationToken = default);
    Task<bool> SendVerificationEmailAsync(SendVerificationEmailRequest request, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetEmailAsync(SendPasswordResetEmailRequest request, CancellationToken cancellationToken = default);
}
