using Content.Api.Data;
using Content.Api.Data.Entities;
using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Services.Implementations;

public class QueryService : IQueryService
{
    private readonly ContentDbContext _dbContext;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<QueryService> _logger;

    public QueryService(
        ContentDbContext dbContext,
        INotificationClient notificationClient,
        ILogger<QueryService> logger)
    {
        _dbContext = dbContext;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<ApiResponse<QueryResponse>> SubmitQueryAsync(CreateQueryRequest request, Guid? userId = null)
    {
        var query = new Query
        {
            UserId = userId,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Queries.Add(query);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Người dùng {Email} đã gửi câu hỏi hỗ trợ mới (ID: {Id})", query.Email, query.Id);
        return await GetQueryByIdAsync(query.Id);
    }

    public async Task<ApiResponse<List<QueryResponse>>> GetAllQueriesAsync(string? status = null, int page = 1, int pageSize = 20)
    {
        var queryable = _dbContext.Queries
            .AsNoTracking()
            .Include(q => q.Replies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            queryable = queryable.Where(q => q.Status == status.Trim());
        }

        var queries = await queryable
            .OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => MapToResponse(q))
            .ToListAsync();

        return ApiResponse<List<QueryResponse>>.Ok(queries);
    }

    public async Task<ApiResponse<QueryResponse>> GetQueryByIdAsync(int id)
    {
        var query = await _dbContext.Queries
            .AsNoTracking()
            .Include(q => q.Replies)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (query == null)
        {
            return ApiResponse<QueryResponse>.Fail("Không tìm thấy câu hỏi hoặc yêu cầu hỗ trợ.");
        }

        return ApiResponse<QueryResponse>.Ok(MapToResponse(query));
    }

    public async Task<ApiResponse<List<QueryResponse>>> GetUserQueriesAsync(Guid userId)
    {
        var queries = await _dbContext.Queries
            .AsNoTracking()
            .Include(q => q.Replies)
            .Where(q => q.UserId == userId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => MapToResponse(q))
            .ToListAsync();

        return ApiResponse<List<QueryResponse>>.Ok(queries);
    }

    public async Task<ApiResponse<QueryResponse>> ReplyToQueryAsync(int id, ReplyQueryRequest request, Guid responderUserId)
    {
        var query = await _dbContext.Queries
            .Include(q => q.Replies)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (query == null)
        {
            return ApiResponse<QueryResponse>.Fail("Không tìm thấy câu hỏi cần phản hồi.");
        }

        var reply = new QueryReply
        {
            QueryId = id,
            ResponderUserId = responderUserId,
            ReplyMessage = request.ReplyMessage.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        query.Replies.Add(reply);
        query.Status = request.MarkAsResolved ? "Resolved" : "InProgress";
        if (request.MarkAsResolved)
        {
            query.ResolvedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Admin {AdminId} đã phản hồi câu hỏi ID {QueryId}. Trạng thái: {Status}", responderUserId, id, query.Status);

        // Bắn email thông báo phản hồi ngầm cho người dùng
        _ = Task.Run(async () =>
        {
            try
            {
                await _notificationClient.SendQueryReplyEmailAsync(
                    query.Email,
                    query.FullName,
                    query.Subject,
                    reply.ReplyMessage
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ngầm khi gửi email phản hồi câu hỏi tới {Email}", query.Email);
            }
        });

        return await GetQueryByIdAsync(id);
    }

    private static QueryResponse MapToResponse(Query q)
    {
        return new QueryResponse
        {
            Id = q.Id,
            UserId = q.UserId,
            FullName = q.FullName,
            Email = q.Email,
            Phone = q.Phone,
            Subject = q.Subject,
            Message = q.Message,
            Status = q.Status,
            CreatedAt = q.CreatedAt,
            ResolvedAt = q.ResolvedAt,
            Replies = q.Replies
                .OrderBy(r => r.CreatedAt)
                .Select(r => new QueryReplyResponse
                {
                    Id = r.Id,
                    QueryId = r.QueryId,
                    ResponderUserId = r.ResponderUserId,
                    ReplyMessage = r.ReplyMessage,
                    CreatedAt = r.CreatedAt
                }).ToList()
        };
    }
}
