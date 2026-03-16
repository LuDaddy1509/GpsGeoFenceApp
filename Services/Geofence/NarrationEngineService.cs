using GpsGeoFence.DTOs;
using GpsGeoFence.Enums;
using GpsGeoFence.Interfaces;
using GpsGeoFence.Models;

namespace GpsGeoFence.Services.Geofence;

/// <summary>
/// Narration Engine — nhận yêu cầu phát thuyết minh từ GeofenceService,
/// quyết định có phát không (cooldown, đang phát), chọn nguồn audio tốt nhất,
/// và ghi PlaybackLog.
///
/// Thứ tự ưu tiên nguồn audio:
///   1. File audio (AudioUrl) – giọng tự nhiên, chuyên nghiệp
///   2. TTS Script – phát tức thì, không cần download
/// </summary>
public class NarrationEngineService : INarrationEngine
{
    // ──────────────────────────────────────────
    // FIELDS
    // ──────────────────────────────────────────
    private readonly IAudioPlayerService  _audioPlayer;
    private readonly ILocalCacheService   _cache;
    private readonly ILogger<NarrationEngineService> _logger;

    /// <summary>Cooldown: thời điểm phát cuối cùng theo poiId.</summary>
    private readonly Dictionary<int, DateTime> _cooldowns = [];
    private readonly SemaphoreSlim _playSemaphore = new(1, 1);

    // ──────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────
    public int    CooldownSeconds   { get; set; } = 30;
    public string PreferredLanguage { get; set; } = "vi";
    public bool   IsPlaying         => _audioPlayer.IsPlaying;
    public PoiDto? CurrentPlayingPoi { get; private set; }

    // ──────────────────────────────────────────
    // EVENTS
    // ──────────────────────────────────────────
    public event EventHandler<PoiDto>?          NarrationStarted;
    public event EventHandler<PlaybackResult>?  NarrationCompleted;

    // ──────────────────────────────────────────
    // CONSTRUCTOR
    // ──────────────────────────────────────────
    public NarrationEngineService(
        IAudioPlayerService audioPlayer,
        ILocalCacheService cache,
        ILogger<NarrationEngineService> logger)
    {
        _audioPlayer = audioPlayer;
        _cache       = cache;
        _logger      = logger;

        // Đăng ký sự kiện khi audio phát xong
        _audioPlayer.PlaybackEnded += OnPlaybackEnded;
        _audioPlayer.PlaybackError += OnPlaybackError;
    }

    // ──────────────────────────────────────────
    // PUBLIC — TRIGGER
    // ──────────────────────────────────────────

    /// <summary>
    /// Thử phát thuyết minh cho POI.
    /// Engine tự kiểm tra cooldown, trạng thái đang phát, và chọn nguồn tốt nhất.
    /// </summary>
    public async Task<PlaybackResult> TriggerAsync(PoiDto poi, TriggerType trigger)
    {
        // ── Guard: cooldown ────────────────────
        if (trigger != TriggerType.Manual && IsInCooldown(poi.Id))
        {
            var remaining = CooldownRemaining(poi.Id);
            _logger.LogDebug("POI #{Id} đang cooldown ({Sec:F0}s còn lại).",
                poi.Id, remaining.TotalSeconds);
            return PlaybackResult.Skipped($"Cooldown {remaining.TotalSeconds:F0}s còn lại");
        }

        // ── Guard: đang phát và không phải Manual ─
        if (IsPlaying && trigger != TriggerType.Manual)
        {
            _logger.LogDebug("Đang phát thuyết minh khác, bỏ qua POI #{Id}.", poi.Id);
            return PlaybackResult.Skipped("Đang phát thuyết minh khác");
        }

        // ── Chờ semaphore (tránh đồng thời) ───
        if (!await _playSemaphore.WaitAsync(500))
            return PlaybackResult.Skipped("Bận xử lý request trước");

        try
        {
            return await DoPlayAsync(poi, trigger);
        }
        finally
        {
            _playSemaphore.Release();
        }
    }

    // ──────────────────────────────────────────
    // PUBLIC — COOLDOWN
    // ──────────────────────────────────────────

    public bool IsInCooldown(int poiId)
    {
        if (!_cooldowns.TryGetValue(poiId, out var lastPlayed)) return false;
        return (DateTime.UtcNow - lastPlayed).TotalSeconds < CooldownSeconds;
    }

    public void ResetCooldown(int poiId)
    {
        _cooldowns.Remove(poiId);
        _logger.LogDebug("Reset cooldown POI #{Id}.", poiId);
    }

    public void ResetAllCooldowns()
    {
        _cooldowns.Clear();
        _logger.LogDebug("Reset toàn bộ cooldown.");
    }

    public async Task StopAsync()
    {
        await _audioPlayer.StopAsync();
        CurrentPlayingPoi = null;
    }

    // ──────────────────────────────────────────
    // PRIVATE — PLAY LOGIC
    // ──────────────────────────────────────────

