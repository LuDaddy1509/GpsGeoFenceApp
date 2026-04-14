using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;

using MauiApp1.Configuration;

using ZXing.Net.Maui.Controls;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var mobileOptions = MobileAppOptionsLoader.Load();

        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(mobileOptions);
        builder.Services
            .AddTravelPlatformServices()
            .AddTravelCoreServices()
            .AddTravelApiClients(mobileOptions)
            .AddTravelPages();

        return builder.Build();
    }
}
