using Microsoft.Maui.Storage;

namespace MauiApp1.Services;

public sealed class GeofenceEventGate
{
    private const int CrossTypeSuppressSeconds = 12;
    private const int CrossPoiSuppressSeconds = 8;
    private const int SessionNarrationMemorySeconds = 180;

    private readonly object _gate = new();
    private readonly Dictionary<string, DateTimeOffset> _sessionAcceptedEvents = [];
    private readonly Dictionary<int, DateTimeOffset> _sessionNarrationMemory = [];
    private (int PoiId, string EventType, DateTimeOffset AcceptedAt)? _lastAcceptedAutoTrigger;

    public bool ShouldAccept(int poiId, string eventType, int debounceSec, int cooldownSec) =>
        Evaluate(poiId, eventType, debounceSec, cooldownSec).Accepted;

    public GeofenceGateDecision Evaluate(int poiId, string eventType, int debounceSec, int cooldownSec)
    {
        var normalizedEventType = NormalizeEventType(eventType);
        var now = DateTimeOffset.UtcNow;
        var perEventKey = BuildEventKey(poiId, normalizedEventType);
        debounceSec = Math.Max(0, debounceSec);
        cooldownSec = Math.Max(debounceSec, cooldownSec);

        lock (_gate)
        {
            CleanupExpiredStateLocked(now);

            if (TryRejectByPreferences(perEventKey, now, debounceSec, cooldownSec, out var reason))
            {
                LogDecision(poiId, normalizedEventType, accepted: false, reason);
                return new GeofenceGateDecision(false, reason);
            }

            if (TryRejectBySession(poiId, normalizedEventType, now, debounceSec, out reason))
            {
                LogDecision(poiId, normalizedEventType, accepted: false, reason);
                return new GeofenceGateDecision(false, reason);
            }

            if (TryRejectByCompetingPoi(poiId, normalizedEventType, now, out reason))
            {
                LogDecision(poiId, normalizedEventType, accepted: false, reason);
                return new GeofenceGateDecision(false, reason);
            }

            Preferences.Set(perEventKey, now.Ticks);
            _sessionAcceptedEvents[perEventKey] = now;
            if (IsAutoTrigger(normalizedEventType))
                _lastAcceptedAutoTrigger = (poiId, normalizedEventType, now);

            LogDecision(poiId, normalizedEventType, accepted: true, "accepted");
            return new GeofenceGateDecision(true, "accepted");
        }
    }

    public bool ShouldPlayAutoNarration(int poiId, int sessionMemorySeconds = SessionNarrationMemorySeconds)
    {
        var now = DateTimeOffset.UtcNow;
        lock (_gate)
        {
            CleanupExpiredStateLocked(now);

            if (_sessionNarrationMemory.TryGetValue(poiId, out var lastNarrationAt))
            {
                var ageSeconds = (now - lastNarrationAt).TotalSeconds;
                if (ageSeconds < sessionMemorySeconds)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GeofenceGate] Auto narration suppressed for poi={poiId}, age={ageSeconds:F0}s");
                    return false;
                }
            }

