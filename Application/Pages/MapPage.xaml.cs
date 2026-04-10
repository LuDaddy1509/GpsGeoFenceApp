using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.Api;
using MauiApp1.Services.Narration;
using MauiApp1.Services.Sync;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MauiApp1.Pages;

public partial class MapPage : ContentPage
{
    private bool _isInitialized = false;

    private readonly IGeofenceService _geofence;
    private readonly ILocationService _location;
    private readonly PoiDatabase _db;
    private readonly NarrationManager _narration;
    private readonly PoiSyncService _poiSync;
    private readonly PlaybackApiClient _playback;
    private readonly PoiNarrationApiClient _narrationApi;
    private readonly PoiNarrationCache _narrationCache;
    private readonly TranslatorClient _translator;

    private string _currentLang = LanguageService.Current;

    private readonly List<Poi> _pois = new();
    private readonly Dictionary<string, Pin> _pinMap = new();

    private CancellationTokenSource? _cts;
    private Poi? _nearestPoi;
    private Location? _userLocation;

    private static readonly Location _hcmCenter = new(10.776889, 106.700806);

    // ── THÊM MỚI: Vòng tròn POI radius ──────────────────────────────
    // Dictionary lưu Circle cho từng POI (key = poi.Id)
    private readonly Dictionary<string, Circle> _radiusCircleMap = new();

    // ── THÊM MỚI: Lịch sử di chuyển (3 chấm) ────────────────────────
    // Tối đa 40 điểm gần nhất — mỗi điểm là 1 Pin nhỏ hình tròn
    private readonly Queue<Pin> _trailPins = new();
    private const int TrailMaxPoints = 40;      // số chấm tối đa hiển thị
    private const int TrailIntervalMeters = 8;  // chỉ ghi khi đi ít nhất 8m
    private Location? _lastTrailLocation;

    // BottomSheet
    double _sheetCollapsedOffset = -1;
    readonly double _sheetExpandedOffset = 0;
    double _sheetStartPanY = 0;
    bool _sheetReady = false;
    const double SheetPeekHeight = 140;

