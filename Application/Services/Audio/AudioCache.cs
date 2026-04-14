using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Maui.Storage;

namespace MauiApp1.Services.Audio;

public sealed class AudioCache
{
    private static readonly HttpClient HttpClient = new();

    private readonly string _dir;
    private readonly ConcurrentDictionary<string, Task<string?>> _inflightDownloads = new();

    public AudioCache()
    {
        _dir = Path.Combine(FileSystem.AppDataDirectory, "audio");
        Directory.CreateDirectory(_dir);
    }

    public static string Sha256Hex(string s)
    {
        using var sha = SHA256.Create();
        var b = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
        var sb = new StringBuilder(b.Length * 2);
        foreach (var x in b)
            sb.Append(x.ToString("x2"));
        return sb.ToString();
    }

    public string GetLocalPathFromId(string id) => Path.Combine(_dir, id + ".mp3");

    public bool IsCached(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return TryGetCachedPath(url) is not null;
    }

    public string? TryGetCachedPath(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var id = Sha256Hex(url);
        var path = GetLocalPathFromId(id);
        return File.Exists(path) ? path : null;
    }

    public Task<string?> GetOrAddFromUrlAsync(string url, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult<string?>(null);

        var cachedPath = TryGetCachedPath(url);
        if (!string.IsNullOrWhiteSpace(cachedPath))
            return Task.FromResult<string?>(cachedPath);

        var id = Sha256Hex(url);
        return _inflightDownloads.GetOrAdd(id, _ => DownloadAsync(id, url, ct));
    }

    public static void CleanupOldFiles(TimeSpan olderThan)
    {
        var root = Path.Combine(FileSystem.AppDataDirectory, "audio");
        if (!Directory.Exists(root))
            return;

        foreach (var f in Directory.GetFiles(root, "*.mp3"))
        {
            try
            {
                if (DateTime.UtcNow - File.GetLastWriteTimeUtc(f) > olderThan)
                    File.Delete(f);
            }
            catch
            {
            }
        }
    }

    private async Task<string?> DownloadAsync(string id, string url, CancellationToken ct)
    {
        var path = GetLocalPathFromId(id);

        try
        {
            using var resp = await HttpClient.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();

            await using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await resp.Content.CopyToAsync(fs, ct);

            System.Diagnostics.Debug.WriteLine($"[AudioCache] Downloaded audio for {id}");
            return path;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AudioCache] Download failed for {id}: {ex.Message}");
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }

            return null;
        }
        finally
        {
            _inflightDownloads.TryRemove(id, out _);
        }
    }
}
