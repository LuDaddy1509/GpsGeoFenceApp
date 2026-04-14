using MapApi.Data;
using MapApi.Dtos.Narration;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Services;

public sealed class NarrationService
{
    private static readonly Dictionary<string, string> NearPrefix = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vi-VN"] = "Ban sap den {0}.",
        ["en-US"] = "You are approaching {0}.",
        ["zh-Hans"] = "You are approaching {0}.",
        ["ja-JP"] = "You are approaching {0}.",
        ["ko-KR"] = "You are approaching {0}.",
        ["de-DE"] = "You are approaching {0}."
    };

    private static readonly Dictionary<string, string> EnterPrefix = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vi-VN"] = "Ban da den {0}.",
        ["en-US"] = "You have arrived at {0}.",
        ["zh-Hans"] = "You have arrived at {0}.",
        ["ja-JP"] = "You have arrived at {0}.",
        ["ko-KR"] = "You have arrived at {0}.",
        ["de-DE"] = "You have arrived at {0}."
    };

    private static readonly Dictionary<string, string> DwellPrefix = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vi-VN"] = "Ban dang o tai {0}.",
        ["en-US"] = "You are now within {0}.",
        ["zh-Hans"] = "You are now within {0}.",
        ["ja-JP"] = "You are now within {0}.",
        ["ko-KR"] = "You are now within {0}.",
        ["de-DE"] = "You are now within {0}."
    };

    private readonly AppDb _db;

    public NarrationService(AppDb db)
    {
        _db = db;
    }

    public async Task<PoiNarrationResponse?> GetNarrationAsync(int poiId, string? lang, string? eventType, CancellationToken ct)
    {
        var poi = await _db.Pois.AsNoTracking().FirstOrDefaultAsync(p => p.Id == poiId, ct);
        if (poi is null)
            return null;

        var language = string.IsNullOrWhiteSpace(lang) ? "vi-VN" : lang.Trim();
        var eventTypeByte = ParseEventTypeByte(eventType);
        var languageRow = await _db.PoiLanguages.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPoi == poiId && x.LanguageTag == language, ct);

        var prefix = BuildPrefix(language, eventTypeByte, poi.Name);
        var narrationText = string.IsNullOrWhiteSpace(languageRow?.TextToSpeech)
            ? prefix
            : $"{prefix} {languageRow!.TextToSpeech}";

        return new PoiNarrationResponse
        {
            PoiId = poiId,
            EventType = eventTypeByte,
            Language = language,
            NarrationText = narrationText,
            Cached = languageRow is not null
        };
    }

    public static byte ParseEventTypeByte(string? value) =>
        (value ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "enter" => 0,
            "near" => 1,
            "tap" => 2,
            "dwell" => 3,
            _ => 0
        };

    private static string BuildPrefix(string language, byte eventType, string poiName)
    {
        var source = eventType switch
        {
            1 => NearPrefix,
            3 => DwellPrefix,
            _ => EnterPrefix
        };

        var template = source.TryGetValue(language, out var exact)
            ? exact
            : source["vi-VN"];

        return string.Format(template, poiName);
    }
}