    public MapPage(
        IGeofenceService geofence,
        ILocationService location,
        PoiDatabase db,
        NarrationManager narration,
        PoiSyncService poiSync,
        PlaybackApiClient playback,
        PoiNarrationApiClient narrationApi,
        PoiNarrationCache narrationCache,
        TranslatorClient translator)
    {
        InitializeComponent();

        _geofence = geofence ?? throw new ArgumentNullException(nameof(geofence));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _narration = narration ?? throw new ArgumentNullException(nameof(narration));
        _poiSync = poiSync ?? throw new ArgumentNullException(nameof(poiSync));
        _playback = playback ?? throw new ArgumentNullException(nameof(playback));
        _narrationApi = narrationApi ?? throw new ArgumentNullException(nameof(narrationApi));
        _narrationCache = narrationCache ?? throw new ArgumentNullException(nameof(narrationCache));
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));

        // Toolbar
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Reset",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(() => MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(_hcmCenter, Distance.FromKilometers(3))))
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "QR",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async () => await Shell.Current.GoToAsync("qrscan"))
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sync",
            Order = ToolbarItemOrder.Secondary,
            Command = new Command(async () =>
            {
                await _poiSync.SyncOnceAsync();
                await ReloadPoisAsync();
                if (_pois.Count > 0)
                    await _geofence.RegisterAsync(_pois);
                else
                    System.Diagnostics.Debug.WriteLine("[Geofence] Skip register: no POIs after sync.");
            })
        });

        BtnOpenInMaps.Clicked += async (_, _) => await OpenMapsAsync(_nearestPoi);
        BottomSheet.SizeChanged += (_, _) => SetupBottomSheetOffsets();
        this.SizeChanged += (_, _) => SetupBottomSheetOffsets();

        _geofence.OnPoiEvent += OnGeofenceEvent;
    }

    // ── Geofence event — GIỮ NGUYÊN ─────────────────────────────────
    private async void OnGeofenceEvent(Poi poi, string type)
    {
        if (type == "EXIT")
        {
            await MainThread.InvokeOnMainThreadAsync(ClearHighlight);
            return;
        }

        var evType = type == "ENTER" ? PoiEventType.Enter : PoiEventType.Near;
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            HighlightPoi(poi, $"Vào vùng {type}");
            ShowDetail(poi);
        });

        var started = DateTime.UtcNow;
        var lang = LanguageService.Current;

        var narrationText = await GetNarrationTextAsync(poi.Id, evType, lang);
        var poiText = await GetTranslatedPoiTextAsync(poi, lang);

        var fullText = string.IsNullOrWhiteSpace(narrationText)
            ? poiText
            : $"{poiText}. {narrationText}";

        await _narration.HandleAsync(
            new Announcement(poi, evType, started, PreferredLanguage: lang),
            overrideText: fullText);

        var dur = (int)(DateTime.UtcNow - started).TotalSeconds;
        _ = _playback.LogAsync(poi.Id, type, dur > 0 ? dur : null);
    }

    // ── Language bar — GIỮ NGUYÊN ────────────────────────────────────
    private void RefreshLangBar()
    {
        var map = new Dictionary<string, Border>
        {
            ["vi-VN"] = BtnVi,
            ["en-US"] = BtnEn,
            ["ja-JP"] = BtnJa,
            ["ko-KR"] = BtnKo,
            ["de-DE"] = BtnDe,
        };
        foreach (var (code, btn) in map)
            btn.Background = new SolidColorBrush(code == _currentLang
                ? Color.FromArgb("#1976D2")
                : Color.FromArgb("#333333"));
    }

    private async void OnLangTapped(object? sender, TappedEventArgs e)
    {
        var code = e.Parameter as string;
        if (string.IsNullOrWhiteSpace(code)) return;
        _currentLang = code;
        LanguageService.Set(code);
        RefreshLangBar();
        try { _narration.Stop(); } catch { }
        await Task.CompletedTask;
    }

    // ── Lifecycle — GIỮ NGUYÊN ───────────────────────────────────────
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _currentLang = LanguageService.Current;
        RefreshLangBar();
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(_hcmCenter, Distance.FromKilometers(3)));
        if (!_isInitialized)
        {
            _isInitialized = true;
            _ = InitializeMapAsync();
        }
    }

    private async Task InitializeMapAsync()
    {
        try
        {
            await _db.InitAsync();
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                await _poiSync.SyncOnceAsync();

            await ReloadPoisAsync();

            if (!await EnsureLocationPermissionsWithTimeoutAsync())
            {
                System.Diagnostics.Debug.WriteLine("[Map] Location permission denied");
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() => MyMap.IsShowingUser = true);

            if (_pois.Count > 0)
            {
                try { await _geofence.RegisterAsync(_pois); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Geofence] Register error: {ex.Message}"); }
            }

            _poiSync.StartAutoSync(TimeSpan.FromMinutes(2));
            _ = Task.Run(MoveToRealLocationAsync);
            StartTracking();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MapInit] Fatal error: {ex}");
        }
    }

        return null; // fallback -> NarrationManager dùng Poi.NarrationText
    }
    // ====== Lifecycle ======
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _currentLang = LanguageService.Current;
        RefreshLangBar();

        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(_hcmCenter, Distance.FromKilometers(3)));

        // ✅ Luôn load POI trước để pins không phụ thuộc quyền location
        await ReloadPoisAsync();

        // Xin quyền location chỉ để show user + tracking
        if (!await EnsureLocationPermissionsAsync())
        {
            System.Diagnostics.Debug.WriteLine("[Map] Location permission denied. Show pins only.");
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() => MyMap.IsShowingUser = true);

        _ = Task.Run(MoveToRealLocationAsync);

        if (_pois.Count > 0)
            await _geofence.RegisterAsync(_pois);
        else
            System.Diagnostics.Debug.WriteLine("[Geofence] Skip register: no POIs.");

        StartTracking();
    }

    protected override void OnDisappearing()
    {
        _geofence.OnPoiEvent -= OnGeofenceEvent;
        StopTracking();
        _poiSync.StopAutoSync();
        base.OnDisappearing();
    }
    private async Task<bool> EnsureLocationPermissionsAsync()
    {
        // ✅ Chỉ xin WhenInUse để tránh cảnh báo "only one set of permissions at a time"
        var when = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return when == PermissionStatus.Granted;
    }

    // ====== Load POIs ======
    private async Task ReloadPoisAsync()
    {
        try
        {
            var pois = await _db.GetActivePoisAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _pois.Clear();
                _pinMap.Clear();
                MyMap.Pins.Clear();

                // Xoá vòng tròn cũ
                ClearAllRadiusCircles();

                foreach (var p in pois)
                {
                    _pois.Add(p);

                    // Pin marker — GIỮ NGUYÊN
                    var pin = new Pin
                    {
                        Label = p.Name,
                        Address = p.Description,
                        Location = new Location(p.Latitude, p.Longitude),
                        Type = PinType.Place
                    };

                    pin.MarkerClicked += async (_, e) =>
                    {
                        e.HideInfoWindow = false;
                        HighlightPoi(p, "Đã chọn");
                        ShowDetail(p);

                        var started = DateTime.UtcNow;
                        var lang = LanguageService.Current;

                        // ✅ HƯỚNG 2: lấy narration theo TAP + lang
                        var text = await GetNarrationTextAsync(p.Id, PoiEventType.Tap, lang);

                        await _narration.HandleAsync(
                            new Announcement(p, PoiEventType.Tap, started, PreferredLanguage: lang),
                            overrideText: fullText);

                        var dur = (int)(DateTime.UtcNow - started).TotalSeconds;
                        _ = _playback.LogAsync(p.Id, "TAP", dur > 0 ? dur : null);
                    };

                    _pinMap[p.Id] = pin;
                    MyMap.Pins.Add(pin);

                    // ── THÊM MỚI: Vẽ vòng tròn bán kính kích hoạt quanh POI ──
                    DrawPoiRadiusCircle(p);
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Map] Reload lỗi: {ex.Message}");
        }
    }

    // ── THÊM MỚI: Vẽ Circle cho 1 POI ──────────────────────────────
    // Vòng tròn xanh nhạt = NearRadius (vùng bắt đầu chuẩn bị thuyết minh)
    // Vòng tròn xanh đậm  = RadiusMeters (vùng kích hoạt chính thức)
    private void DrawPoiRadiusCircle(Poi poi)
    {
        var center = new Location(poi.Latitude, poi.Longitude);

        // Vòng ngoài: NearRadius — màu xanh nhạt, trong suốt
        var outerCircle = new Circle
        {
            Center = center,
            Radius = Distance.FromMeters(poi.NearRadiusMeters),
            StrokeColor = Color.FromArgb("#661976D2"),   // xanh 40% opacity
            StrokeWidth = 1.5f,
            FillColor = Color.FromArgb("#141976D2"),   // xanh 8% opacity
        };

        // Vòng trong: RadiusMeters — màu xanh đậm hơn
        var innerCircle = new Circle
        {
            Center = center,
            Radius = Distance.FromMeters(poi.RadiusMeters),
            StrokeColor = Color.FromArgb("#CC1976D2"),   // xanh 80% opacity
            StrokeWidth = 2f,
            FillColor = Color.FromArgb("#281976D2"),   // xanh 16% opacity
        };

        MyMap.MapElements.Add(outerCircle);
        MyMap.MapElements.Add(innerCircle);

        // Lưu để xoá khi reload
        _radiusCircleMap[$"{poi.Id}_outer"] = outerCircle;
        _radiusCircleMap[$"{poi.Id}_inner"] = innerCircle;
    }

    // Highlight POI đang active: tô đậm vòng tròn
    private void HighlightPoiCircle(Poi poi, bool active)
    {
        if (_radiusCircleMap.TryGetValue($"{poi.Id}_inner", out var inner))
        {
            inner.StrokeColor = active
                ? Color.FromArgb("#FF4CAF50")    // xanh lá đậm khi đang trong vùng
                : Color.FromArgb("#CC1976D2");   // xanh dương bình thường
            inner.FillColor = active
                ? Color.FromArgb("#334CAF50")
                : Color.FromArgb("#281976D2");
        }
        if (_radiusCircleMap.TryGetValue($"{poi.Id}_outer", out var outer))
        {
            outer.StrokeColor = active
                ? Color.FromArgb("#994CAF50")
                : Color.FromArgb("#661976D2");
        }
    }

    private void ClearAllRadiusCircles()
    {
        foreach (var circle in _radiusCircleMap.Values)
            MyMap.MapElements.Remove(circle);
        _radiusCircleMap.Clear();
    }

    // ── GPS ── GIỮ NGUYÊN ────────────────────────────────────────────
    private async Task MoveToRealLocationAsync()
    {
        try
        {
            var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var loc = await Geolocation.GetLocationAsync(req);
            if (loc == null) return;
            _userLocation = loc;
            await MainThread.InvokeOnMainThreadAsync(() =>
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(loc, Distance.FromKilometers(1))));
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[GPS] {ex.Message}"); }
    }

    private void OnSharedLocationChanged(double lat, double lng)
        => _userLocation = new Location(lat, lng);

    private void StartTracking()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = TrackLoopAsync(_cts.Token);
        // Subscribe to location updates bridged from BackgroundLocationService
        SharedLocationState.LocationChanged += OnSharedLocationChanged;
    }

    private void StopTracking()
    {
        _cts?.Cancel();
        _cts = null;
        SharedLocationState.LocationChanged -= OnSharedLocationChanged;
    }

    // ── TrackLoop — THÊM: ghi trail + highlight vòng tròn ───────────
    private async Task TrackLoopAsync(CancellationToken token)
    {
        var req = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

        while (!token.IsCancellationRequested)
        {
            try
            {
                var loc = await Geolocation.GetLocationAsync(req, token);
                if (loc == null) { await Task.Delay(5000, token); continue; }

                _userLocation = loc;

                // ── THÊM MỚI: Cập nhật dấu 3 chấm lịch sử di chuyển ──
                await MainThread.InvokeOnMainThreadAsync(() => AddTrailDot(loc));

                // Tìm POI gần nhất — GIỮ NGUYÊN logic
                Poi? nearest = null;
                double nearestDist = double.MaxValue;

                foreach (var poi in _pois.ToList())
                {
                    var dist = Location.CalculateDistance(
                        new Location(poi.Latitude, poi.Longitude),
                        loc, DistanceUnits.Kilometers) * 1000.0;

                    if (dist > poi.NearRadiusMeters) continue;

                    var thisPri = poi.Priority ?? 999;
                    var bestPri = nearest?.Priority ?? 999;

                    if (nearest == null || thisPri < bestPri || (thisPri == bestPri && dist < nearestDist))
                    {
                        nearest = poi;
                        nearestDist = dist;
                    }
                }

                if (nearest != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        HighlightPoi(nearest, $"Đến gần (~{nearestDist:F0}m) • Ưu tiên #{nearest.Priority}");
                        ShowDetail(nearest);
                        // ── THÊM MỚI: tô màu vòng tròn POI đang trong vùng ──
                        HighlightPoiCircle(nearest, active: true);
                    });

                    if (GeofenceEventGate.ShouldAccept(nearest.Id, "NEAR",
                            nearest.DebounceSeconds, nearest.CooldownSeconds))
                    {
                        var started = DateTime.UtcNow;
                        var lang = LanguageService.Current;
                        var text = await GetNarrationTextAsync(nearest.Id, PoiEventType.Near, lang, token);

                        var narrationText = await GetNarrationTextAsync(nearest.Id, PoiEventType.Near, lang, token);
                        var poiText = await GetTranslatedPoiTextAsync(nearest, lang, token);
                        var fullText = string.IsNullOrWhiteSpace(narrationText)
                            ? poiText : $"{poiText}. {narrationText}";

                        await _narration.HandleAsync(
                            new Announcement(nearest, PoiEventType.Near, started, PreferredLanguage: lang),
                            overrideText: fullText,
                            ct: token);

                        var dur = (int)(DateTime.UtcNow - started).TotalSeconds;
                        _ = _playback.LogAsync(nearest.Id, "NEAR", dur > 0 ? dur : null);
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        // Reset màu tất cả vòng tròn khi ra khỏi vùng
                        if (_nearestPoi != null)
                            HighlightPoiCircle(_nearestPoi, active: false);
                        ClearHighlight();
                    });
                }
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine($"[Loop] {ex.Message}");
            }

            try { await Task.Delay(5000, token); } catch { }
        }
    }

    // ── THÊM MỚI: Vẽ chấm tròn nhỏ theo lịch sử di chuyển ─────────
    // Dùng Pin với type Generic — chấm nhỏ mờ dần theo thứ tự
    private void AddTrailDot(Location loc)
    {
        // Chỉ vẽ khi đi đủ xa điểm trước (tránh chấm chồng chất khi đứng yên)
        if (_lastTrailLocation != null)
        {
            var moved = Location.CalculateDistance(
                _lastTrailLocation, loc, DistanceUnits.Kilometers) * 1000.0;
            if (moved < TrailIntervalMeters) return;
        }
        _lastTrailLocation = loc;

        // Tạo 1 chấm nhỏ dạng Pin với label rỗng
        var dot = new Pin
        {
            Label = "·",          // chấm nhỏ, không hiển thị callout
            Location = loc,
            Type = PinType.Generic,
        };

        MyMap.Pins.Add(dot);
        _trailPins.Enqueue(dot);

        // Xoá chấm cũ nhất khi vượt quá giới hạn
        if (_trailPins.Count > TrailMaxPoints)
        {
            var oldest = _trailPins.Dequeue();
            MyMap.Pins.Remove(oldest);
        }
    }

    // Xoá toàn bộ trail khi reload POI / thoát trang
    private void ClearTrail()
    {
        foreach (var dot in _trailPins)
            MyMap.Pins.Remove(dot);
        _trailPins.Clear();
        _lastTrailLocation = null;
    }

    // ── Highlight — THÊM: reset vòng tròn khi clear ──────────────────
    private void HighlightPoi(Poi poi, string status)
    {
        // Reset màu POI trước đó nếu có
        if (_nearestPoi != null && _nearestPoi.Id != poi.Id)
            HighlightPoiCircle(_nearestPoi, active: false);

        ClearHighlight();
        _nearestPoi = poi;

        if (_pinMap.TryGetValue(poi.Id, out var pin))
            pin.Label = $"★ {poi.Name}";
    }

    private void ClearHighlight()
    {
        if (_nearestPoi == null) return;
        if (_pinMap.TryGetValue(_nearestPoi.Id, out var pin))
            pin.Label = _nearestPoi.Name;
        _nearestPoi = null;
    }

    // ── Bottom sheet — GIỮ NGUYÊN HOÀN TOÀN ─────────────────────────
    private void ShowDetail(Poi? poi)
    {
        if (poi == null) return;

        DetailName.Text = poi.Name;
        DetailDesc.Text = string.IsNullOrWhiteSpace(poi.Description)
            ? "(Không có mô tả)" : poi.Description;
        DetailCoord.Text = $"📍 {poi.Latitude:F6}, {poi.Longitude:F6}";
        DetailRadius.Text = $"🔵 Bán kính: {poi.RadiusMeters}m | Gần: {poi.NearRadiusMeters}m";
        DetailImage.Source = !string.IsNullOrWhiteSpace(poi.ImageUrl) ? poi.ImageUrl : null;

        var link = !string.IsNullOrWhiteSpace(poi.MapLink)
            ? poi.MapLink
            : $"https://maps.google.com/?q={poi.Latitude},{poi.Longitude}";
        LblPoiLink.Text = link;

        _ = ExpandSheetAsync();

        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
            new Location(poi.Latitude, poi.Longitude), Distance.FromMeters(400)));
    }

    private async Task OpenMapsAsync(Poi? poi)
    {
        if (poi == null) return;
        var url = !string.IsNullOrWhiteSpace(poi.MapLink)
            ? poi.MapLink
            : $"https://maps.google.com/?q={poi.Latitude},{poi.Longitude}";
        try { await Launcher.OpenAsync(new Uri(url)); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Maps] {ex.Message}"); }
    }

    void SetupBottomSheetOffsets()
    {
        if (BottomSheet.Height <= 0 || this.Height <= 0) return;
        _sheetCollapsedOffset = Math.Max(0, BottomSheet.Height - SheetPeekHeight);
        if (!_sheetReady)
        {
            BottomSheet.TranslationY = _sheetCollapsedOffset;
            _sheetReady = true;
        }
    }

    Task ExpandSheetAsync() =>
        BottomSheet.TranslateToAsync(0, _sheetExpandedOffset, 180, Easing.CubicOut);

    Task CollapseSheetAsync() =>
        BottomSheet.TranslateToAsync(0, _sheetCollapsedOffset, 180, Easing.CubicOut);

    void BottomSheet_TapHeader(object? sender, EventArgs e)
    {
        if (!_sheetReady) return;
        var isCollapsed = Math.Abs(BottomSheet.TranslationY - _sheetCollapsedOffset) < 1;
        _ = BottomSheet.TranslateToAsync(0,
            isCollapsed ? _sheetExpandedOffset : _sheetCollapsedOffset,
            200, Easing.CubicOut);
    }

    void BottomSheet_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (!_sheetReady) return;
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _sheetStartPanY = BottomSheet.TranslationY;
                break;
            case GestureStatus.Running:
                var newY = Math.Clamp(_sheetStartPanY + e.TotalY,
                    _sheetExpandedOffset, _sheetCollapsedOffset);
                BottomSheet.TranslationY = newY;
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                var half = (_sheetCollapsedOffset - _sheetExpandedOffset) / 2.0;
                var curr = BottomSheet.TranslationY - _sheetExpandedOffset;
                _ = BottomSheet.TranslateToAsync(0,
                    curr <= half ? _sheetExpandedOffset : _sheetCollapsedOffset,
                    180, Easing.CubicOut);
                break;
        }
    }
}