using MauiApp1.Configuration;
using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Services.AppState;
using MauiApp1.Services.Geofencing;
using MauiApp1.Services.Map;
using MauiApp1.Services.Navigation;
using MauiApp1.Services.Sync;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Networking;
using System.Linq;

namespace MauiApp1.Pages;

public partial class MapPage : ContentPage
{
    private static readonly Location HcmCenter = new(10.776889, 106.700806);

    private readonly SemaphoreSlim _runtimeLock = new(1, 1);
    private readonly object _poiStateLock = new();

    private readonly IGeofenceService _geofence;
    private readonly ILocationService _location;
    private readonly MapRuntimeService _runtimeService;
    private readonly SyncStatusService _syncStatusService;
    private readonly PermissionStatusService _permissionStatusService;
    private readonly PoiNavigationService _poiNavigationService;
    private readonly GeofenceNarrationCoordinator _geofenceCoordinator;
    private readonly MobileAppOptions _options;

    private readonly List<Poi> _pois = [];
    private readonly Dictionary<int, Pin> _pinMap = [];

    private bool _isInitialized;
    private bool _isVisible;
    private bool _isRuntimeBusy;
    private CancellationTokenSource? _cts;
    private Poi? _activePoi;
    private Window? _lifecycleWindow;

    public MapPage(
        IGeofenceService geofence,
        ILocationService location,
        MapRuntimeService runtimeService,
        SyncStatusService syncStatusService,
        PermissionStatusService permissionStatusService,
        PoiNavigationService poiNavigationService,
        GeofenceNarrationCoordinator geofenceCoordinator,
        MobileAppOptions options)
    {
        InitializeComponent();
        _geofence = geofence;
        _location = location;
        _runtimeService = runtimeService;
        _syncStatusService = syncStatusService;
        _permissionStatusService = permissionStatusService;
        _poiNavigationService = poiNavigationService;
        _geofenceCoordinator = geofenceCoordinator;
        _options = options;

        ConfigureToolbar();
        MapStateLabel.Text = "Dang chuan bi ban do...";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isVisible = true;
        SubscribeRuntimeEvents();
        AttachWindowLifecycle();
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(HcmCenter, Distance.FromKilometers(3)));
        _ = RefreshStatusAsync();

        if (_isInitialized)
        {
            _ = ResumeRuntimeAsync();
            return;
        }

