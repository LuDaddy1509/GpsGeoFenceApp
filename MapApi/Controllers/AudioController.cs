using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApi.Data;

namespace MapApi.Controllers;

[ApiController]
[Route("api/audio")]
public sealed class AudioController : ControllerBase
{
    private readonly AppDb _db;
    public AudioController(AppDb db) => _db = db;
    // GET /api/audio/{poiId}?lang=vi-VN
    [HttpGet("{poiId}")]
    public async Task<IActionResult> GetAudio(string poiId, [FromQuery] string? lang, CancellationToken ct)
    {
        _ = lang;

        var audioUrl = await _db.Pois.AsNoTracking()
            .Where(p => p.Id == poiId && p.IsActive)
            .Select(p => p.AudioUrl)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(audioUrl))
            return NotFound(new { error = "Audio not found" });

        return Ok(new { poiId, audioUrl });
    }
}