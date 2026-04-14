using MauiApp1.Data;
using Microsoft.Maui.Networking;

namespace MauiApp1.Services.AppState;

public sealed class SyncStatusService
{
    private readonly SyncMetadataRepository _metadataRepository;

    public SyncStatusService(SyncMetadataRepository metadataRepository)
    {
        _metadataRepository = metadataRepository;
    }

    public async Task<SyncStatusSnapshot> GetStatusAsync(CancellationToken ct = default)
    {
        var lastSyncUtc = await _metadataRepository.GetLastSyncUtcAsync("pois", ct);
        var networkAccess = Connectivity.Current.NetworkAccess;

        return new SyncStatusSnapshot(
            IsOnline: networkAccess == NetworkAccess.Internet,
            LastSyncUtc: lastSyncUtc,
            StatusText: BuildStatusText(networkAccess, lastSyncUtc));
    }

    private static string BuildStatusText(NetworkAccess networkAccess, DateTime? lastSyncUtc)
    {
        if (networkAccess != NetworkAccess.Internet)
        {
            return lastSyncUtc.HasValue
                ? $"Đang offline • Dùng dữ liệu đã sync lúc {lastSyncUtc:HH:mm dd/MM}"
                : "Đang offline • Chưa có dữ liệu sync";
        }

        return lastSyncUtc.HasValue
            ? $"Online • Sync lần cuối {lastSyncUtc:HH:mm dd/MM}"
            : "Online • Chưa sync dữ liệu";
    }
}

public sealed record SyncStatusSnapshot(bool IsOnline, DateTime? LastSyncUtc, string StatusText);
