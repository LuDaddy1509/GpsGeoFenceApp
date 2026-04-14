namespace MapApi.Configuration;

public sealed class ApiRuntimeOptions
{
    public const string SectionName = "ApiRuntime";

    public int SqlRetryCount { get; set; } = 3;
    public int SqlRetryDelaySeconds { get; set; } = 5;
    public int SqlCommandTimeoutSeconds { get; set; } = 120;
    public int TranslatorHttpTimeoutSeconds { get; set; } = 20;
    public bool AllowDevelopmentJwtFallback { get; set; } = false;
    public int MediaMaxUploadMegabytes { get; set; } = 10;
    public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png"];
    public string[] AllowedAudioExtensions { get; set; } = [".mp3", ".wav", ".m4a"];
}