    private async Task<PlaybackResult> DoPlayAsync(PoiDto poi, TriggerType trigger)
    {
        // 1. Chọn AudioContent tốt nhất — dùng trực tiếp từ poi được truyền vào
        var audio = SelectBestAudio(poi.AudioContents);

        if (audio is null)
        {
            _logger.LogWarning("POI #{Id} không có AudioContent.", poi.Id);
            return PlaybackResult.Failure(poi.Id, "Không có nội dung âm thanh", trigger);
        }

        // 2. Cập nhật cooldown ngay trước khi phát (tránh race condition)
        _cooldowns[poi.Id] = DateTime.UtcNow;
        CurrentPlayingPoi  = poi;

        // 3. Notify UI
        await MainThread.InvokeOnMainThreadAsync(
            () => NarrationStarted?.Invoke(this, poi));

        try
        {
            // 4. Phát — ưu tiên file audio, fallback TTS
            if (audio.HasAudioFile())
            {
                _logger.LogInformation(
                    "Phát audio file: POI #{Id} [{Lang}] {Url}",
                    poi.Id, audio.Language, audio.AudioUrl);
                await _audioPlayer.PlayAsync(audio.AudioUrl!);
            }
            else if (audio.HasTtsScript())
            {
                _logger.LogInformation(
                    "Phát TTS: POI #{Id} [{Lang}]", poi.Id, audio.Language);
                await PlayTtsAsync(audio.TtsScript!, audio.Language);
            }
            else
            {
                return PlaybackResult.Failure(poi.Id, "Nguồn audio không hợp lệ", trigger);
            }

            // 5. Tạo log và ghi
            var result = PlaybackResult.Success(
                poi.Id, audio.Id, trigger, audio.DurationSeconds);

            await LogPlaybackAsync(result, audio.DurationSeconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi phát thuyết minh POI #{Id}.", poi.Id);
            CurrentPlayingPoi = null;

            var failed = PlaybackResult.Failure(poi.Id, ex.Message, trigger);
            await LogPlaybackAsync(failed, 0);
            return failed;
        }
    }

    // ──────────────────────────────────────────
    // PRIVATE — AUDIO SELECTION
    // ──────────────────────────────────────────

    /// <summary>
    /// Chọn AudioContent tốt nhất theo thứ tự:
    ///   1. Ngôn ngữ ưa thích + IsDefault
    ///   2. Ngôn ngữ ưa thích bất kỳ
    ///   3. IsDefault bất kỳ ngôn ngữ
    ///   4. Bất kỳ
    /// </summary>
    private AudioContentDto? SelectBestAudio(IEnumerable<AudioContentDto> contents)
    {
        var list = contents?.ToList() ?? [];
        return list.FirstOrDefault(a => a.Language == PreferredLanguage && a.IsDefault)
            ?? list.FirstOrDefault(a => a.Language == PreferredLanguage)
            ?? list.FirstOrDefault(a => a.IsDefault)
            ?? list.FirstOrDefault();
    }

    // ──────────────────────────────────────────
    // PRIVATE — TTS
    // ──────────────────────────────────────────

    /// <summary>
    /// Phát TTS dùng MAUI TextToSpeech API.
    /// PACKAGE: đã có sẵn trong .NET MAUI
    /// </summary>
    private static async Task PlayTtsAsync(string script, string language)
    {
        var settings = new SpeechOptions
        {
            Locale = GetLocale(language),
            Volume = 1.0f,
            Pitch  = 1.0f
        };
        await TextToSpeech.Default.SpeakAsync(script, settings);
    }

    private static Locale? GetLocale(string language)
    {
        var locales = TextToSpeech.Default.GetLocalesAsync().GetAwaiter().GetResult();
        return locales.FirstOrDefault(l =>
            l.Language.StartsWith(language, StringComparison.OrdinalIgnoreCase));
    }

    // ──────────────────────────────────────────
    // PRIVATE — LOGGING
    // ──────────────────────────────────────────

    private async Task LogPlaybackAsync(PlaybackResult result, int duration)
    {
        var log = new PlaybackLog
        {
            PoiId           = result.PoiId,
            AudioContentId  = result.AudioContentId,
            TriggerType     = result.TriggerType,
            PlayedAt        = result.StartedAt,
            TotalDuration   = duration,
            DurationListened = result.DurationListened,
            IsSuccess       = result.IsSuccess,
            FailReason      = result.ErrorMessage,
            IsSynced        = false
        };

        try
        {
            await _cache.SavePlaybackLogAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không ghi được PlaybackLog.");
        }
    }

    // ──────────────────────────────────────────
    // PRIVATE — AUDIO EVENTS
    // ──────────────────────────────────────────

    private void OnPlaybackEnded(object? sender, EventArgs e)
    {
        var poi = CurrentPlayingPoi;
        CurrentPlayingPoi = null;

        if (poi is null) return;

        var result = PlaybackResult.Success(poi.Id, 0,
            TriggerType.Geofence,
            (int)_audioPlayer.Duration);

        MainThread.BeginInvokeOnMainThread(
            () => NarrationCompleted?.Invoke(this, result));

        _logger.LogInformation("Thuyết minh POI #{Id} hoàn thành.", poi.Id);
    }

    private void OnPlaybackError(object? sender, string error)
    {
        var poi = CurrentPlayingPoi;
        CurrentPlayingPoi = null;
        _logger.LogWarning("Lỗi audio: {Error}", error);

        if (poi is null) return;

        var result = PlaybackResult.Failure(poi.Id, error);
        MainThread.BeginInvokeOnMainThread(
            () => NarrationCompleted?.Invoke(this, result));
    }

    // ──────────────────────────────────────────
    // HELPERS
    // ──────────────────────────────────────────

    private TimeSpan CooldownRemaining(int poiId)
    {
        if (!_cooldowns.TryGetValue(poiId, out var last)) return TimeSpan.Zero;
        var elapsed   = DateTime.UtcNow - last;
        var remaining = TimeSpan.FromSeconds(CooldownSeconds) - elapsed;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
}
