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

    // Lấy từ DI container thay vì inject qua constructor parameter
    // Tránh lỗi XAML SourceGen không thấy NarrationEngine trong generated file
    private NarrationEngine? _narration;

    private readonly List<Poi> _pois = new();
    private CancellationTokenSource? _cts;
    private Location? _lastLocation;

    public MapPage(IGeofenceService geofence, ILocationService location)
    {
        InitializeComponent();

        _geofence = geofence ?? throw new ArgumentNullException(nameof(geofence));
        _location = location ?? throw new ArgumentNullException(nameof(location));

        // Lấy NarrationEngine từ DI container (đã đăng ký trong MauiProgram)
        _narration = IPlatformApplication.Current?.Services
            .GetService(typeof(NarrationEngine)) as NarrationEngine;

        BtnStart.Clicked += (_, _) => StartUiLoop();
        BtnStop.Clicked += (_, _) => StopUiLoop();

        SeedPoisAndPins();

        // Geofence event -> phát thuyết minh
        _geofence.OnPoiEvent += async (poi, type) =>
        {
            if (_narration is not null)
                await _narration.TriggerAsync(poi, type);
        };
    }

    // ── Seed dữ liệu POI test ────────────────────────────────────────────
    void SeedPoisAndPins()
    {
        _pois.Add(new Poi
        {
            Id = "poi_hcm",
            Name = "TP.HCM",
            Description = "Trung tam Thanh pho Ho Chi Minh",
            Latitude = 10.776889,
            Longitude = 106.700806,
            RadiusMeters = 150,
            NearRadiusMeters = 300,
            DebounceSeconds = 3,
            CooldownSeconds = 30,
            AudioUrl = null,   // null -> dung TTS fallback
            NarrationText = "Chao mung den Thanh pho Ho Chi Minh, trai tim kinh te Viet Nam."
        });

        _pois.Add(new Poi
        {
            Id = "poi_ntmk",
            Name = "NTMK Park",
            Description = "Cong vien Nguyen Thi Minh Khai",
            Latitude = 10.787,
            Longitude = 106.700,
            RadiusMeters = 120,
            NearRadiusMeters = 240,
            DebounceSeconds = 3,
            CooldownSeconds = 30,
            AudioUrl = null,
            NarrationText = "Ban dang den gan cong vien Nguyen Thi Minh Khai."
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

    // ── Lifecycle ────────────────────────────────────────────────────────
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var when = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (when != PermissionStatus.Granted) return;

        _ = await Permissions.RequestAsync<Permissions.LocationAlways>();

        await _geofence.RegisterAsync(_pois);
        StartUiLoop();
    }

    protected override void OnDisappearing()
    {
        StopUiLoop();
        base.OnDisappearing();
    }

    // ── GPS Loop ─────────────────────────────────────────────────────────
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
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(loc, Distance.FromMeters(180)));

                // Kiểm tra NEAR (giữa NearRadius và RadiusMeters)
                foreach (var poi in _pois)
                {
                    var distMeters = Location.CalculateDistance(
                        new Location(poi.Latitude, poi.Longitude),
                        loc,
                        DistanceUnits.Kilometers) * 1000.0;

                    if (distMeters <= poi.NearRadiusMeters && distMeters > poi.RadiusMeters)
                    {
                        if (GeofenceEventGate.ShouldAccept(poi.Id, "NEAR",
                            poi.DebounceSeconds, poi.CooldownSeconds))
                        {
                            if (_narration is not null)
                                await _narration.TriggerAsync(poi, "NEAR");
                        }
                    }
                }
            }
            catch { /* timeout/deny -> bo qua */ }

            try { await Task.Delay(5000, token); } catch { }
        }
    }
}