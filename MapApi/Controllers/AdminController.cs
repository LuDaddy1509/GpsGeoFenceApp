using MapApi.Common;
using MapApi.Data;
using MapApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "admin")]
public sealed class AdminController : ApiControllerBase
{
    private readonly AppDb _db;
    private readonly PoiManagementService _poiService;

    public AdminController(AppDb db, PoiManagementService poiService)
    {
        _db = db;
        _poiService = poiService;
    }

    [HttpGet("seed/status")]
    public async Task<ActionResult> GetSeedStatus(CancellationToken ct)
    {
        var languages = await _db.PoiLanguages.AsNoTracking()
            .GroupBy(x => x.LanguageTag)
            .Select(g => new { Language = g.Key, Count = g.Count() })
            .OrderBy(x => x.Language)
            .ToListAsync(ct);

        return Ok(new
        {
            TotalPois = await _db.Pois.CountAsync(ct),
            TotalTranslations = languages.Sum(x => x.Count),
            ByLanguage = languages
        });
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard(CancellationToken ct)
    {
        var totalPois = await _db.Pois.CountAsync(ct);
        var activePois = await _db.Pois.CountAsync(x => x.IsActive, ct);
        var totalTranslations = await _db.PoiLanguages.CountAsync(ct);
        var totalHistoryRows = await _db.HistoryPoi.CountAsync(ct);
        var totalActivations = await _db.HistoryPoi.SumAsync(x => (int?)x.Quantity, ct) ?? 0;
        var totalListeningSeconds = await _db.HistoryPoi.SumAsync(x => (int?)x.TotalDurationSeconds, ct) ?? 0;

        var topPois = await _db.HistoryPoi.AsNoTracking()
            .GroupBy(x => new { x.IdPoi, x.PoiName })
            .Select(g => new
            {
                PoiId = g.Key.IdPoi,
                PoiName = g.Key.PoiName,
                Activations = g.Sum(x => x.Quantity),
                ListeningSeconds = g.Sum(x => x.TotalDurationSeconds ?? 0),
                LastVisitedAt = g.Max(x => x.LastVisitedAt)
            })
            .OrderByDescending(x => x.Activations)
            .ThenByDescending(x => x.LastVisitedAt)
            .Take(10)
            .ToListAsync(ct);

        return Ok(new
        {
            TotalPois = totalPois,
            ActivePois = activePois,
            InactivePois = totalPois - activePois,
            TotalTranslations = totalTranslations,
            TotalHistoryRows = totalHistoryRows,
            TotalActivations = totalActivations,
            TotalListeningSeconds = totalListeningSeconds,
            TopPois = topPois
        });
    }

    [HttpPost("translate-all")]
    public async Task<ActionResult> TranslateAll([FromQuery] bool overwrite = false, CancellationToken ct = default)
    {
        var pois = await _db.Pois.AsNoTracking().Where(x => x.IsActive).ToListAsync(ct);
        var log = new List<string>();
        var translated = 0;
        var skipped = 0;

        foreach (var poi in pois)
        {
            if (!overwrite)
            {
                var existingCount = await _db.PoiLanguages.CountAsync(x => x.IdPoi == poi.Id, ct);
                if (existingCount >= 1 + PoiManagementService.TargetLanguages.Length)
                {
                    skipped++;
                    log.Add($"[SKIP] {poi.Id} da co du ngon ngu.");
                    continue;
                }
            }

            var viRow = await _db.PoiLanguages.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdPoi == poi.Id && x.LanguageTag == "vi-VN", ct);

            await _poiService.AddOrUpdatePoiWithAutoTranslationAsync(
                poi,
                viRow?.TextToSpeech,
                poi.Description,
                new Progress<string>(message => log.Add(message)),
                ct);

            translated++;
        }

        return Ok(new { translated, skipped, log });
    }
}
