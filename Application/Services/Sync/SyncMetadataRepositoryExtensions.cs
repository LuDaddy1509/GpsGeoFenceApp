using Microsoft.Maui.Storage;
using MauiApp1.Data;

namespace MauiApp1.Services.Sync;

// Extension methods cho SyncMetadataRepository để không cần sửa class gốc
public static class SyncMetadataRepositoryExtensions
{
    private static string Key(string dataset) => $"sync:lastUtc:{dataset}";
    public static Task<DateTime?> GetLastSyncUtcAsync(this SyncMetadataRepository repo, string dataset)
    {
        // lưu dạng ISO string, null nếu chưa có
        var s = Preferences.Get(Key(dataset), string.Empty);
        if (string.IsNullOrWhiteSpace(s)) return Task.FromResult<DateTime?>(null);

        if (DateTime.TryParse(s, out var dt))
            return Task.FromResult<DateTime?>(DateTime.SpecifyKind(dt, DateTimeKind.Utc));

        return Task.FromResult<DateTime?>(null);
    }

    public static Task SetLastSyncUtcAsync(this SyncMetadataRepository repo, string dataset, DateTime utc)
    {
        Preferences.Set(Key(dataset), utc.ToUniversalTime().ToString("O"));
        return Task.CompletedTask;
    }
}