            return true;
        }
    }

    public void RecordNarration(int poiId)
    {
        lock (_gate)
        {
            _sessionNarrationMemory[poiId] = DateTimeOffset.UtcNow;
        }
    }

    public void ClearPoiSession(int poiId)
    {
        lock (_gate)
        {
            _sessionNarrationMemory.Remove(poiId);

            var keysToRemove = _sessionAcceptedEvents.Keys
                .Where(key => key.StartsWith($"geo_{poiId}_", StringComparison.Ordinal))
                .ToList();

            foreach (var key in keysToRemove)
                _sessionAcceptedEvents.Remove(key);

            if (_lastAcceptedAutoTrigger is { PoiId: var lastPoiId } && lastPoiId == poiId)
                _lastAcceptedAutoTrigger = null;
        }
    }

    private static string BuildEventKey(int poiId, string eventType) => $"geo_{poiId}_{eventType}_last";

    private static bool TryRejectByPreferences(
        string key,
        DateTimeOffset now,
        int debounceSec,
        int cooldownSec,
        out string reason)
    {
        var lastTicks = Preferences.Get(key, 0L);
        if (lastTicks == 0)
        {
            reason = string.Empty;
            return false;
        }

        var last = new DateTimeOffset(lastTicks, TimeSpan.Zero);
        var diff = (now - last).TotalSeconds;

        if (diff < debounceSec)
        {
            reason = $"debounce:{diff:F1}s";
            return true;
        }

        if (diff < cooldownSec)
        {
            reason = $"cooldown:{diff:F1}s";
            return true;
        }

        reason = string.Empty;
        return false;
    }

    private bool TryRejectBySession(
        int poiId,
        string eventType,
        DateTimeOffset now,
        int debounceSec,
        out string reason)
    {
        var sessionKey = BuildEventKey(poiId, eventType);
        if (_sessionAcceptedEvents.TryGetValue(sessionKey, out var lastAcceptedAt))
        {
            var diff = (now - lastAcceptedAt).TotalSeconds;
            if (diff < debounceSec)
            {
                reason = $"session-debounce:{diff:F1}s";
                return true;
            }
        }

        if (!IsAutoTrigger(eventType))
        {
            reason = string.Empty;
            return false;
        }

        var duplicateWindowSeconds = Math.Max(debounceSec, CrossTypeSuppressSeconds);
        var competingEvent = _sessionAcceptedEvents
            .Where(entry => entry.Key.StartsWith($"geo_{poiId}_", StringComparison.Ordinal))
            .Select(entry => new
            {
                EventType = ExtractEventType(entry.Key),
                AcceptedAt = entry.Value
            })
            .FirstOrDefault(entry =>
                !string.Equals(entry.EventType, eventType, StringComparison.OrdinalIgnoreCase) &&
                IsAutoTrigger(entry.EventType) &&
                (now - entry.AcceptedAt).TotalSeconds < duplicateWindowSeconds);

        if (competingEvent is not null)
        {
            reason = $"cross-type-suppressed:{competingEvent.EventType}";
            return true;
        }

        reason = string.Empty;
        return false;
    }

    private bool TryRejectByCompetingPoi(
        int poiId,
        string eventType,
        DateTimeOffset now,
        out string reason)
    {
        if (!IsAutoTrigger(eventType) || _lastAcceptedAutoTrigger is null)
        {
            reason = string.Empty;
            return false;
        }

        var competing = _lastAcceptedAutoTrigger.Value;
        if (competing.PoiId == poiId)
        {
            reason = string.Empty;
            return false;
        }

        if ((now - competing.AcceptedAt).TotalSeconds < CrossPoiSuppressSeconds)
        {
            reason = $"competing-poi-suppressed:{competing.PoiId}:{competing.EventType}";
            return true;
        }

        reason = string.Empty;
        return false;
    }

    private void CleanupExpiredStateLocked(DateTimeOffset now)
    {
        var expiredAcceptedKeys = _sessionAcceptedEvents
            .Where(entry => (now - entry.Value).TotalSeconds > SessionNarrationMemorySeconds)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var key in expiredAcceptedKeys)
            _sessionAcceptedEvents.Remove(key);

        var expiredNarrations = _sessionNarrationMemory
            .Where(entry => (now - entry.Value).TotalSeconds > SessionNarrationMemorySeconds)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var poiId in expiredNarrations)
            _sessionNarrationMemory.Remove(poiId);

        if (_lastAcceptedAutoTrigger is { AcceptedAt: var acceptedAt } &&
            (now - acceptedAt).TotalSeconds > SessionNarrationMemorySeconds)
        {
            _lastAcceptedAutoTrigger = null;
        }
    }

    private static bool IsAutoTrigger(string eventType) =>
        eventType is GeofenceEventTypes.Enter or GeofenceEventTypes.Near or GeofenceEventTypes.Dwell;

    private static string NormalizeEventType(string eventType) =>
        string.IsNullOrWhiteSpace(eventType)
            ? GeofenceEventTypes.Unknown
            : eventType.Trim().ToLowerInvariant();

    private static string ExtractEventType(string key)
    {
        const string prefix = "geo_";
        const string suffix = "_last";

        if (!key.StartsWith(prefix, StringComparison.Ordinal) || !key.EndsWith(suffix, StringComparison.Ordinal))
            return GeofenceEventTypes.Unknown;

        var body = key[prefix.Length..^suffix.Length];
        var separatorIndex = body.IndexOf('_');
        return separatorIndex >= 0 && separatorIndex < body.Length - 1
            ? body[(separatorIndex + 1)..]
            : GeofenceEventTypes.Unknown;
    }

    private static void LogDecision(int poiId, string eventType, bool accepted, string reason) =>
        System.Diagnostics.Debug.WriteLine(
            $"[GeofenceGate] poi={poiId}, event={eventType}, accepted={accepted}, reason={reason}");
}

public sealed record GeofenceGateDecision(bool Accepted, string Reason);
