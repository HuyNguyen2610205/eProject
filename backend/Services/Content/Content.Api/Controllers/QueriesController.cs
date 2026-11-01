using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Content.Api.Controllers;

[ApiController]
[Route("api/content/[controller]")]
public class QueriesController : ControllerBase
{
    private readonly IQueryService _queryService;

    public QueriesController(IQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitQuery([FromBody] CreateQueryRequest request)
    {
        Guid? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (Guid.TryParse(userIdClaim, out var parsedGuid))
            {
                userId = parsedGuid;
            }
        }

        var result = await _queryService.SubmitQueryAsync(request, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQueryById(int id)
    {
        var result = await _queryService.GetQueryByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        // Chỉ Admin hoặc chính chủ sở hữu câu hỏi mới được xem chi tiết
        var isAdmin = User.IsInRole("Admin");
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var isOwner = Guid.TryParse(userIdClaim, out var currentUserId) && result.Data!.UserId == currentUserId;

        if (!isAdmin && !isOwner)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<QueryResponse>.Fail("Bạn không có quyền xem thông tin câu hỏi này."));
        }

        return Ok(result);
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<QueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllQueries(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _queryService.GetAllQueriesAsync(status, page, pageSize);
        return Ok(result);
    }

    [HttpGet("my-queries")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<QueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyQueries()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<List<QueryResponse>>.Fail("Không thể xác thực thông tin tài khoản."));
        }

        var result = await _queryService.GetUserQueriesAsync(userId);
        return Ok(result);
    }

    [HttpPost("{id:int}/reply")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<QueryResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReplyToQuery(int id, [FromBody] ReplyQueryRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var adminUserId))
        {
            return Unauthorized(ApiResponse<QueryResponse>.Fail("Không thể xác thực thông tin quản trị viên."));
        }

        var result = await _queryService.ReplyToQueryAsync(id, request, adminUserId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

[ApiController]
[Route("api/content/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IContentCmsService _cmsService;

    public InvitationsController(IContentCmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<InvitationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<InvitationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendInvitation([FromBody] SendInvitationRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var senderUserId))
        {
            return Unauthorized(ApiResponse<InvitationResponse>.Fail("Vui lòng đăng nhập để gửi lời mời bạn bè tham gia hoạt động từ thiện."));
        }

        var result = await _cmsService.SendFriendInvitationAsync(request, senderUserId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
