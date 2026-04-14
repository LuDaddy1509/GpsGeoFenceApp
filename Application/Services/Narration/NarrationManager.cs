using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Audio;
using Microsoft.Maui.Media;
using System.Text.RegularExpressions;

namespace MauiApp1.Services.Narration;

public enum PoiEventType
{
    Enter,
    Near,
    Dwell,
    Tap
}

public enum NarrationPlaybackPriority
{
    Auto,
    Manual
}

public sealed record Announcement(
    Poi Poi,
    PoiEventType EventType,
    DateTime CreatedAtUtc,
    string? PreferredLanguage = null,
    NarrationPlaybackPriority PlaybackPriority = NarrationPlaybackPriority.Auto)
{
    public string ResolvedLanguage =>
        PreferredLanguage
        ?? TryGetCurrentLanguage()
        ?? "vi-VN";

    private static string? TryGetCurrentLanguage()
    {
        try { return LanguageService.Current; }
        catch { return null; }
    }
}

public sealed class NarrationManager : INarrationManager
{
    private sealed record NarrationSession(
        int PoiId,
        PoiEventType EventType,
        string Language,
        NarrationPlaybackPriority PlaybackPriority,
        DateTimeOffset StartedAtUtc);

    private readonly IAudioPlayer _player;
    private readonly AudioCache _cache;

    private readonly object _gate = new();
    private CancellationTokenSource? _currentCts;
    private NarrationSession? _currentSession;

    public NarrationManager(IAudioPlayer player, AudioCache cache)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<bool> HandleAsync(Announcement ann, string? overrideText = null, CancellationToken ct = default)
    {
        CancellationTokenSource sessionCts;
        NarrationSession session;

        lock (_gate)
        {
            if (ShouldSuppressLocked(ann, out var reason))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Narration] Suppressed poi={ann.Poi.Id}, event={ann.EventType}, reason={reason}");
                return false;
            }

            if (ShouldInterruptLocked(ann))
                StopLocked();

            sessionCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            session = new NarrationSession(
                ann.Poi.Id,
                ann.EventType,
                ann.ResolvedLanguage,
                ann.PlaybackPriority,
                DateTimeOffset.UtcNow);

            _currentSession = session;
            _currentCts = sessionCts;
        }

        try
        {
            var token = sessionCts.Token;

            if (!string.IsNullOrWhiteSpace(ann.Poi.AudioUrl))
            {
                var localPath = await _cache.GetOrAddFromUrlAsync(ann.Poi.AudioUrl!, token);
                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[Narration] Playing cached audio for poi={ann.Poi.Id}, priority={ann.PlaybackPriority}");
                    await _player.PlayFileAsync(localPath!, token);
                    return true;
                }
            }

            var text = !string.IsNullOrWhiteSpace(overrideText)
                ? overrideText!
                : (!string.IsNullOrWhiteSpace(ann.Poi.NarrationText)
                    ? ann.Poi.NarrationText!
                    : ComposeFallbackText(ann));

            if (string.IsNullOrWhiteSpace(text))
                return false;

            var options = new SpeechOptions { Volume = 1.0f, Pitch = 1.0f };
            var locale = await FindLocaleAsync(ann.ResolvedLanguage, token);
            if (locale is not null)
                options.Locale = locale;

            System.Diagnostics.Debug.WriteLine(
                $"[Narration] Speaking via TTS for poi={ann.Poi.Id}, event={ann.EventType}, priority={ann.PlaybackPriority}");

            foreach (var part in SplitToParts(text))
            {
                token.ThrowIfCancellationRequested();
                await TextToSpeech.Default.SpeakAsync(part, options, token);
                await Task.Delay(300, token);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Narration] Error: {ex}");
            return false;
        }
        finally
        {
            CompleteSession(sessionCts);
        }
    }

    public void Stop()
    {
        lock (_gate)
        {
            StopLocked();
        }
    }

    private bool ShouldSuppressLocked(Announcement incoming, out string reason)
    {
        if (_currentSession is null)
        {
            reason = string.Empty;
            return false;
        }

        if (_currentSession.PlaybackPriority == NarrationPlaybackPriority.Manual &&
            incoming.PlaybackPriority == NarrationPlaybackPriority.Auto)
        {
            reason = "manual-playback-active";
            return true;
        }

        if (_currentSession.PlaybackPriority == NarrationPlaybackPriority.Auto &&
            incoming.PlaybackPriority == NarrationPlaybackPriority.Auto)
        {
            reason = "auto-playback-active";
            return true;
        }

        if (_currentSession.PoiId == incoming.Poi.Id &&
            _currentSession.EventType == incoming.EventType &&
            string.Equals(_currentSession.Language, incoming.ResolvedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            reason = "duplicate-session";
            return true;
        }

        reason = string.Empty;
        return false;
    }

    private bool ShouldInterruptLocked(Announcement incoming) =>
        _currentSession is not null &&
        incoming.PlaybackPriority == NarrationPlaybackPriority.Manual;

    private void StopLocked()
    {
        try { _currentCts?.Cancel(); } catch { }
        try { _player.Stop(); } catch { }

        _currentCts?.Dispose();
        _currentCts = null;
        _currentSession = null;
    }

    private void CompleteSession(CancellationTokenSource sessionCts)
    {
        lock (_gate)
        {
            if (!ReferenceEquals(_currentCts, sessionCts))
                return;

            try { _player.Stop(); } catch { }

            _currentCts?.Dispose();
            _currentCts = null;
            _currentSession = null;
        }
    }

    private static string ComposeFallbackText(Announcement ann)
    {
        var name = ann.Poi.Name?.Trim() ?? string.Empty;
        var desc = string.IsNullOrWhiteSpace(ann.Poi.Description) ? string.Empty : ann.Poi.Description.Trim();

        return ann.EventType switch
        {
            PoiEventType.Near => $"Ban sap den {name}.",
            PoiEventType.Enter => string.IsNullOrWhiteSpace(desc)
                ? $"Ban da den {name}."
                : $"Ban da den {name}. {desc}",
            PoiEventType.Dwell => string.IsNullOrWhiteSpace(desc)
                ? $"Ban dang o tai {name}."
                : $"Ban dang o tai {name}. {desc}",
            PoiEventType.Tap => string.IsNullOrWhiteSpace(desc)
                ? $"Thong tin chi tiet ve {name}."
                : $"{name}. {desc}",
            _ => name
        };
    }

    private static IEnumerable<string> SplitToParts(string text)
    {
        var lines = text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();

        if (lines.Count > 1)
            return lines;

        var parts = Regex.Split(text, @"(?<=[\.!\?。！？])\s+")
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();

        return parts.Count > 0 ? parts : new[] { text };
    }

    private static async Task<Locale?> FindLocaleAsync(string lang, CancellationToken ct)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var exact = locales.FirstOrDefault(l =>
                string.Equals(l.Language, lang, StringComparison.OrdinalIgnoreCase));
            if (exact is not null)
                return exact;

            var primary = lang.Split('-')[0];
            return locales.FirstOrDefault(l =>
                l.Language.StartsWith(primary, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
        }
    }
}
