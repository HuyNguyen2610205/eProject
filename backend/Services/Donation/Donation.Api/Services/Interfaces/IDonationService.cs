using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;

namespace Donation.Api.Services.Interfaces;

public interface IDonationService
{
    Task<ApiResponse<DonationResponse>> ProcessDonationAsync(DonateRequest request, Guid? userId = null);
    Task<ApiResponse<DonationResponse>> GetDonationByCodeAsync(string transactionCode);
    Task<ApiResponse<List<DonationResponse>>> GetUserDonationHistoryAsync(Guid userId);
    Task<ApiResponse<DonationSummaryResponse>> GetDonationSummaryAsync();
    Task<ApiResponse<List<DonationResponse>>> GetAllDonationsAsync(string? status = null, int? causeId = null, int page = 1, int pageSize = 20);
}
