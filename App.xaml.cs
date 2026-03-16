namespace GpsGeoFence;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
        // FIX: Không dùng MainPage = shell (obsolete trong .NET MAUI 10)
        // Dùng CreateWindow override thay thế
    }

    // FIX: Override CreateWindow thay vì set MainPage
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_shell) { Title = "GPS GeoFence" };
}
