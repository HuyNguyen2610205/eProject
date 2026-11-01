using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Content.Api.Controllers;

[ApiController]
[Route("api/content/[controller]")]
public class ProgrammesController : ControllerBase
{
    private readonly IProgrammeService _programmeService;

    public ProgrammesController(IProgrammeService programmeService)
    {
        _programmeService = programmeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProgrammeResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProgrammes(
        [FromQuery] string? status = null,
        [FromQuery] string? category = null,
        [FromQuery] int? ngoId = null,
        [FromQuery] bool? isFeatured = null)
    {
        var result = await _programmeService.GetAllProgrammesAsync(status, category, ngoId, isFeatured);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgrammeById(int id)
    {
        var result = await _programmeService.GetProgrammeByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProgramme([FromBody] CreateProgrammeRequest request)
    {
        var result = await _programmeService.CreateProgrammeAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetProgrammeById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProgramme(int id, [FromBody] UpdateProgrammeRequest request)
    {
        var result = await _programmeService.UpdateProgrammeAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProgramme(int id)
    {
        var result = await _programmeService.DeleteProgrammeAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost("{id:int}/register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProgrammeRegistrationResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterProgramme(int id, [FromBody] RegisterProgrammeRequest request)
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

        var result = await _programmeService.RegisterProgrammeAsync(id, request, userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{id:int}/registrations")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<ProgrammeRegistrationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProgrammeRegistrations(int id)
    {
        var result = await _programmeService.GetProgrammeRegistrationsAsync(id);
        return Ok(result);
    }

    [HttpGet("my-registrations")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<ProgrammeRegistrationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRegistrations()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<List<ProgrammeRegistrationResponse>>.Fail("Không thể xác thực thông tin tài khoản."));
        }

        var result = await _programmeService.GetUserRegistrationsAsync(userId);
        return Ok(result);
    }
}
