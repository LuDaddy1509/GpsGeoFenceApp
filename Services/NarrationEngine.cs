using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MauiApp1.Models;

namespace MauiApp1.Services;

/// <summary>
/// Nhận Geofence/NEAR event -> quyết định phát audio hay TTS -> hiện Snackbar.
/// </summary>
public sealed class NarrationEngine
{
    private readonly IAudioPlayerService _audio;

    public NarrationEngine(IAudioPlayerService audio)
    {
        _audio = audio;
    }

    /// <summary>
    /// Gọi từ MapPage khi Geofence (ENTER/EXIT/DWELL) hoặc NEAR trigger.
    /// An toàn để gọi từ background thread.
    /// </summary>
    public async Task TriggerAsync(Poi poi, string eventType)
    {
        Debug.WriteLine($"[Narration] Trigger: {eventType} -> {poi.Name}");

        // Hiện Snackbar nhẹ trên UI (không block)
        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var icon = eventType switch
                {
                    "ENTER" => "Vao vung:",
                    "DWELL" => "Dang o:",
                    "NEAR" => "Den gan:",
                    _ => "POI:"
                };
                await AppShell.DisplaySnackbarAsync($"{icon} {poi.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Narration] Snackbar loi: {ex.Message}");
            }
        });

        // Phát audio hoặc TTS
        if (!string.IsNullOrWhiteSpace(poi.AudioUrl))
        {
            Debug.WriteLine($"[Narration] Phat audio: {poi.AudioUrl}");
            await _audio.PlayAsync(poi.AudioUrl, poi.Id);
        }
        else
        {
            var text = !string.IsNullOrWhiteSpace(poi.NarrationText)
                ? poi.NarrationText
                : !string.IsNullOrWhiteSpace(poi.Description)
                    ? poi.Description
                    : $"Ban dang den gan {poi.Name}";

            Debug.WriteLine($"[Narration] TTS: \"{text}\"");
            await _audio.SpeakAsync(text, poi.Id);
        }
    }
}