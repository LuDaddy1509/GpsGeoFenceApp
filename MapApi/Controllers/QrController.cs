using Microsoft.AspNetCore.Mvc;
using QRCoder;
using MapApi.Data;

namespace MapApi.Controllers;

[ApiController]
[Route("api/v1/pois/{id}")]
public class QrController : ControllerBase
{
    private readonly AppDb _db;

    public QrController(AppDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Generate QR code cho POI
    /// GET /api/v1/pois/{id}/qr?size=300&format=png
    /// </summary>
    [HttpGet("qr")]
    public async Task<IActionResult> GenerateQrCode(
        string id,
        [FromQuery] int size = 300,
        [FromQuery] string format = "png")
    {
        try
        {
            // ✅ Kiểm tra POI có tồn tại
            var poi = await _db.Pois.FindAsync(id);
            if (poi == null)
                return NotFound(new { error = "POI not found" });

            // ✅ Format QR data
            var qrData = $"smarttourism://poi/{id}";

            // ✅ Generate QR using QRCoder
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);

            byte[] imageBytes;

            if (format.ToLower() == "svg")
            {
                // ✅ SVG format
                var qrCode = new SvgQRCode(qrCodeData);
                var svgString = qrCode.GetGraphic(size);
                imageBytes = System.Text.Encoding.UTF8.GetBytes(svgString);
                return File(imageBytes, "image/svg+xml", $"{id}_qr.svg");
            }
            else
            {
                // ✅ PNG format (default)
                var qrCode = new PngByteQRCode(qrCodeData);
                imageBytes = qrCode.GetGraphic(size);
                return File(imageBytes, "image/png", $"{id}_qr.png");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QR] Error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get POI details + QR code URL
    /// GET /api/v1/pois/{id}/details
    /// </summary>
    [HttpGet("details")]
    public async Task<IActionResult> GetPoiWithQr(string id)
    {
        try
        {
            var poi = await _db.Pois.FindAsync(id);
            if (poi == null)
                return NotFound();

            return Ok(new
            {
                poi,
                qrUrl = $"/api/v1/pois/{id}/qr?format=png&size=300",
                qrSvgUrl = $"/api/v1/pois/{id}/qr?format=svg&size=300"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
