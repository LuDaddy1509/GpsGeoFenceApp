using MapApi.Common;
using MapApi.Dtos.Pois;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/pois/{id:int}/media")]
public sealed class PoiMediaController : ApiControllerBase
{
    private readonly PoiManagementService _poiService;
    private readonly MediaStorageService _mediaStorageService;

    public PoiMediaController(PoiManagementService poiService, MediaStorageService mediaStorageService)
    {
        _poiService = poiService;
        _mediaStorageService = mediaStorageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> Get(int id, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        var media = await _poiService.GetMediaAsync(id, ct);
        return media is null ? NotFoundError("Media not found.") : Ok(media);
    }

    [HttpPut("links")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateLinks(int id, [FromBody] PoiMediaLinksUpdateRequest request, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        var media = await _poiService.UpsertMediaLinksAsync(id, request, ct);
        return Ok(media);
    }

    [HttpPost("image")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UploadImage(int id, IFormFile file, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        try
        {
            var url = await _mediaStorageService.SaveImageAsync(id, file, ct);
            var media = await _poiService.SetImageAsync(id, url, ct);
            return Ok(media);
        }
        catch (InvalidOperationException ex)
        {
            return Error(StatusCodes.Status400BadRequest, "invalid_file", ex.Message);
        }
    }

    [HttpPost("audio")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UploadAudio(int id, IFormFile file, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        try
        {
            var url = await _mediaStorageService.SaveAudioAsync(id, file, ct);
            var media = await _poiService.SetAudioAsync(id, url, ct);
            return Ok(media);
        }
        catch (InvalidOperationException ex)
        {
            return Error(StatusCodes.Status400BadRequest, "invalid_file", ex.Message);
        }
    }
}
