using MauiApp1.Configuration;
using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Api;
using MauiApp1.Services.Map;
using MauiApp1.Services.Narration;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;

namespace MauiApp1.Services.Geofencing;

public sealed class GeofenceNarrationCoordinator
{
    private readonly object _gate = new();
    private readonly MobileAppOptions _options;
    private readonly NarrationManager _narrationManager;
    private readonly PoiNarrationApiClient _narrationApiClient;
    private readonly PoiNarrationCache _narrationCache;
    private readonly PlaybackApiClient _playbackApiClient;
    private readonly GeofenceEventGate _eventGate;

    private int? _activePoiId;
    private GeofenceFlowState _currentState = GeofenceFlowState.Idle;
    private DateTimeOffset? _manualPriorityUntilUtc;

    public GeofenceNarrationCoordinator(
        MobileAppOptions options,
        NarrationManager narrationManager,
        PoiNarrationApiClient narrationApiClient,
        PoiNarrationCache narrationCache,
        PlaybackApiClient playbackApiClient,
        GeofenceEventGate eventGate)
    {
        _options = options;
        _narrationManager = narrationManager;
        _narrationApiClient = narrationApiClient;
        _narrationCache = narrationCache;
        _playbackApiClient = playbackApiClient;
        _eventGate = eventGate;
    }

    public GeofenceFlowDecision EvaluateNearby(IEnumerable<Poi> pois, Location currentLocation)
    {
        lock (_gate)
        {
            NormalizeManualStateLocked();

            if (IsManualPriorityLocked())
            {
                return new GeofenceFlowDecision(
                    null,
                    GeofenceFlowState.Suppressed,
                    "Dang uu tien narration thu cong.",
                    WasSuppressed: true);
            }

            var result = PoiProximitySelector.FindNearestCandidate(pois, currentLocation);
            if (result.Poi is null)
            {
                if (_currentState == GeofenceFlowState.Near)
                {
                    _currentState = GeofenceFlowState.Idle;
                    _activePoiId = null;

                    return new GeofenceFlowDecision(
                        null,
                        GeofenceFlowState.Idle,
                        "Khong co POI gan ngay luc nay.",
                        HideCard: true);
                }

                return new GeofenceFlowDecision(null, _currentState, string.Empty, WasSuppressed: true);
            }

            if (_currentState == GeofenceFlowState.Entered || _currentState == GeofenceFlowState.Dwelling)
            {
                if (_activePoiId == result.Poi.Id)
                {
                    return new GeofenceFlowDecision(
                        result.Poi,
                        GeofenceFlowState.Suppressed,
                        "POI da duoc kich hoat geofence.",
                        ShowCard: true,
                        WasSuppressed: true,
                        DistanceMeters: result.DistanceMeters);
                }

                return new GeofenceFlowDecision(
                    null,
                    GeofenceFlowState.Suppressed,
                    "Dang giu uu tien cho POI vua enter/dwell.",
                    WasSuppressed: true);
            }

            var poiChanged = _activePoiId != result.Poi.Id || _currentState != GeofenceFlowState.Near;
            _activePoiId = result.Poi.Id;
            _currentState = GeofenceFlowState.Near;

            string? toastText = null;
            if (poiChanged &&
                _eventGate.ShouldAccept(
                    result.Poi.Id,
                    GeofenceEventTypes.Near,
                    result.Poi.DebounceSeconds + 4,
                    result.Poi.CooldownSeconds))
            {
                toastText = $"Ban dang den gan {result.Poi.Name}";
            }

            return new GeofenceFlowDecision(
                result.Poi,
                GeofenceFlowState.Near,
                $"Gan POI (~{result.DistanceMeters:F0}m)",
                ShowCard: true,
                ToastText: toastText,
                DistanceMeters: result.DistanceMeters);
        }
    }

