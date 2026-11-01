using Content.Api.DTOs;
using Content.Api.DTOs.Responses;

namespace Content.Api.Services.Interfaces;

public interface IProgrammeService
{
    Task<ApiResponse<List<ProgrammeResponse>>> GetAllProgrammesAsync(string? status = null, string? category = null, int? ngoId = null, bool? isFeatured = null);
    Task<ApiResponse<ProgrammeResponse>> GetProgrammeByIdAsync(int id);
    Task<ApiResponse<ProgrammeResponse>> CreateProgrammeAsync(CreateProgrammeRequest request);
    Task<ApiResponse<ProgrammeResponse>> UpdateProgrammeAsync(int id, UpdateProgrammeRequest request);
    Task<ApiResponse<bool>> DeleteProgrammeAsync(int id);
    Task<ApiResponse<ProgrammeRegistrationResponse>> RegisterProgrammeAsync(int programmeId, RegisterProgrammeRequest request, Guid? userId = null);
    Task<ApiResponse<List<ProgrammeRegistrationResponse>>> GetProgrammeRegistrationsAsync(int programmeId);
    Task<ApiResponse<List<ProgrammeRegistrationResponse>>> GetUserRegistrationsAsync(Guid userId);
}
