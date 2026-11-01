namespace Donation.Api.Services.Interfaces;

public interface INotificationClient
{
    Task<bool> SendDonationThankYouEmailAsync(string toEmail, string donorName, string transactionCode, decimal amount, string causeName, CancellationToken cancellationToken = default);
}
