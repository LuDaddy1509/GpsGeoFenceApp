using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MauiApp1.Configuration;

public static class MobileAppOptionsLoader
{
    public static MobileAppOptions Load()
    {
        var environmentName = ResolveEnvironmentName();
        var merged = LoadJsonObject("appsettings.json");
        MergeInto(merged, LoadJsonObject($"appsettings.{environmentName}.json"));

        var options = merged.Deserialize<MobileAppOptions>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new MobileAppOptions();

        options.EnvironmentName = environmentName;
        return options;
    }

    private static string ResolveEnvironmentName()
    {
        var env = Environment.GetEnvironmentVariable("GPS_APP_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(env))
            return env.Trim();

#if DEBUG
        return "Development";
#else
        return "Production";
#endif
    }

    private static JsonObject LoadJsonObject(string resourceFileName)
    {
        var assembly = typeof(MobileAppOptionsLoader).Assembly;
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceFileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return new JsonObject();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return new JsonObject();

        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        return JsonNode.Parse(content)?.AsObject() ?? new JsonObject();
    }

    private static void MergeInto(JsonObject target, JsonObject source)
    {
        foreach (var kvp in source)
        {
            if (kvp.Value is JsonObject sourceObj &&
                target[kvp.Key] is JsonObject targetObj)
            {
                MergeInto(targetObj, sourceObj);
                continue;
            }

            target[kvp.Key] = kvp.Value?.DeepClone();
        }
    }
}
