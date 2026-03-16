using GpsGeoFence.Pages;
using System;

    namespace GpsGeoFence;

public partial class AppShell : Shell
{
    public AppShell()
    {   
        InitializeComponent();

        // Guard the platform-specific API with a runtime platform/version check to satisfy CA1416
        if (OperatingSystem.IsAndroidVersionAtLeast(21))
        {
            Routing.RegisterRoute("poiDetail", typeof(PoiDetailPage));
            Routing.RegisterRoute("qrscan",    typeof(QrScanPage));
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            _ = Shell.Current.GoToAsync("..");
            return true;
        }
        _ = ConfirmExitAsync();
        return true;
    }

    private static async Task ConfirmExitAsync()
    {
        var confirm = await Application.Current!.MainPage!
            .DisplayAlert("Thoát app", "Bạn có muốn thoát?", "Thoát", "Huỷ");
        if (confirm)
            Application.Current.Quit();
    }
}
