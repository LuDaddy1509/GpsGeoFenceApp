using System;
using System.Collections.Generic;
using System.Text;
#if ANDROID
using global::Android.Content;
using global::Android.Gms.Location;
using global::Android.OS;
using global::MauiApp1.Services;

namespace MauiApp1.Platforms.Android.Services;
public sealed class AndroidLocationService : ILocationService
{
    private global::Android.Gms.Location.IFusedLocationProviderClient? _client;
    private global::Android.Gms.Location.LocationCallback? _callback;
    private bool _isTracking;

    public AndroidLocationService()
    {
        _client = LocationServices.GetFusedLocationProviderClient(global::Android.App.Application.Context);
    }

    public void StartTracking(Action<double, double> onLocation)
    {
        if (_client == null) return;

        StopTracking();

        var request = new global::Android.Gms.Location.LocationRequest
            .Builder(global::Android.Gms.Location.Priority.PriorityBalancedPowerAccuracy, 5000) // 5s – Balanced
            .SetMinUpdateDistanceMeters(10)
            .Build();

        _callback = new CallbackImpl(onLocation);
        _client.RequestLocationUpdates(request, _callback, global::Android.OS.Looper.MainLooper);
        _isTracking = true;
        System.Diagnostics.Debug.WriteLine("[AndroidLocation] Tracking started");
    }

    public void StopTracking()
    {
        if (_client != null && _callback != null)
            _client.RemoveLocationUpdates(_callback);

        _callback = null;
        if (_isTracking)
            System.Diagnostics.Debug.WriteLine("[AndroidLocation] Tracking stopped");
        _isTracking = false;
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
