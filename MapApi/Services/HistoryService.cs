using MapApi.Data;
using MapApi.Dtos.History;
using MapApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Services;

public sealed class HistoryService
{
    private readonly AppDb _db;

    public HistoryService(AppDb db)
    {
        _db = db;
    }

    public async Task<(bool Success, string? Error)> LogPlaybackAsync(PlaybackLogRequest request, CancellationToken ct) =>
        request.Success
            ? await LogAsync(request.PoiId, request.UserId, request.DurationSeconds, ct)
            : (true, null);

    public async Task<(bool Success, string? Error)> LogVisitAsync(VisitLogRequest request, CancellationToken ct) =>
        await LogAsync(request.PoiId, request.UserId, request.DurationSeconds, ct);

    public async Task<HistorySummaryResponse?> GetSummaryByUserAsync(Guid userId, CancellationToken ct)
    {
        if (!await _db.Users.AsNoTracking().AnyAsync(x => x.UserId == userId, ct))
            return null;

        var history = await _db.HistoryPoi.AsNoTracking()
            .Where(x => x.IdUser == userId)
            .OrderByDescending(x => x.LastVisitedAt)
            .ToListAsync(ct);

        return new HistorySummaryResponse
        {
            UserId = userId,
            ByPoi = history.Select(x => new HistoryItemResponse
            {
                PoiId = x.IdPoi,
                PoiName = x.PoiName,
                Quantity = x.Quantity,
                LastVisitedAt = x.LastVisitedAt,
                TotalDurationSeconds = x.TotalDurationSeconds ?? 0
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<HistoryItemResponse>> GetSummaryByPoiAsync(int poiId, CancellationToken ct)
    {
        return await _db.HistoryPoi.AsNoTracking()
            .Where(x => x.IdPoi == poiId)
            .OrderByDescending(x => x.LastVisitedAt)
            .Select(x => new HistoryItemResponse
            {
                PoiId = x.IdPoi,
                PoiName = x.PoiName,
                Quantity = x.Quantity,
                LastVisitedAt = x.LastVisitedAt,
                TotalDurationSeconds = x.TotalDurationSeconds ?? 0
            })
            .ToListAsync(ct);
    }

    private async Task<(bool Success, string? Error)> LogAsync(int poiId, Guid userId, int? durationSeconds, CancellationToken ct)
    {
        var poi = await _db.Pois.AsNoTracking().FirstOrDefaultAsync(x => x.Id == poiId, ct);
        if (poi is null)
            return (false, "POI not found.");

        if (!await _db.Users.AsNoTracking().AnyAsync(x => x.UserId == userId, ct))
            return (false, "User not found.");

        var existing = await _db.HistoryPoi.FirstOrDefaultAsync(x => x.IdPoi == poiId && x.IdUser == userId, ct);
        if (existing is null)
        {
            _db.HistoryPoi.Add(new HistoryPoi
            {
                IdPoi = poiId,
                IdUser = userId,
                PoiName = poi.Name,
                Quantity = 1,
                LastVisitedAt = DateTime.UtcNow,
                TotalDurationSeconds = durationSeconds
            });
        }
        else
        {
            existing.Quantity += 1;
            existing.LastVisitedAt = DateTime.UtcNow;
            existing.TotalDurationSeconds = (existing.TotalDurationSeconds ?? 0) + (durationSeconds ?? 0);
        }

        await _db.SaveChangesAsync(ct);
        return (true, null);
    }
}
