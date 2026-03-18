using GpsGeoFence.ViewModels;

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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            await _vm.OnAppearingAsync();
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _vm.OnDisappearingAsync();
    }
}
