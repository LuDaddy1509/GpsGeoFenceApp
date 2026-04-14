using MapApi.Common;
using MapApi.Dtos.Pois;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/pois")]
public sealed class PoiController : ApiControllerBase
{
    private readonly PoiManagementService _poiService;

    public PoiController(PoiManagementService poiService)
    {
        _poiService = poiService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetList([FromQuery] PoiListQuery query, CancellationToken ct)
    {
        var result = await _poiService.GetPagedAsync(query, ct);
        return Ok(result.Items);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult> Search([FromQuery] PoiListQuery query, CancellationToken ct)
    {
        var result = await _poiService.GetPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetDetail(int id, [FromQuery] string? lang, CancellationToken ct)
    {
        var poi = await _poiService.GetDetailAsync(id, lang, ct);
        return poi is null ? NotFoundError("POI not found.") : Ok(poi);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Create([FromBody] PoiUpsertRequest request, CancellationToken ct)
    {
        var poi = await _poiService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetDetail), new { id = poi.Id, lang = poi.Language }, poi);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Update(int id, [FromBody] PoiUpsertRequest request, CancellationToken ct)
    {
        var poi = await _poiService.UpdateAsync(id, request, ct);
        return poi is null ? NotFoundError("POI not found.") : Ok(poi);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> SetStatus(int id, [FromBody] PoiStatusUpdateRequest request, CancellationToken ct)
    {
        var poi = await _poiService.SetStatusAsync(id, request.IsActive, ct);
        return poi is null ? NotFoundError("POI not found.") : Ok(poi);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _poiService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFoundError("POI not found.");
    }

    [HttpGet("{id:int}/languages")]
    [AllowAnonymous]
    public async Task<ActionResult> GetLanguages(int id, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        var data = await _poiService.GetLanguagesAsync(id, ct);
        return Ok(data);
    }

    [HttpPut("{id:int}/languages/{languageTag}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpsertLanguage(int id, string languageTag, [FromBody] PoiLanguageUpsertRequest request, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        request.LanguageTag = languageTag;
        var row = await _poiService.UpsertLanguageAsync(id, request, ct);
        return Ok(row);
    }

    [HttpDelete("{id:int}/languages/{languageTag}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteLanguage(int id, string languageTag, CancellationToken ct)
    {
        if (!await _poiService.ExistsAsync(id, ct))
            return NotFoundError("POI not found.");

        var deleted = await _poiService.DeleteLanguageAsync(id, languageTag, ct);
        return deleted ? NoContent() : NotFoundError("Language record not found.");
    }
}