    public async Task<GeofenceFlowDecision> HandleGeofenceTransitionAsync(
        Poi poi,
        string geofenceEventType,
        CancellationToken ct = default)
    {
        if (geofenceEventType == GeofenceEventTypes.Exit)
        {
            lock (_gate)
            {
                if (_activePoiId == poi.Id)
                {
                    _activePoiId = null;
                    _currentState = GeofenceFlowState.Idle;
                }
            }

            return new GeofenceFlowDecision(
                poi,
                GeofenceFlowState.Idle,
                "Da roi khoi vung POI.",
                HideCard: true);
        }

        var nextState = geofenceEventType == GeofenceEventTypes.Dwell
            ? GeofenceFlowState.Dwelling
            : GeofenceFlowState.Entered;

        var eventType = geofenceEventType == GeofenceEventTypes.Dwell
            ? PoiEventType.Dwell
            : PoiEventType.Enter;

        lock (_gate)
        {
            NormalizeManualStateLocked();

            if (IsManualPriorityLocked())
            {
                _activePoiId = poi.Id;

                return new GeofenceFlowDecision(
                    poi,
                    GeofenceFlowState.Suppressed,
                    "Narration tu dong duoc tam hoan vi nguoi dung dang phat thu cong.",
                    ShowCard: true,
                    WasSuppressed: true);
            }

            _activePoiId = poi.Id;
            _currentState = nextState;
        }

        var shouldPlay = _eventGate.ShouldPlayAutoNarration(
            poi.Id,
            sessionMemorySeconds: Math.Max(poi.CooldownSeconds * 2, _options.Narration.SessionMemorySeconds));

        _ = _playbackApiClient.LogVisitAsync(poi.Id, geofenceEventType, ct);

        if (!shouldPlay)
        {
            return new GeofenceFlowDecision(
                poi,
                nextState,
                nextState == GeofenceFlowState.Entered ? "Ban da vao vung POI" : "Ban dang o on dinh trong vung POI",
                ShowCard: true,
                WasSuppressed: true);
        }

        var started = await PlayNarrationAsync(
            poi,
            eventType,
            geofenceEventType,
            NarrationPlaybackPriority.Auto,
            ct);

        if (started)
            _eventGate.RecordNarration(poi.Id);

        return new GeofenceFlowDecision(
            poi,
            nextState,
            nextState == GeofenceFlowState.Entered ? "Ban da vao vung POI" : "Ban dang o on dinh trong vung POI",
            ShowCard: true,
            NarrationStarted: started,
            WasSuppressed: !started);
    }

    public async Task<bool> PlayManualAsync(Poi poi, CancellationToken ct = default)
    {
        lock (_gate)
        {
            _activePoiId = poi.Id;
            _currentState = GeofenceFlowState.ManuallyPlaying;
            _manualPriorityUntilUtc = DateTimeOffset.UtcNow.AddSeconds(_options.Narration.ManualPriorityWindowSeconds);
        }

        var started = await PlayNarrationAsync(
            poi,
            PoiEventType.Tap,
            GeofenceEventTypes.Tap,
            NarrationPlaybackPriority.Manual,
            ct);

        if (started)
            _eventGate.RecordNarration(poi.Id);

        return started;
    }

    public void Reset()
    {
        lock (_gate)
        {
            _activePoiId = null;
            _currentState = GeofenceFlowState.Idle;
            _manualPriorityUntilUtc = null;
        }
    }

    private async Task<bool> PlayNarrationAsync(
        Poi poi,
        PoiEventType eventType,
        string triggerType,
        NarrationPlaybackPriority playbackPriority,
        CancellationToken ct)
    {
        var language = LanguageService.Current;
        var narrationText = await GetNarrationTextAsync(poi.Id, eventType, language, ct);
        var started = await _narrationManager.HandleAsync(
            new Announcement(
                poi,
                eventType,
                DateTime.UtcNow,
                PreferredLanguage: language,
                PlaybackPriority: playbackPriority),
            overrideText: narrationText,
            ct: ct);

        if (started)
        {
            _ = _playbackApiClient.LogAsync(poi.Id, triggerType, ct: ct);
            System.Diagnostics.Debug.WriteLine(
                $"[GeoNarration] Started poi={poi.Id}, trigger={triggerType}, priority={playbackPriority}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine(
                $"[GeoNarration] Suppressed poi={poi.Id}, trigger={triggerType}, priority={playbackPriority}");
        }

        return started;
    }

    private async Task<string?> GetNarrationTextAsync(
        int poiId,
        PoiEventType eventType,
        string language,
        CancellationToken ct)
    {
        var eventByte = eventType switch
        {
            PoiEventType.Enter => (byte)0,
            PoiEventType.Near => (byte)1,
            PoiEventType.Tap => (byte)2,
            PoiEventType.Dwell => (byte)3,
            _ => (byte)0
        };

        var cached = await _narrationCache.GetAsync(poiId, eventByte, language);
        if (!string.IsNullOrWhiteSpace(cached))
            return cached;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return null;

        var dto = await _narrationApiClient.GetNarrationAsync(poiId, language, eventType.ToString(), ct);
        if (dto is null || string.IsNullOrWhiteSpace(dto.NarrationText))
            return null;

        await _narrationCache.UpsertAsync(poiId, dto.EventType, dto.Language, dto.NarrationText);
        return dto.NarrationText;
    }

    private bool IsManualPriorityLocked() =>
        _currentState == GeofenceFlowState.ManuallyPlaying &&
        _manualPriorityUntilUtc.HasValue &&
        _manualPriorityUntilUtc.Value > DateTimeOffset.UtcNow;

    private void NormalizeManualStateLocked()
    {
        if (!IsManualPriorityLocked() && _currentState == GeofenceFlowState.ManuallyPlaying)
        {
            _currentState = _activePoiId.HasValue ? GeofenceFlowState.Entered : GeofenceFlowState.Idle;
            _manualPriorityUntilUtc = null;
        }
    }
}
