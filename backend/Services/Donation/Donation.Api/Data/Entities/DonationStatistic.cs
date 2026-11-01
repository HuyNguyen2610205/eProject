using Microsoft.EntityFrameworkCore;

namespace Donation.Api.Data.Entities;

[Keyless]
public class DonationStatistic
{
    public int CauseId { get; set; }
    public string CauseName { get; set; } = string.Empty;
    public string CauseSlug { get; set; } = string.Empty;
    public int TotalDonations { get; set; }
    public int SuccessfulDonations { get; set; }
    public decimal TotalAmountRaised { get; set; }
    public decimal? MinDonationAmount { get; set; }
    public decimal? MaxDonationAmount { get; set; }
}
