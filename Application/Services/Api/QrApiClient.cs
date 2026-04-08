using System;
using System.Net.Http.Json;

namespace MauiApp1.Services.Api;

/// <summary>
/// Client để lấy QR code cho POI từ backend
/// </summary>
public sealed class QrApiClient
{
    private readonly HttpClient _http;

    public QrApiClient(HttpClient http)
    {
        _http = http;
    }
    public async Task<byte[]?> GetQrCodePngAsync(
        string poiId,
        int size = 300,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"/api/v1/pois/{poiId}/qr?format=png&size={size}";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[QrApiClient] GetQrCodePng failed: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync(ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QrApiClient] GetQrCodePng error: {ex.Message}");
            return null;
        }
    }
    public async Task<string?> GetQrCodeSvgAsync(
        string poiId,
        int size = 300,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"/api/v1/pois/{poiId}/qr?format=svg&size={size}";
            return await _http.GetStringAsync(url, ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QrApiClient] GetQrCodeSvg error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Lấy POI + QR code URLs
    /// GET /api/v1/pois/{poiId}/details
    /// </summary>
    public async Task<PoiWithQrDto?> GetPoiWithQrAsync(
        string poiId,
        CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync($"/api/v1/pois/{poiId}/details", ct);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PoiWithQrDto>(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QrApiClient] GetPoiWithQr error: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// DTO: POI + QR URLs
/// </summary>
public record PoiWithQrDto(
    string Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string QrUrl,
    string QrSvgUrl);
