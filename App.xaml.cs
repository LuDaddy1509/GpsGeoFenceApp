namespace GpsGeoFence;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            var shell = IPlatformApplication.Current!.Services
                            .GetRequiredService<AppShell>();
            return new Window(shell) { Title = "GPS GeoFence" };
        }
        catch (Exception ex)
        {
            // Hiện lỗi trực tiếp lên màn hình
            var msg = ex.ToString();
            System.Diagnostics.Debug.WriteLine($"[CRASH CreateWindow] {msg}");

            var page = new ContentPage
            {
                BackgroundColor = Colors.Black,
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text              = $"LỖI KHỞI ĐỘNG:\n\n{msg}",
                        TextColor         = Colors.Red,
                        FontSize          = 11,
                        Margin            = new Thickness(12),
                        LineBreakMode     = LineBreakMode.WordWrap
                    }
                }
            };
            return new Window(page);
        }
    }
}
