using MauiApp1.Models;

namespace MauiApp1.Services.Geofencing;

public enum GeofenceFlowState
{
    Idle,
    Near,
    Entered,
    Dwelling,
    Suppressed,
    ManuallyPlaying
}

public sealed record GeofenceFlowDecision(
    Poi? Poi,
    GeofenceFlowState State,
    string StatusText,
    bool ShowCard = false,
    bool HideCard = false,
    bool WasSuppressed = false,
    bool NarrationStarted = false,
    string? ToastText = null,
    double? DistanceMeters = null);
