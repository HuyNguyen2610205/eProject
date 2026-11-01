using Content.Api.DTOs;
using Content.Api.DTOs.Responses;

namespace Content.Api.Services.Interfaces;

public interface IQueryService
{
    Task<ApiResponse<QueryResponse>> SubmitQueryAsync(CreateQueryRequest request, Guid? userId = null);
    Task<ApiResponse<List<QueryResponse>>> GetAllQueriesAsync(string? status = null, int page = 1, int pageSize = 20);
    Task<ApiResponse<QueryResponse>> GetQueryByIdAsync(int id);
    Task<ApiResponse<List<QueryResponse>>> GetUserQueriesAsync(Guid userId);
    Task<ApiResponse<QueryResponse>> ReplyToQueryAsync(int id, ReplyQueryRequest request, Guid responderUserId);
}
