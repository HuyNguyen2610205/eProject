namespace Content.Api.Services.Interfaces;

public interface INotificationClient
{
    Task<bool> SendQueryReplyEmailAsync(string toEmail, string fullName, string querySubject, string replyMessage, CancellationToken cancellationToken = default);
    Task<bool> SendFriendInvitationEmailAsync(string toEmail, string senderName, string? message, CancellationToken cancellationToken = default);
}
