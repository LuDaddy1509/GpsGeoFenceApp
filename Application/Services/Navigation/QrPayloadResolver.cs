using System.Text.Json;

namespace MauiApp1.Services.Navigation;

public sealed class QrPayloadResolver
{
    private const string PoiSchemePrefix = "smarttourism://poi/";

    public QrResolveResult Resolve(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return QrResolveResult.Invalid("QR rong hoac khong hop le.");

        var trimmed = raw.Trim();

        if (TryResolvePoi(trimmed, out var poiPayload))
            return QrResolveResult.Poi(poiPayload.PoiId, poiPayload.QuickPlay);

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return QrResolveResult.External(uriResult);
        }

        return QrResolveResult.Invalid("Ma QR nay khong thuoc flow POI cua ung dung.");
    }

    private static bool TryResolvePoi(string raw, out QrPoiPayload payload)
    {
        if (raw.StartsWith(PoiSchemePrefix, StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(raw);
            var lastSegment = uri.AbsolutePath.Trim('/').Split('/').LastOrDefault();
            var quickPlay = string.Equals(uri.Query, "?mode=quickplay", StringComparison.OrdinalIgnoreCase)
                || uri.Query.Contains("quickplay=true", StringComparison.OrdinalIgnoreCase);

            if (int.TryParse(lastSegment, out var poiId))
            {
                payload = new QrPoiPayload(poiId, quickPlay);
                return true;
            }
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            int? poiId = null;
            var quickPlay = false;

            if (root.TryGetProperty("poi_id", out var poiElement))
            {
                poiId = poiElement.ValueKind == JsonValueKind.Number
                    ? poiElement.GetInt32()
                    : int.TryParse(poiElement.GetString(), out var parsedPoiId) ? parsedPoiId : null;
            }

            if (root.TryGetProperty("quick_play", out var quickPlayElement) &&
                quickPlayElement.ValueKind == JsonValueKind.True)
            {
                quickPlay = true;
            }

            if (poiId.HasValue)
            {
                payload = new QrPoiPayload(poiId.Value, quickPlay);
                return true;
            }
        }
        catch
        {
        }

        if (int.TryParse(raw, out var directPoiId))
        {
            payload = new QrPoiPayload(directPoiId, false);
            return true;
        }

        payload = default;
        return false;
    }

    public readonly record struct QrPoiPayload(int PoiId, bool QuickPlay);
}

public sealed record QrResolveResult(
    QrResolveKind Kind,
    int? PoiId = null,
    bool QuickPlay = false,
    Uri? ExternalUri = null,
    string? ErrorMessage = null)
{
    public static QrResolveResult Poi(int poiId, bool quickPlay) =>
        new(QrResolveKind.Poi, PoiId: poiId, QuickPlay: quickPlay);

    public static QrResolveResult External(Uri uri) =>
        new(QrResolveKind.ExternalLink, ExternalUri: uri);

    public static QrResolveResult Invalid(string message) =>
        new(QrResolveKind.Invalid, ErrorMessage: message);
}

public enum QrResolveKind
{
    Poi,
    ExternalLink,
    Invalid
}
