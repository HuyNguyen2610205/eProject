using Content.Api.DTOs;
using Content.Api.DTOs.Responses;
using Content.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Content.Api.Controllers;

[ApiController]
[Route("api/content/about")]
public class AboutPagesController : ControllerBase
{
    private readonly IContentCmsService _cmsService;

    public AboutPagesController(IContentCmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AboutPageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPages()
    {
        var result = await _cmsService.GetAllAboutPagesAsync();
        return Ok(result);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ApiResponse<AboutPageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutPageResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPageBySlug(string slug)
    {
        var result = await _cmsService.GetAboutPageBySlugAsync(slug);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPut("{slug}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AboutPageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutPageResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePage(string slug, [FromBody] UpdateAboutPageRequest request)
    {
        Guid? adminUserId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(userIdClaim, out var parsedGuid))
        {
            adminUserId = parsedGuid;
        }

        var result = await _cmsService.UpdateAboutPageAsync(slug, request, adminUserId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

[ApiController]
[Route("api/content/partners")]
public class PartnersController : ControllerBase
{
    private readonly IContentCmsService _cmsService;

    public PartnersController(IContentCmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PartnerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPartners([FromQuery] bool includeInactive = false)
    {
        var result = await _cmsService.GetAllPartnersAsync(includeInactive);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PartnerResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PartnerResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartner([FromBody] CreatePartnerRequest request)
    {
        var result = await _cmsService.CreatePartnerAsync(request);
        return CreatedAtAction(nameof(GetAllPartners), result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PartnerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PartnerResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartner(int id, [FromBody] UpdatePartnerRequest request)
    {
        var result = await _cmsService.UpdatePartnerAsync(id, request);
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
    public async Task<IActionResult> DeletePartner(int id)
    {
        var result = await _cmsService.DeletePartnerAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}

[ApiController]
[Route("api/content/galleries")]
public class GalleriesController : ControllerBase
{
    private readonly IContentCmsService _cmsService;

    public GalleriesController(IContentCmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<GalleryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGalleries([FromQuery] int? programmeId = null)
    {
        var result = await _cmsService.GetAllGalleriesAsync(programmeId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<GalleryResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateGalleryItem([FromBody] CreateGalleryRequest request)
    {
        var result = await _cmsService.CreateGalleryItemAsync(request);
        return CreatedAtAction(nameof(GetAllGalleries), result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGalleryItem(int id)
    {
        var result = await _cmsService.DeleteGalleryItemAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
