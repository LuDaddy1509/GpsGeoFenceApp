using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MauiApp1.Services;

/// <summary>
/// Phát âm thanh thuyết minh POI.
/// - Nếu có AudioUrl  => phát qua Android MediaPlayer (native, không cần package ngoài)
/// - Nếu không có     => dùng TextToSpeech (TTS) built-in MAUI
/// - SemaphoreSlim đảm bảo không phát 2 nguồn cùng lúc
/// - KHÔNG dùng MediaElement (tránh lỗi CommunityToolkit)
/// </summary>
public sealed class AudioPlayerService : IAudioPlayerService
{
    private bool _isPlaying;
    private string? _currentPoiId;
    private CancellationTokenSource? _ttsCts;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public bool IsPlaying => _isPlaying;

    // ── Phát file audio từ URL ────────────────────────────────────────────
    public async Task PlayAsync(string audioUrl, string poiId)
    {
        if (!await _lock.WaitAsync(0))
        {
            Debug.WriteLine($"[Audio] Bỏ qua '{poiId}' - đang phát '{_currentPoiId}'.");
            return;
        }
        try
        {
            if (_isPlaying) { Debug.WriteLine($"[Audio] Đang phát, bỏ qua: {poiId}"); return; }
            _isPlaying = true;
            _currentPoiId = poiId;
            Debug.WriteLine($"[Audio] Phát: {audioUrl} (POI: {poiId})");
#if ANDROID
            await PlayAndroidAsync(audioUrl);
#else
            // iOS/Windows fallback -> TTS
            await TextToSpeech.Default.SpeakAsync($"Đang phát nội dung {poiId}");
#endif
        }
        catch (Exception ex) { Debug.WriteLine($"[Audio] Lỗi: {ex.Message}"); }
        finally
        {
            _isPlaying = false;
            _currentPoiId = null;
            _lock.Release();
        }
    }

    // ── TTS Fallback ──────────────────────────────────────────────────────
    public async Task SpeakAsync(string text, string poiId)
    {
        if (!await _lock.WaitAsync(0))
        {
            Debug.WriteLine($"[TTS] Bỏ qua '{poiId}' - đang bận.");
            return;
        }
        try
        {
            if (_isPlaying) { Debug.WriteLine($"[TTS] Đang phát, bỏ qua: {poiId}"); return; }
            _isPlaying = true;
            _currentPoiId = poiId;
            Debug.WriteLine($"[TTS] Doc: \"{text}\" (POI: {poiId})");
            _ttsCts = new CancellationTokenSource();
            var opts = new SpeechOptions { Volume = 1.0f, Pitch = 1.0f };
            await TextToSpeech.Default.SpeakAsync(text, opts, _ttsCts.Token);
            Debug.WriteLine($"[TTS] Xong: {poiId}");
        }
        catch (OperationCanceledException) { Debug.WriteLine($"[TTS] Bi huy: {poiId}"); }
        catch (Exception ex) { Debug.WriteLine($"[TTS] Loi: {ex.Message}"); }
        finally
        {
            _isPlaying = false;
            _currentPoiId = null;
            _ttsCts = null;
            _lock.Release();
        }
    }

    // ── Dừng ─────────────────────────────────────────────────────────────
    public void Stop()
    {
        _ttsCts?.Cancel();
        _isPlaying = false;
        _currentPoiId = null;
        Debug.WriteLine("[Audio] Da dung.");
    }

#if ANDROID
    // ── Android MediaPlayer native (không cần package ngoài) ─────────────
    private static async Task PlayAndroidAsync(string audioUrl)
    {
        var tcs = new TaskCompletionSource<bool>();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            Android.Media.MediaPlayer? player = null;
            try
            {
                player = new Android.Media.MediaPlayer();
                player.SetDataSource(audioUrl);
                player.Completion += (_, _) => { tcs.TrySetResult(true);  player?.Release(); };
                player.Error      += (_, e) => { tcs.TrySetResult(false); player?.Release();
                    Debug.WriteLine($"[Audio] Android error: {e.What}"); };
                player.Prepared   += (_, _) => player?.Start();
                player.PrepareAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Audio] Android init error: {ex.Message}");
                tcs.TrySetResult(false);
                player?.Release();
            }
        });
        // Chờ xong, timeout 5 phút
        await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromMinutes(5)));
    }
#endif
}