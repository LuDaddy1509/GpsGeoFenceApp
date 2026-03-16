using GpsGeoFence.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace GpsGeoFence.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _vm;
    private bool _isFirstLoad = true;

    public MapPage(MapViewModel vm)
    {
        InitializeComponent();
        _vm            = vm;
        BindingContext = vm;

        // ✅ FIX: Wire events trong code-behind (không binding trong XAML)
        DismissErrorBtn.Clicked += OnDismissError;

        // ✅ FIX: Theo dõi thay đổi để cập nhật map
        _vm.PropertyChanged += OnViewModelPropertyChanged;

        // ✅ FIX: Set vùng bản đồ mặc định từ code (VisibleRegion không bindable trực tiếp)
        MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(
            new Location(10.762622, 106.660172),
            Distance.FromKilometers(2)));
    }

    // ──────────────────────────────────────────
    // LIFECYCLE
    // ──────────────────────────────────────────

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            await _vm.OnAppearingAsync();
            // Load pins lên bản đồ sau khi POI được tải
            RefreshMapPins();
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _vm.OnDisappearingAsync();
    }

    // ──────────────────────────────────────────
    // MAP PINS
    // ──────────────────────────────────────────

    // ✅ FIX: Thêm Pin trực tiếp vào MainMap.Pins thay vì binding ItemsSource
    private void RefreshMapPins()
    {
        MainMap.Pins.Clear();

        foreach (var poi in _vm.Pois)
        {
            var pin = new Pin
            {
                Label    = poi.Name,
                Address  = $"Bán kính: {poi.RadiusMeters}m",
                Location = new Location(poi.Latitude, poi.Longitude),
                Type     = PinType.Place
            };
            pin.MarkerClicked += OnPinMarkerClicked;
            MainMap.Pins.Add(pin);
        }
    }

    // ──────────────────────────────────────────
    // MAP EVENTS
    // ──────────────────────────────────────────

    // ✅ FIX: Event handler đặt ở code-behind, không dùng XAML binding
    private async void OnPinMarkerClicked(object? sender, PinClickedEventArgs e)
    {
        e.HideInfoWindow = true;
        if (sender is not Pin pin) return;

        var poi = _vm.Pois.FirstOrDefault(p => p.Name == pin.Label);
        if (poi is null) return;

        if (_vm.SelectPoiCommand.CanExecute(poi))
            await _vm.SelectPoiCommand.ExecuteAsync(poi);
    }

    // ✅ FIX: Dismiss error từ code-behind
    private void OnDismissError(object? sender, EventArgs e)
    {
        _vm.HasError     = false;
        _vm.ErrorMessage = string.Empty;
    }

    // ──────────────────────────────────────────
    // VIEWMODEL CHANGES
    // ──────────────────────────────────────────

    private void OnViewModelPropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Khi danh sách POI thay đổi → refresh pins
        if (e.PropertyName == nameof(MapViewModel.Pois))
            RefreshMapPins();

        // Khi vị trí user thay đổi → zoom bản đồ
        if (e.PropertyName == nameof(MapViewModel.UserLocation)
            && _vm.UserLocation is not null)
        {
            // Uncomment dòng dưới nếu muốn auto-follow user:
            // MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(
            //     _vm.UserLocation, Distance.FromMeters(500)));
        }
    }

    // ──────────────────────────────────────────
    // DISPOSE
    // ──────────────────────────────────────────

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.NewHandler is null)
        {
            _vm.PropertyChanged    -= OnViewModelPropertyChanged;
            DismissErrorBtn.Clicked -= OnDismissError;
        }
    }
}
