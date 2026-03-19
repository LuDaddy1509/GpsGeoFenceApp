using MauiApp1.Models;
using MauiApp1.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
namespace MauiApp1.Pages;

public partial class MapPage : ContentPage
{
    private readonly IGeofenceService _geofence;
    private readonly ILocationService _location;

    private readonly List<Poi> _pois = new();
    private CancellationTokenSource? _cts;
    private Location? _lastLocation;

    public MapPage(IGeofenceService geofence, ILocationService location)
    {
        InitializeComponent(); // ✅ sẽ có vì x:Class khớp namespace

        _geofence = geofence ?? throw new ArgumentNullException(nameof(geofence));
        _location = location ?? throw new ArgumentNullException(nameof(location));

        BtnStart.Clicked += (_, __) => StartUiLoop();
        BtnStop.Clicked += (_, __) => StopUiLoop();

        SeedPoisAndPins();

        _geofence.OnPoiEvent += async (poi, type) =>
            await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlertAsync("Geofence", $"{type}: {poi.Name}", "OK"));
    }

    void SeedPoisAndPins()
    {
        _pois.Add(new Poi
        {
            Id = "poi_hcm",
            Name = "TP.HCM",
            Description = "Trung tâm",
            Latitude = 10.776889,
            Longitude = 106.700806,
            RadiusMeters = 150,
            NearRadiusMeters = 300,
            DebounceSeconds = 3,
            CooldownSeconds = 30
        });

        _pois.Add(new Poi
        {
            Id = "poi_ntmk",
            Name = "NTMK Park",
            Description = "Công viên",
            Latitude = 10.787,
            Longitude = 106.700,
            RadiusMeters = 120,
            NearRadiusMeters = 240,
            DebounceSeconds = 3,
            CooldownSeconds = 30
        });

        foreach (var p in _pois)
        {
            MyMap.Pins.Add(new Pin
            {
                Label = $"{p.Name} ({p.RadiusMeters}m)",
                Address = p.Description,
                Location = new Location(p.Latitude, p.Longitude)
            });
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var when = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (when != PermissionStatus.Granted) return;

        _ = await Permissions.RequestAsync<Permissions.LocationAlways>(); // nền

        await _geofence.RegisterAsync(_pois);
        StartUiLoop();
    }

    protected override void OnDisappearing()
    {
        StopUiLoop();
        base.OnDisappearing();
    }

    void StartUiLoop()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = TrackLoopAsync(_cts.Token);

        _location.StartTracking((lat, lng) =>
        {
            _lastLocation = new Location(lat, lng);
        });
    }

    void StopUiLoop()
    {
        _cts?.Cancel();
        _cts = null;
        _location.StopTracking();
    }

    async Task TrackLoopAsync(CancellationToken token)
    {
        var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

        while (!token.IsCancellationRequested)
        {
            try
            {
                var loc = await Geolocation.GetLocationAsync(req, token);
                if (loc == null) { await Task.Delay(5000, token); continue; }

                _lastLocation = loc;

                // Camera follow nhẹ
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(loc, Distance.FromMeters(180)));

                // Tính “đến gần” NEAR
                foreach (var poi in _pois)
                {
                    var distMeters = Location.CalculateDistance(
                        new Location(poi.Latitude, poi.Longitude),
                        loc,
                        DistanceUnits.Kilometers) * 1000.0;

                    if (distMeters <= poi.NearRadiusMeters && distMeters > poi.RadiusMeters)
                    {
                        if (GeofenceEventGate.ShouldAccept(poi.Id, "NEAR", poi.DebounceSeconds, poi.CooldownSeconds))
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                                DisplayAlertAsync("Near POI", $"Đến gần: {poi.Name} (~{distMeters:F0} m)", "OK"));
                        }
                    }
                }
            }
            catch { /* timeout/deny -> bỏ qua lần này */ }

            try { await Task.Delay(5000, token); } catch { }
        }
    }
}