using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MauiApp1.Configuration;
using Font = Microsoft.Maui.Font;

namespace MauiApp1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(AppRoutes.QrScan, typeof(Pages.QrScanPage));
        Routing.RegisterRoute(AppRoutes.PoiDetail, typeof(Pages.PoiDetailPage));
        Routing.RegisterRoute(AppRoutes.Settings, typeof(Pages.SettingsPage));
    }

    public static async Task DisplaySnackbarAsync(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FF3300"),
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = new CornerRadius(0),
            Font = Font.SystemFontOfSize(18),
            ActionButtonFont = Font.SystemFontOfSize(14)
        };

        var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
        await snackbar.Show(cancellationTokenSource.Token);
    }

    public static async Task DisplayToastAsync(string message)
    {
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(message, textSize: 18);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await toast.Show(cts.Token);
    }
}
