using MauiApp1.Services.Map;
using MauiApp1.Services.Navigation;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace MauiApp1.Pages;

public partial class QrScanPage : ContentPage
{
    private readonly MapRuntimeService _runtimeService;
    private readonly PoiNavigationService _poiNavigationService;
    private readonly QrPayloadResolver _qrPayloadResolver;
    private bool _isProcessing;
    private bool _torchOn;
    private CameraBarcodeReaderView? _cameraView;

    public QrScanPage(
        MapRuntimeService runtimeService,
        PoiNavigationService poiNavigationService,
        QrPayloadResolver qrPayloadResolver)
    {
        InitializeComponent();
        _runtimeService = runtimeService;
        _poiNavigationService = poiNavigationService;
        _qrPayloadResolver = qrPayloadResolver;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isProcessing = false;

        if (_cameraView == null)
        {
            _cameraView = new CameraBarcodeReaderView
            {
                Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormats.TwoDimensional,
                    AutoRotate = true
                },
                IsDetecting = true
            };
            _cameraView.BarcodesDetected += OnBarcodesDetected;
            CameraContainer.Content = _cameraView;
        }
        else
        {
            _cameraView.IsDetecting = true;
        }
    }

    protected override void OnDisappearing()
    {
        if (_cameraView != null)
        {
            _cameraView.IsDetecting = false;
            _cameraView.IsTorchOn = false;
            _torchOn = false;
        }

        base.OnDisappearing();
    }

    private void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing)
            return;

        var raw = e.Results?.FirstOrDefault()?.Value;
        if (string.IsNullOrWhiteSpace(raw))
            return;

        _isProcessing = true;
        MainThread.BeginInvokeOnMainThread(async () => await HandleQrValueAsync(raw.Trim()));
    }

    private async Task HandleQrValueAsync(string raw)
    {
        try
        {
            if (_cameraView != null)
                _cameraView.IsDetecting = false;

            LblStatus.Text = "Dang xu ly QR...";

            var resolved = _qrPayloadResolver.Resolve(raw);
            switch (resolved.Kind)
            {
                case QrResolveKind.Poi when resolved.PoiId.HasValue:
                {
                    var poi = await _runtimeService.GetPoiByIdAsync(resolved.PoiId.Value);
                    if (poi is null)
                    {
                        LblStatus.Text = "POI chua co offline";
                        await DisplayAlert(
                            "Chua co du lieu",
                            "POI nay chua co trong du lieu cuc bo. Hay dong bo roi quet lai.",
                            "OK");
                        await ResumeScanAsync();
                        return;
                    }

                    LblStatus.Text = "Da nhan dien POI";
                    await Shell.Current.GoToAsync("..");
                    await _poiNavigationService.OpenDetailAsync(resolved.PoiId.Value, resolved.QuickPlay);
                    return;
                }
                case QrResolveKind.ExternalLink when resolved.ExternalUri is not null:
                {
                    var openLink = await DisplayAlert(
                        "Lien ket ngoai",
                        $"Ban co muon mo duong dan nay khong?\n\n{resolved.ExternalUri}",
                        "Mo",
                        "Huy");

                    if (openLink)
                        await Launcher.Default.OpenAsync(resolved.ExternalUri);

                    await ResumeScanAsync();
                    return;
                }
                default:
                    LblStatus.Text = "QR khong hop le";
                    await DisplayAlert(
                        "Khong nhan dien duoc",
                        resolved.ErrorMessage ?? "Ma QR nay khong thuoc flow POI cua ung dung.",
                        "Quet lai");
                    await ResumeScanAsync();
                    return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QR Error] {ex.Message}");
            await ResumeScanAsync();
        }
    }

    private async Task ResumeScanAsync()
    {
        _isProcessing = false;
        await Task.Delay(500);
        if (_cameraView != null)
            _cameraView.IsDetecting = true;
        LblStatus.Text = "San sang quet";
    }

    private void OnToggleTorchClicked(object? sender, EventArgs e)
    {
        try
        {
            _torchOn = !_torchOn;
            if (_cameraView != null)
                _cameraView.IsTorchOn = _torchOn;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Flash Error] {ex.Message}");
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await CloseAsync();

    private Task CloseAsync() => Shell.Current.GoToAsync("..");
}