        _isInitialized = true;
        _ = InitializeMapAsync();
    }

    protected override void OnDisappearing()
    {
        _isVisible = false;
        UnsubscribeRuntimeEvents();
        DetachWindowLifecycle();
        StopTracking();
        _runtimeService.StopAutoSync();
        base.OnDisappearing();
    }

    private void ConfigureToolbar()
    {
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "QR",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async () => await OpenQrScannerAsync())
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Cai dat",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async () => await Shell.Current.GoToAsync(AppRoutes.Settings))
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sync",
            Order = ToolbarItemOrder.Secondary,
            Command = new Command(async () => await ForceSyncAsync())
        });
    }

    private void SubscribeRuntimeEvents()
    {
        _geofence.OnPoiEvent -= OnGeofenceEvent;
        _geofence.OnPoiEvent += OnGeofenceEvent;
        _runtimeService.SyncCompleted -= OnSyncCompleted;
        _runtimeService.SyncCompleted += OnSyncCompleted;
    }

    private void UnsubscribeRuntimeEvents()
    {
        _geofence.OnPoiEvent -= OnGeofenceEvent;
        _runtimeService.SyncCompleted -= OnSyncCompleted;
    }

    private void AttachWindowLifecycle()
    {
        var window = Window ?? Application.Current?.Windows.FirstOrDefault();
        if (window is null || ReferenceEquals(window, _lifecycleWindow))
            return;

        DetachWindowLifecycle();

        _lifecycleWindow = window;
        _lifecycleWindow.Resumed += OnWindowResumed;
    }

    private void DetachWindowLifecycle()
    {
        if (_lifecycleWindow is null)
            return;

        _lifecycleWindow.Resumed -= OnWindowResumed;
        _lifecycleWindow = null;
    }

    private void OnWindowResumed(object? sender, EventArgs e)
    {
        if (_isVisible)
            _ = ResumeRuntimeAsync();
    }

    private async Task InitializeMapAsync()
    {
        if (!await TryEnterRuntimeAsync())
            return;

        try
        {
            MapStateLabel.Text = "Dang khoi tao du lieu cuc bo...";
            await _runtimeService.InitializeLocalDataAsync();

            await ReloadPoisAsync();
            await RefreshStatusAsync();
            await ResumeRuntimeAsync(useInitialTrigger: _options.Geofence.RegisterInitialTriggerOnEnter);
        }
        catch (Exception ex)
        {
            MapStateLabel.Text = "Khong the khoi tao ban do.";
            System.Diagnostics.Debug.WriteLine($"[MapInit] {ex}");
        }
        finally
        {
            ExitRuntime();
        }
    }

    private async Task ResumeRuntimeAsync(bool useInitialTrigger = false)
    {
        if (!await TryEnterRuntimeAsync())
            return;

        try
        {
            await ReloadPoisAsync();
            await RefreshStatusAsync();

            if (!await EnsureLocationPermissionsWithTimeoutAsync())
            {
                MapStateLabel.Text = "Can quyen vi tri de kich hoat geofence.";
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() => MyMap.IsShowingUser = true);
            await RegisterGeofencesAsync(initialTriggerOnEnter: useInitialTrigger);

            _runtimeService.StartAutoSync();
            StartTracking();
            MapStateLabel.Text = GetPoiSnapshot().Count > 0
                ? "San sang theo doi POI theo vi tri."
                : "Chua co POI cuc bo. Hay dong bo du lieu.";
        }
        finally
        {
            ExitRuntime();
        }
    }

    private async Task ForceSyncAsync()
    {
        if (!await TryEnterRuntimeAsync())
            return;

        try
        {
            MapStateLabel.Text = "Dang dong bo du lieu...";
            await _runtimeService.ForceSyncAsync();
            await RefreshStatusAsync();
            MapStateLabel.Text = "Dong bo hoan tat.";
        }
        finally
        {
            ExitRuntime();
        }
    }

    private async Task RefreshStatusAsync()
    {
        var sync = await _syncStatusService.GetStatusAsync();
        SyncStatusLabel.Text = sync.StatusText;
        PermissionStatusLabel.Text = await _permissionStatusService.GetLocationStatusTextAsync();
        LanguageStatusLabel.Text = $"Ngon ngu: {LanguageService.Display(LanguageService.Current)}";
    }

    private async Task ReloadPoisAsync()
    {
        var pois = await _runtimeService.LoadActivePoisAsync();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            lock (_poiStateLock)
            {
                _pois.Clear();
                _pois.AddRange(pois);
            }

            _pinMap.Clear();
            MyMap.Pins.Clear();

            foreach (var poi in pois)
            {
                var pin = new Pin
                {
                    Label = poi.Name,
                    Address = poi.Description,
                    Location = new Location(poi.Latitude, poi.Longitude),
                    Type = PinType.Place
                };

                pin.MarkerClicked += (_, e) =>
                {
                    e.HideInfoWindow = false;
                    ActivatePoi(poi, "POI dang chon tren ban do");
                };

                _pinMap[poi.Id] = pin;
                MyMap.Pins.Add(pin);
            }

            EmptyStatePanel.IsVisible = _pois.Count == 0;
        });
    }

    private async Task RegisterGeofencesAsync(bool? initialTriggerOnEnter = null)
    {
        var poiSnapshot = GetPoiSnapshot();
        if (poiSnapshot.Count == 0)
            return;

        await _runtimeService.RegisterGeofencesAsync(poiSnapshot, initialTriggerOnEnter);
    }

    private async Task<bool> EnsureLocationPermissionsWithTimeoutAsync()
    {
        try
        {
            var task = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            var result = await task.WaitAsync(TimeSpan.FromSeconds(_options.Location.PermissionTimeoutSeconds));
            await RefreshStatusAsync();
            return result == PermissionStatus.Granted;
        }
        catch
        {
            await RefreshStatusAsync();
            return false;
        }
    }

    private void StartTracking()
    {
        var previousCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        try { previousCts?.Cancel(); } catch { }
        previousCts?.Dispose();

        var currentCts = _cts;
        if (currentCts is null)
            return;

        _ = TrackLoopAsync(currentCts.Token);
        _location.StartTracking((_, _) => { });
    }

    private void StopTracking()
    {
        var currentCts = Interlocked.Exchange(ref _cts, null);
        try { currentCts?.Cancel(); } catch { }
        currentCts?.Dispose();
        _location.StopTracking();
    }

    private async Task TrackLoopAsync(CancellationToken token)
    {
        var request = new GeolocationRequest(
            GeolocationAccuracy.Medium,
            TimeSpan.FromSeconds(_options.Location.InitialGpsTimeoutSeconds));

        while (!token.IsCancellationRequested)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(request, token);
                if (location is not null)
                {
                    var decision = _geofenceCoordinator.EvaluateNearby(GetPoiSnapshot(), location);
                    await ApplyDecisionAsync(decision);
                }
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine($"[TrackLoop] {ex.Message}");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.Location.TrackingLoopDelaySeconds), token);
            }
            catch
            {
            }
        }
    }

    private async void OnGeofenceEvent(Poi poi, string type)
    {
        if (!_isVisible)
            return;

        try
        {
            var decision = await _geofenceCoordinator.HandleGeofenceTransitionAsync(poi, type);
            await ApplyDecisionAsync(decision);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MapPage] Geofence event error: {ex.Message}");
        }
    }

    private async Task ApplyDecisionAsync(GeofenceFlowDecision decision)
    {
        if (!_isVisible)
            return;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (!_isVisible)
                return;

            if (decision.HideCard)
            {
                HidePoiCard();
                if (!string.IsNullOrWhiteSpace(decision.StatusText))
                    MapStateLabel.Text = decision.StatusText;
            }
            else if (decision.Poi is not null && decision.ShowCard)
            {
                ActivatePoi(decision.Poi, decision.StatusText);
            }
            else if (!string.IsNullOrWhiteSpace(decision.StatusText) && !decision.WasSuppressed)
            {
                MapStateLabel.Text = decision.StatusText;
            }

            if (!string.IsNullOrWhiteSpace(decision.ToastText))
                await AppShell.DisplayToastAsync(decision.ToastText);
        });
    }

    private void ActivatePoi(Poi poi, string stateText)
    {
        if (_activePoi is not null &&
            _activePoi.Id != poi.Id &&
            _pinMap.TryGetValue(_activePoi.Id, out var previousPin))
        {
            previousPin.Label = _activePoi.Name;
        }

        _activePoi = poi;
        CurrentPoiCard.IsVisible = true;
        CurrentPoiTitleLabel.Text = poi.Name;
        CurrentPoiSummaryLabel.Text = string.IsNullOrWhiteSpace(poi.Description) ? "Chua co mo ta." : poi.Description;
        CurrentPoiStatusLabel.Text = stateText;
        MapStateLabel.Text = "Chon Xem chi tiet de nghe day du va thao tac sau.";

        if (_pinMap.TryGetValue(poi.Id, out var pin))
            pin.Label = $"* {poi.Name}";
    }

    private void HidePoiCard()
    {
        if (_activePoi is not null && _pinMap.TryGetValue(_activePoi.Id, out var pin))
            pin.Label = _activePoi.Name;

        _activePoi = null;
        CurrentPoiCard.IsVisible = false;
    }

    private async Task OpenQrScannerAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
        {
            await Shell.Current.GoToAsync(AppRoutes.QrScan);
            return;
        }

        await DisplayAlert("Quyen camera", "Ban can cap quyen camera de quet QR.", "OK");
    }

    private async void OnOpenDetailClicked(object sender, EventArgs e)
    {
        if (_activePoi is null)
            return;

        await _poiNavigationService.OpenDetailAsync(_activePoi);
    }

    private async void OnOpenInMapsClicked(object sender, EventArgs e)
    {
        if (_activePoi is null)
            return;

        await Launcher.OpenAsync(new Uri(PoiMapLinkBuilder.BuildDetailLink(_activePoi)));
    }

    private async void OnSyncCompleted(int savedCount)
    {
        if (!_isVisible)
            return;

        if (!await TryEnterRuntimeAsync())
            return;

        try
        {
            await ReloadPoisAsync();
            await RegisterGeofencesAsync(initialTriggerOnEnter: false);
            await RefreshStatusAsync();
            MapStateLabel.Text = savedCount > 0
                ? "Du lieu POI da cap nhat va geofence da dang ky lai."
                : "Dong bo da chay xong.";
        }
        finally
        {
            ExitRuntime();
        }
    }

    private IReadOnlyList<Poi> GetPoiSnapshot()
    {
        lock (_poiStateLock)
        {
            return _pois.ToList();
        }
    }

    private async Task<bool> TryEnterRuntimeAsync()
    {
        if (!_isVisible)
            return false;

        if (!await _runtimeLock.WaitAsync(0))
        {
            System.Diagnostics.Debug.WriteLine("[MapPage] Runtime operation skipped because another operation is active.");
            return false;
        }

        _isRuntimeBusy = true;
        return true;
    }

    private void ExitRuntime()
    {
        if (!_isRuntimeBusy)
            return;

        _isRuntimeBusy = false;
        _runtimeLock.Release();
    }
}