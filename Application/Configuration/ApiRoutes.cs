namespace MauiApp1.Configuration;

public static class ApiRoutes
{
    public const string Pois = "/api/v1/pois";
    public const string AuthLogin = "/api/v1/auth/login";
    public const string AuthRegister = "/api/v1/auth/register";
    public const string History = "/api/v1/history";
    public const string Playbacks = "/api/v1/playbacks";
    public const string Visits = "/api/v1/visits";
    public const string TranslatorTranslate = "/api/v1/translator/translate";

    public static string PoiNarration(int poiId, string lang, string eventType) =>
        $"/api/v1/pois/{poiId}/narration?lang={Uri.EscapeDataString(lang)}&eventType={Uri.EscapeDataString(eventType)}";
}
