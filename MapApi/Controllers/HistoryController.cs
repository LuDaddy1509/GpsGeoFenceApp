using MapApi.Common;
using MapApi.Dtos.History;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class HistoryController : ApiControllerBase
{
    private readonly HistoryService _historyService;

    public HistoryController(HistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpPost("history")]
    [AllowAnonymous]
    public async Task<ActionResult> LogPlaybackCompat([FromBody] PlaybackLogRequest request, CancellationToken ct)
    {
        var result = await _historyService.LogPlaybackAsync(request, ct);
        return result.Success ? Ok(new { ok = true }) : Error(StatusCodes.Status400BadRequest, "history_log_failed", result.Error!);
    }

    [HttpPost("playbacks")]
    [AllowAnonymous]
    public async Task<ActionResult> LogPlayback([FromBody] PlaybackLogRequest request, CancellationToken ct)
    {
        var result = await _historyService.LogPlaybackAsync(request, ct);
        return result.Success ? Ok(new { ok = true }) : Error(StatusCodes.Status400BadRequest, "playback_log_failed", result.Error!);
    }

    [HttpPost("visits")]
    [AllowAnonymous]
    public async Task<ActionResult> LogVisit([FromBody] VisitLogRequest request, CancellationToken ct)
    {
        var result = await _historyService.LogVisitAsync(request, ct);
        return result.Success ? Ok(new { ok = true }) : Error(StatusCodes.Status400BadRequest, "visit_log_failed", result.Error!);
    }

    [HttpGet("history/users/{userId:guid}")]
    [Authorize]
    public async Task<ActionResult> GetUserSummary(Guid userId, CancellationToken ct)
    {
        var summary = await _historyService.GetSummaryByUserAsync(userId, ct);
        return summary is null ? NotFoundError("User history not found.") : Ok(summary);
    }

    [HttpGet("history/pois/{poiId:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> GetPoiSummary(int poiId, CancellationToken ct)
    {
        var summary = await _historyService.GetSummaryByPoiAsync(poiId, ct);
        return Ok(summary);
    }
}
