using MapApi.Configuration;
using Microsoft.Extensions.Options;

namespace MapApi.Services;

public sealed class MediaStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApiRuntimeOptions _options;

    public MediaStorageService(IWebHostEnvironment environment, IOptions<ApiRuntimeOptions> options)
    {
        _environment = environment;
        _options = options.Value;
    }

    public async Task<string> SaveImageAsync(int poiId, IFormFile file, CancellationToken ct) =>
        await SaveAsync(poiId, file, "images", _options.AllowedImageExtensions, ct);

    public async Task<string> SaveAudioAsync(int poiId, IFormFile file, CancellationToken ct) =>
        await SaveAsync(poiId, file, "audio", _options.AllowedAudioExtensions, ct);

    private async Task<string> SaveAsync(
        int poiId,
        IFormFile file,
        string folderName,
        IReadOnlyCollection<string> allowedExtensions,
        CancellationToken ct)
    {
        if (file is null)
            throw new InvalidOperationException("Missing file.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException($"Unsupported file extension: {ext}");

        if (file.Length <= 0)
            throw new InvalidOperationException("Empty file.");

        var maxBytes = _options.MediaMaxUploadMegabytes * 1024 * 1024L;
        if (file.Length > maxBytes)
            throw new InvalidOperationException($"File too large. Max {_options.MediaMaxUploadMegabytes} MB.");

        var root = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            Directory.CreateDirectory(root);
        }

        var dir = Path.Combine(root, folderName);
        Directory.CreateDirectory(dir);

        var safeName = $"{poiId}_{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(dir, safeName);

        await using var fs = File.Create(path);
        await file.CopyToAsync(fs, ct);

        return $"/{folderName}/{safeName}";
    }
}
