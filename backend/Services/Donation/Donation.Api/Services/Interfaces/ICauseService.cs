using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;

namespace Donation.Api.Services.Interfaces;

public interface ICauseService
{
    Task<ApiResponse<List<CauseResponse>>> GetAllCausesAsync(bool includeInactive = false);
    Task<ApiResponse<CauseResponse>> GetCauseByIdOrSlugAsync(string idOrSlug);
    Task<ApiResponse<CauseResponse>> CreateCauseAsync(CreateCauseRequest request);
    Task<ApiResponse<CauseResponse>> UpdateCauseAsync(int id, UpdateCauseRequest request);
    Task<ApiResponse<bool>> ToggleCauseStatusAsync(int id);
}
