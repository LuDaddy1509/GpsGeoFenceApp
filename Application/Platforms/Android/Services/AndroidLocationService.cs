using System;
using System.Collections.Generic;
using System.Text;
#if ANDROID
using global::Android.Content;
using global::Android.Gms.Location;
using global::Android.OS;
using global::MauiApp1.Services;
using Microsoft.Maui.Devices;

namespace MauiApp1.Platforms.Android.Services;
public sealed class AndroidLocationService : ILocationService
{
    private global::Android.Gms.Location.IFusedLocationProviderClient? _client;
    private global::Android.Gms.Location.LocationCallback? _callback;

    public AndroidLocationService()
    {
        _client = LocationServices.GetFusedLocationProviderClient(global::Android.App.Application.Context);
    }

    private static (long IntervalMs, int Priority) GetLocationParams()
    {
        var level = Battery.Default.ChargeLevel; // 0.0 → 1.0
        return level switch
        {
            < 0.20 => (15_000, global::Android.Gms.Location.Priority.PriorityLowPower),
            < 0.50 => (8_000,  global::Android.Gms.Location.Priority.PriorityBalancedPowerAccuracy),
            _      => (5_000,  global::Android.Gms.Location.Priority.PriorityHighAccuracy)
        };
    }

    public void StartTracking(Action<double, double> onLocation)
    {
        if (_client == null) return;

        var (intervalMs, priority) = GetLocationParams();

        var request = new global::Android.Gms.Location.LocationRequest
            .Builder(priority, intervalMs)
            .SetMinUpdateDistanceMeters(10)
            .Build();

        _callback = new CallbackImpl(onLocation);
        _client.RequestLocationUpdates(request, _callback, global::Android.OS.Looper.MainLooper);
    }

    public void StopTracking()
    {
        if (_client != null && _callback != null)
            _client.RemoveLocationUpdates(_callback);
    }

    private sealed class CallbackImpl : global::Android.Gms.Location.LocationCallback
    {
        private readonly Action<double, double> _onLocation;
        public CallbackImpl(Action<double, double> onLocation) => _onLocation = onLocation;

        public override void OnLocationResult(global::Android.Gms.Location.LocationResult? result)
        {
            var loc = result?.LastLocation;
            if (loc != null) _onLocation(loc.Latitude, loc.Longitude);
        }
    }
}
#endif