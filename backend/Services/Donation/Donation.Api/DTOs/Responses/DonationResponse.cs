namespace Donation.Api.DTOs.Responses;

public class DonationResponse
{
    public Guid Id { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public int CauseId { get; set; }
    public string CauseName { get; set; } = string.Empty;
    public string CauseSlug { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string DonorEmail { get; set; } = string.Empty;
    public string? DonorPhone { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? CardLastFour { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class DonationSummaryResponse
{
    public decimal TotalAmountRaised { get; set; }
    public int TotalDonationsCount { get; set; }
    public int SuccessfulDonationsCount { get; set; }
    public int PendingDonationsCount { get; set; }
    public int FailedDonationsCount { get; set; }
    public List<CauseStatisticItemResponse> CausesStats { get; set; } = new();
}

public class CauseStatisticItemResponse
{
    public int CauseId { get; set; }
    public string CauseName { get; set; } = string.Empty;
    public string CauseSlug { get; set; } = string.Empty;
    public int TotalDonations { get; set; }
    public int SuccessfulDonations { get; set; }
    public decimal TotalAmountRaised { get; set; }
}
