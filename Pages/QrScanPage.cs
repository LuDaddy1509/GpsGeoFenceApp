// ✅ ZXing.Net.Maui v0.4 — namespace đúng
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using GpsGeoFence.Interfaces;

namespace GpsGeoFence.Pages;

// KHÔNG dùng partial — không có file XAML tương ứng
public class QrScanPage : ContentPage
{
    private readonly IGeofenceService _geofenceService;
    private bool _isProcessing;

    private CameraBarcodeReaderView? _barcodeReader;
    private ActivityIndicator?       _loadingIndicator;
    private Border?                  _resultBanner;
    private Label?                   _resultLabel;

    public QrScanPage(IGeofenceService geofenceService)
    {
        _geofenceService = geofenceService;
        BackgroundColor  = Colors.Black;
        Title            = "Quét QR Code";
        BuildUI();
    }

    private void BuildUI()
    {
        _barcodeReader = new CameraBarcodeReaderView
        {
            IsDetecting = false,
            Options = new BarcodeReaderOptions
            {
                Formats    = BarcodeFormat.QrCode,
                AutoRotate = true,
                Multiple   = false
            }
        };
        _barcodeReader.BarcodesDetected += OnBarcodesDetected;

        var overlay = new BoxView
        {
            Color             = Color.FromArgb("#88000000"),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions   = LayoutOptions.Fill
        };

        var scanFrame = new Border
        {
            WidthRequest      = 260,
            HeightRequest     = 260,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions   = LayoutOptions.Center,
            BackgroundColor   = Colors.Transparent,
            Stroke            = new SolidColorBrush(Colors.White),
            StrokeThickness   = 2,
            StrokeShape       = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                                { CornerRadius = 16 }
        };

        var instruction = new Label
        {
            Text                    = "Hướng camera vào mã QR tại điểm tham quan",
            FontSize                = 14,
            TextColor               = Colors.White,
            HorizontalOptions       = LayoutOptions.Center,
            VerticalOptions         = LayoutOptions.End,
            Margin                  = new Thickness(20, 0, 20, 120),
            HorizontalTextAlignment = TextAlignment.Center
        };

        _loadingIndicator = new ActivityIndicator
        {
            Color             = Colors.White,
            IsVisible         = false,
            IsRunning         = false,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions   = LayoutOptions.Center
        };

        _resultLabel = new Label
        {
            FontSize          = 15,
            FontAttributes    = FontAttributes.Bold,
            TextColor         = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _resultBanner = new Border
        {
            IsVisible         = false,
            Margin            = new Thickness(16, 0, 16, 60),
            VerticalOptions   = LayoutOptions.End,
            BackgroundColor   = Color.FromArgb("#4CAF50"),
            StrokeShape       = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                                { CornerRadius = 14 },
            Stroke            = new SolidColorBrush(Colors.Transparent),
            Padding           = new Thickness(16, 12),
            Content           = _resultLabel
        };

        Content = new Grid
        {
            Children =
            {
                _barcodeReader,
                overlay,
                scanFrame,
                instruction,
                _loadingIndicator,
                _resultBanner
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
        {
            if (_barcodeReader != null)
                _barcodeReader.IsDetecting = true;
            _isProcessing = false;
            HideResult();
        }
        else
        {
            await DisplayAlert("Thiếu quyền",
                "Ứng dụng cần quyền Camera để quét QR Code.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_barcodeReader != null)
            _barcodeReader.IsDetecting = false;
    }

    private async void OnBarcodesDetected(object? sender,
        BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        var first = e.Results.FirstOrDefault();
        if (first is null) { _isProcessing = false; return; }

        var qrValue = first.Value?.Trim();
        if (string.IsNullOrEmpty(qrValue)) { _isProcessing = false; return; }

        await MainThread.InvokeOnMainThreadAsync(
            () => ProcessQrCodeAsync(qrValue));
    }

    private async Task ProcessQrCodeAsync(string qrValue)
    {
        if (_barcodeReader != null)
            _barcodeReader.IsDetecting = false;
        ShowLoading(true);

        try
        {
            await _geofenceService.TriggerByQrCodeAsync(qrValue);
            ShowResult("✓ Đang phát thuyết minh", isSuccess: true);
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Task.Delay(1500);
            await Shell.Current.GoToAsync("..");
        }
        catch (KeyNotFoundException)
        {
            ShowResult($"Không tìm thấy: \"{qrValue}\"", isSuccess: false);
            await Task.Delay(2500);
            ResetForRescan();
        }
        catch (Exception ex)
        {
            ShowResult($"Lỗi: {ex.Message}", isSuccess: false);
            await Task.Delay(2500);
            ResetForRescan();
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void ShowLoading(bool show)
    {
        if (_loadingIndicator is null) return;
        _loadingIndicator.IsVisible = show;
        _loadingIndicator.IsRunning = show;
    }

    private void ShowResult(string message, bool isSuccess)
    {
        if (_resultLabel is null || _resultBanner is null) return;
        _resultLabel.Text             = message;
        _resultBanner.BackgroundColor = isSuccess
            ? Color.FromArgb("#4CAF50")
            : Color.FromArgb("#F44336");
        _resultBanner.IsVisible       = true;
    }

    private void HideResult()
    {
        if (_resultBanner is null) return;
        _resultBanner.IsVisible = false;
        if (_resultLabel is not null) _resultLabel.Text = string.Empty;
    }

    private void ResetForRescan()
    {
        HideResult();
        _isProcessing = false;
        if (_barcodeReader != null)
            _barcodeReader.IsDetecting = true;
    }
}
