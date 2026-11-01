using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;
using Donation.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly IDonationService _donationService;

    public DonationsController(IDonationService donationService)
    {
        _donationService = donationService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<DonationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DonationResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Donate([FromBody] DonateRequest request)
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

        var result = await _donationService.ProcessDonationAsync(request, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("track/{transactionCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<DonationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DonationResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TrackDonation(string transactionCode)
    {
        var result = await _donationService.GetDonationByCodeAsync(transactionCode);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("my-history")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<DonationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyDonationHistory()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<List<DonationResponse>>.Fail("Không thể xác thực thông tin định danh người dùng."));
        }

        var result = await _donationService.GetUserDonationHistoryAsync(userId);
        return Ok(result);
    }

    [HttpGet("admin/stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<DonationSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDonationStats()
    {
        var result = await _donationService.GetDonationSummaryAsync();
        return Ok(result);
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<DonationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllDonations(
        [FromQuery] string? status = null,
        [FromQuery] int? causeId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _donationService.GetAllDonationsAsync(status, causeId, page, pageSize);
        return Ok(result);
    }
}
