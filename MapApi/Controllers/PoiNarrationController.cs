using MapApi.Common;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/pois/{id:int}/narration")]
public sealed class PoiNarrationController : ApiControllerBase
{
    private readonly NarrationService _narrationService;

    public PoiNarrationController(NarrationService narrationService)
    {
        _narrationService = narrationService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> Get(int id, [FromQuery] string? lang, [FromQuery] string? eventType, CancellationToken ct)
    {
        var response = await _narrationService.GetNarrationAsync(id, lang, eventType, ct);
        return response is null ? NotFoundError("POI not found.") : Ok(response);
    }
}
