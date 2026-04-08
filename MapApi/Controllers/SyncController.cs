using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApi.Data;
using MapApi.Models;

namespace MapApi.Controllers;

[ApiController]
[Route("api/sync")]
public sealed class SyncController : ControllerBase
{
    private readonly AppDb _db;
    public SyncController(AppDb db) => _db = db;

    // GET /api/sync/version
    [HttpGet("version")]
    public async Task<IActionResult> GetVersion(CancellationToken ct)
    {
        // Version đơn giản: Max UpdatedAt của POI active (có thể thay bằng bảng DatasetVersion nếu bạn muốn)
        var maxUpdated = await _db.Pois.AsNoTracking()
            .Where(p => p.IsActive)
            .MaxAsync(p => (DateTime?)p.UpdatedAt, ct);

        var count = await _db.Pois.AsNoTracking().CountAsync(p => p.IsActive, ct);

        return Ok(new
        {
            dataset = "pois",
            versionUtc = (maxUpdated ?? DateTime.UtcNow).ToString("O"),
            count
        });
    }
    // GET /api/sync/pois?lang=vi-VN&since=2026-04-01T00:00:00Z
    [HttpGet("pois")]
    public async Task<IActionResult> GetPois([FromQuery] string? lang, [FromQuery] DateTime? since, CancellationToken ct)
    {
       _ = lang;
        var q = _db.Pois.AsNoTracking().Where(p => p.IsActive);

        if (since.HasValue)
            q = q.Where(p => p.UpdatedAt > since.Value);
        var items = await q
            .OrderBy(p => p.Priority)
            .ThenBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Latitude,
                p.Longitude,
                p.RadiusMeters,
                p.NearRadiusMeters,
                p.DebounceSeconds,
                p.CooldownSeconds,
                p.Priority,
                p.MapLink,
                p.ImageUrl,
                p.AudioUrl,
                p.NarrationText,
                p.Language,
                p.IsActive,
                UpdatedAtUtc = p.UpdatedAt.ToUniversalTime().ToString("O")
            })
            .ToListAsync(ct);
        return Ok(items);
    }
    // GET /api/sync/config
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        // Config cơ bản (bạn có thể đưa xuống DB/table Config sau)
        return Ok(new
        {
            defaultRadiusMeters = 120,
            defaultNearRadiusMeters = 220,
            defaultDebounceSeconds = 3,
            defaultCooldownSeconds = 30,
            wifiOnlySync = true
        });
    }
}