using MauiApp1.Configuration;
using Microsoft.Maui.Storage;

namespace MauiApp1.Services;

public static class LanguageService
{
    public static readonly (string Code, string Name, string Flag)[] Supported =
    [
        ("vi-VN", "Tiếng Việt", "🇻🇳"),
        ("en-US", "English", "🇺🇸"),
        ("ja-JP", "日本語", "🇯🇵"),
        ("ko-KR", "한국어", "🇰🇷"),
        ("de-DE", "Deutsch", "🇩🇪"),
    ];

    public static string Current =>
        Preferences.Get(SessionKeys.UserLanguage, "vi-VN");

    public static void Set(string languageCode) =>
        Preferences.Set(SessionKeys.UserLanguage, languageCode);

    public static string Display(string code) =>
        Supported.FirstOrDefault(x => x.Code == code) is var found && found != default
            ? $"{found.Flag} {found.Name}"
            : code;
}
