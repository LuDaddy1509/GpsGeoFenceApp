using System.Net.Http.Json;
using MauiApp1.Configuration;

namespace MauiApp1.Services.Api;

public sealed class PlaybackApiClient
{
    private readonly HttpClient _http;

    public PlaybackApiClient(HttpClient http) => _http = http;

    public async Task LogAsync(
        int poiId,
        string triggerType,
        int? durationSeconds = null,
        bool success = true,
        CancellationToken ct = default)
    {
        try
        {
            var userId = AuthApiClient.GetCurrentUserId();
            if (userId == Guid.Empty)
                return;

            var body = new
            {
                PoiId = poiId,
                UserId = userId,
                DurationSeconds = durationSeconds,
                TriggerType = triggerType,
                Success = success
            };

            using var resp = await _http.PostAsJsonAsync(ApiRoutes.Playbacks, body, ct);
            if (!resp.IsSuccessStatusCode)
                System.Diagnostics.Debug.WriteLine($"[Playback] Log failed: {resp.StatusCode}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Playback] LogAsync error: {ex.Message}");
        }
    }

    public async Task LogVisitAsync(int poiId, string triggerType, CancellationToken ct = default)
    {
        try
        {
            var userId = AuthApiClient.GetCurrentUserId();
            if (userId == Guid.Empty)
                return;

            var body = new
            {
                PoiId = poiId,
                UserId = userId,
                TriggerType = triggerType
            };

            using var resp = await _http.PostAsJsonAsync(ApiRoutes.Visits, body, ct);
            if (!resp.IsSuccessStatusCode)
                System.Diagnostics.Debug.WriteLine($"[Playback] Visit log failed: {resp.StatusCode}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Playback] LogVisitAsync error: {ex.Message}");
        }
    }
}
