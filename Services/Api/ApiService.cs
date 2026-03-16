using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GpsGeoFence.DTOs;
using GpsGeoFence.Interfaces;
using GpsGeoFence.Models;

namespace GpsGeoFence.Services.Api;

/// <summary>
/// Implement IApiService — gọi Backend ASP.NET Core 10 API.
///
/// Base URL cấu hình trong MauiProgram.cs:
///   builder.Services.AddHttpClient&lt;IApiService, ApiService&gt;(c =>
///       c.BaseAddress = new Uri("https://your-api.azurewebsites.net/api/"));
///
/// Tất cả request đều:
///   - Có CancellationToken để cancel khi app background
///   - Trả về null / empty list khi lỗi (không throw)
///   - Ghi log chi tiết lỗi
/// </summary>
public class ApiService : IApiService
{
    // ──────────────────────────────────────────
    // FIELDS
    // ──────────────────────────────────────────
    private readonly HttpClient          _http;
    private readonly ILogger<ApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented               = false
    };

    // ──────────────────────────────────────────
    // CONSTRUCTOR
    // ──────────────────────────────────────────
    public ApiService(HttpClient http, ILogger<ApiService> logger)
    {
        _http   = http;
        _logger = logger;
    }

    // ══════════════════════════════════════════
    // POI ENDPOINTS
    // ══════════════════════════════════════════

    /// <summary>GET /api/pois — Lấy tất cả POI đang active.</summary>
    public async Task<List<PoiDto>> GetAllPoisAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<PoiDto>>(
                "pois", JsonOptions, ct);
            _logger.LogInformation("API: Nhận {Count} POIs.", result?.Count ?? 0);
            return result ?? [];
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API GetAllPois thất bại.");
            return [];
        }
    }

    /// <summary>GET /api/pois/{id}</summary>
    public async Task<PoiDto?> GetPoiByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<PoiDto>(
                $"pois/{id}", JsonOptions, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API GetPoiById({Id}) thất bại.", id);
            return null;
        }
    }

    /// <summary>GET /api/pois/qr/{qrCode}</summary>
    public async Task<PoiDto?> GetPoiByQrCodeAsync(string qrCode,
        CancellationToken ct = default)
    {
        try
        {
            var encoded = Uri.EscapeDataString(qrCode);
            return await _http.GetFromJsonAsync<PoiDto>(
                $"pois/qr/{encoded}", JsonOptions, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API GetPoiByQr({Qr}) thất bại.", qrCode);
            return null;
        }
    }

    // ══════════════════════════════════════════
    // AUDIO ENDPOINTS
    // ══════════════════════════════════════════

    /// <summary>GET /api/audio?poiId={id}&amp;language={lang}</summary>
    public async Task<List<AudioContentDto>> GetAudioByPoiAsync(
        int poiId, string language = "vi", CancellationToken ct = default)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<AudioContentDto>>(
                $"audio?poiId={poiId}&language={language}", JsonOptions, ct);
            return result ?? [];
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API GetAudioByPoi({Id}) thất bại.", poiId);
            return [];
        }
    }

    // ══════════════════════════════════════════
    // SYNC ENDPOINTS
    // ══════════════════════════════════════════

    /// <summary>
    /// POST /api/playback/sync — Đẩy PlaybackLogs chưa sync lên server.
    /// Gửi theo batch để giảm số lượng request.
    /// </summary>
    public async Task<bool> SyncPlaybackLogsAsync(
        IEnumerable<PlaybackLog> logs, CancellationToken ct = default)
    {
        var list = logs.ToList();
        if (list.Count == 0) return true;

        try
        {
            var payload = list.Select(l => new
            {
                l.UserId,
                l.PoiId,
                l.AudioContentId,
                TriggerType      = l.TriggerType.ToString(),
                l.PlayedAt,
                l.DurationListened,
                l.TotalDuration,
                l.IsSuccess,
                l.FailReason
            });

            var json     = JsonSerializer.Serialize(payload, JsonOptions);
            var content  = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("playback/sync", content, ct);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Sync {Count} PlaybackLogs thành công.", list.Count);
                return true;
            }

            _logger.LogWarning("Sync PlaybackLogs thất bại: {Status}",
                response.StatusCode);
            return false;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API SyncPlaybackLogs thất bại.");
            return false;
        }
    }

    /// <summary>
    /// POST /api/locations/sync — Đẩy UserLocations chưa sync lên server.
    /// </summary>
    public async Task<bool> SyncLocationsAsync(
        IEnumerable<UserLocation> locations, CancellationToken ct = default)
    {
        var list = locations.ToList();
        if (list.Count == 0) return true;

        try
        {
            var payload = list.Select(l => new
            {
                l.UserId,
                l.Latitude,
                l.Longitude,
                l.Accuracy,
                l.RecordedAt,
                l.SessionId,
                l.Speed
            });

            var json     = JsonSerializer.Serialize(payload, JsonOptions);
            var content  = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("locations/sync", content, ct);

            _logger.LogInformation(
                "Sync {Count} locations: {Status}",
                list.Count, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "API SyncLocations thất bại.");
            return false;
        }
    }

    // ══════════════════════════════════════════
    // HEALTH CHECK
    // ══════════════════════════════════════════

    /// <summary>GET /api/health — Kiểm tra API còn sống không.</summary>
    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        try
        {
            using var cts     = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var response = await _http.GetAsync("health", cts.Token);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
