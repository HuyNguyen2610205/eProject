using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;
using Donation.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/donations/[controller]")]
public class CausesController : ControllerBase
{
    private readonly ICauseService _causeService;

    public CausesController(ICauseService causeService)
    {
        _causeService = causeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CauseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCauses([FromQuery] bool includeInactive = false)
    {
        var result = await _causeService.GetAllCausesAsync(includeInactive);
        return Ok(result);
    }

    [HttpGet("{idOrSlug}")]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCause(string idOrSlug)
    {
        var result = await _causeService.GetCauseByIdOrSlugAsync(idOrSlug);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCause([FromBody] CreateCauseRequest request)
    {
        var result = await _causeService.CreateCauseAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetCause), new { idOrSlug = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CauseResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCause(int id, [FromBody] UpdateCauseRequest request)
    {
        var result = await _causeService.UpdateCauseAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPatch("{id:int}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleCauseStatus(int id)
    {
        var result = await _causeService.ToggleCauseStatusAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
