using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly List<MauiApp1.Models.Poi> _pois = new();
    private CancellationTokenSource? _cts;
    private Location _lastLocation;

    public MapPage()
    {
        InitializeComponent();

        // 1) Khai báo POI mẫu
        _pois.Add(new Poi
        {
            Id = "poi_hcmut",
            Name = "ĐH BK",
            Description = "Cổng trường",
            Latitude = 10.772,
            Longitude = 106.657,
            RadiusMeters = 120,
            NearRadiusMeters = 220,
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
            NearRadiusMeters = 220,
            DebounceSeconds = 3,
            CooldownSeconds = 30
        });

        // 2) Vẽ marker POI lên map
        foreach (var p in _pois)
        {
            MyMap.Pins.Add(new Pin
            {
                Label = $"{p.Name} ({p.RadiusMeters}m)",
                Address = p.Description,
                Location = new Location(p.Latitude, p.Longitude)
            });
        }

        // 3) Nhận sự kiện geofence (ENTER/EXIT/DWELL)
        _geofence.OnPoiEvent += async (poi, type) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await DisplayAlertAsync("Geofence", $"{type}: {poi.Name}", "OK"));
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Quyền vị trí
        var when = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (when != PermissionStatus.Granted) return;

        // Nếu muốn chạy nền, xin thêm:
        _ = await Permissions.RequestAsync<Permissions.LocationAlways>();

        // Đăng ký geofence
        await _geofence.RegisterAsync(_pois);

        // Foreground: tính “NEAR”
        _cts = new CancellationTokenSource();
        _ = TrackLoopAsync(_cts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts?.Cancel();
        _cts = null;
    }

    async Task TrackLoopAsync(CancellationToken token)
    {
        var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

        while (!token.IsCancellationRequested)
        {
            try
            {
                var loc = await Geolocation.GetLocationAsync(req, token);
                if (loc != null)
                {
                    _lastLocation = loc;

                    // camera bám theo
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(loc, Distance.FromMeters(180)));

                    // ====== “ĐẾN GẦN” (NEAR) ======
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
                                await MainThread.InvokeOnMainThreadAsync(async () =>
                                    await DisplayAlertAsync("Near POI", $"Đến gần: {poi.Name} (~{distMeters:F0} m)", "OK"));
                            }
                        }
                    }
                }
            }
            catch
            {
                // timeout / tắt GPS -> bỏ qua
            }

            try { await Task.Delay(3000, token); } catch { }
        }
    }

    // (tùy chọn) nếu bạn cần xoay map theo hướng di chuyển, thêm lại logic tính bearing ở đây
}