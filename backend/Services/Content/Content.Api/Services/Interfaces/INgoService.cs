using Content.Api.DTOs;
using Content.Api.DTOs.Responses;

namespace Content.Api.Services.Interfaces;

public interface INgoService
{
    Task<ApiResponse<List<NgoResponse>>> GetAllNgosAsync(int? causeId = null, bool includeInactive = false);
    Task<ApiResponse<NgoResponse>> GetNgoByIdAsync(int id);
    Task<ApiResponse<NgoResponse>> CreateNgoAsync(CreateNgoRequest request);
    Task<ApiResponse<NgoResponse>> UpdateNgoAsync(int id, UpdateNgoRequest request);
    Task<ApiResponse<bool>> ToggleNgoStatusAsync(int id);
}
