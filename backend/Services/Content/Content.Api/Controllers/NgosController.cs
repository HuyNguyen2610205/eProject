using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Content.Api.Controllers;

[ApiController]
[Route("api/content/[controller]")]
public class NgosController : ControllerBase
{
    private readonly INgoService _ngoService;

    public NgosController(INgoService ngoService)
    {
        _ngoService = ngoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<NgoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllNgos([FromQuery] int? causeId = null, [FromQuery] bool includeInactive = false)
    {
        var result = await _ngoService.GetAllNgosAsync(causeId, includeInactive);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNgoById(int id)
    {
        var result = await _ngoService.GetNgoByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNgo([FromBody] CreateNgoRequest request)
    {
        var result = await _ngoService.CreateNgoAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetNgoById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NgoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateNgo(int id, [FromBody] UpdateNgoRequest request)
    {
        var result = await _ngoService.UpdateNgoAsync(id, request);
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
    public async Task<IActionResult> ToggleNgoStatus(int id)
    {
        var result = await _ngoService.ToggleNgoStatusAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
