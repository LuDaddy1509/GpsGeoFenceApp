using GpsGeoFence.Models;
using SQLite;

namespace GpsGeoFence.Data;

/// <summary>
/// SQLite local database context cho GpsGeoFence.
/// Dùng sqlite-net-pcl – lưu data offline, sync lên Azure SQL khi có mạng.
/// 
/// PACKAGE CẦN CÀI:
///   Install-Package sqlite-net-pcl
///   Install-Package SQLitePCLRaw.bundle_green
/// </summary>
public partial class LocalDbContext : IAsyncDisposable
{
    private SQLiteAsyncConnection? _db;
    private bool _isInitialized;

    // ── DB path ──────────────────────────────
    private static string DbPath =>
        Path.Combine(FileSystem.AppDataDirectory, "GpsGeoFence.db3");

    // ──────────────────────────────────────────
    // INIT
    // ──────────────────────────────────────────

    /// <summary>
    /// Khởi tạo kết nối và tạo bảng nếu chưa có.
    /// Gọi 1 lần trong MauiProgram.cs hoặc lần đầu truy cập.
    /// </summary>
    public async Task InitAsync()
    {
        if (_isInitialized) return;

        _db = new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.ReadWrite
            | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _db.CreateTableAsync<POI>();
        await _db.CreateTableAsync<AudioContent>();
        await _db.CreateTableAsync<PlaybackLog>();
        await _db.CreateTableAsync<UserLocation>();

        _isInitialized = true;
    }

    private SQLiteAsyncConnection Db
    {
        get
        {
            if (_db is null || !_isInitialized)
                throw new InvalidOperationException(
                    "LocalDbContext chưa được khởi tạo. Gọi InitAsync() trước.");
            return _db;
        }
    }

    // ──────────────────────────────────────────
    // POI OPERATIONS
    // ──────────────────────────────────────────

    public Task<List<POI>> GetAllPoisAsync()
        => Db.Table<POI>().Where(p => p.IsActive).ToListAsync();

    public Task<POI?> GetPoiByIdAsync(int id)
        => Db.Table<POI>().Where(p => p.Id == id).FirstOrDefaultAsync()!;

    public Task<POI?> GetPoiByQrCodeAsync(string qrCode)
        => Db.Table<POI>().Where(p => p.QrCode == qrCode).FirstOrDefaultAsync()!;

    /// <summary>Upsert danh sách POI từ API (thay toàn bộ cache).</summary>
    public async Task UpsertPoisAsync(IEnumerable<POI> pois)
    {
        await Db.RunInTransactionAsync(conn =>
        {
            foreach (var poi in pois)
            {
                var existing = conn.Find<POI>(poi.Id);
                if (existing is null)
                    conn.Insert(poi);
                else
                    conn.Update(poi);
            }
        });
    }

    public Task<int> DeleteAllPoisAsync()
        => Db.DeleteAllAsync<POI>();

    // ──────────────────────────────────────────
    // AUDIO OPERATIONS
    // ──────────────────────────────────────────

    public Task<List<AudioContent>> GetAudioByPoiAsync(int poiId)
        => Db.Table<AudioContent>().Where(a => a.PoiId == poiId).ToListAsync();

    public Task<AudioContent?> GetDefaultAudioAsync(int poiId, string language = "vi")
        => Db.Table<AudioContent>()
             .Where(a => a.PoiId == poiId && a.Language == language && a.IsDefault)
             .FirstOrDefaultAsync()!;

    public async Task UpsertAudioContentsAsync(IEnumerable<AudioContent> items)
    {
        await Db.RunInTransactionAsync(conn =>
        {
            foreach (var item in items)
            {
                var existing = conn.Find<AudioContent>(item.Id);
                if (existing is null) conn.Insert(item);
                else conn.Update(item);
            }
        });
    }

    // ──────────────────────────────────────────
    // PLAYBACK LOG OPERATIONS
    // ──────────────────────────────────────────

    public Task<int> InsertLogAsync(PlaybackLog log)
        => Db.InsertAsync(log);

    public Task<List<PlaybackLog>> GetUnSyncedLogsAsync()
        => Db.Table<PlaybackLog>().Where(l => !l.IsSynced).ToListAsync();

    public Task<List<PlaybackLog>> GetLogsByPoiAsync(int poiId)
        => Db.Table<PlaybackLog>().Where(l => l.PoiId == poiId).ToListAsync();

    public Task<List<PlaybackLog>> GetRecentLogsAsync(int count = 50)
        => Db.QueryAsync<PlaybackLog>(
            "SELECT * FROM PlaybackLogs ORDER BY PlayedAt DESC LIMIT ?", count);

    public async Task MarkLogsAsSyncedAsync(IEnumerable<long> ids)
    {
        await Db.RunInTransactionAsync(conn =>
        {
            foreach (var id in ids)
            {
                conn.Execute(
                    "UPDATE PlaybackLogs SET IsSynced = 1 WHERE Id = ?", id);
            }
        });
    }

    /// <summary>Xoá log đã sync và cũ hơn X ngày.</summary>
    public Task<int> PurgeOldLogsAsync(int olderThanDays = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-olderThanDays).ToString("o");
        return Db.ExecuteAsync(
            "DELETE FROM PlaybackLogs WHERE IsSynced = 1 AND PlayedAt < ?", cutoff);
    }

    // ──────────────────────────────────────────
    // USER LOCATION OPERATIONS
    // ──────────────────────────────────────────

    public Task<int> InsertLocationAsync(UserLocation loc)
        => Db.InsertAsync(loc);

    public Task<List<UserLocation>> GetLocationsBySessionAsync(string sessionId)
        => Db.Table<UserLocation>().Where(l => l.SessionId == sessionId).ToListAsync();

    public Task<List<UserLocation>> GetUnSyncedLocationsAsync()
        => Db.Table<UserLocation>().Where(l => !l.IsSynced).ToListAsync();

    public Task<int> PurgeOldLocationsAsync(int olderThanDays = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(-olderThanDays).ToString("o");
        return Db.ExecuteAsync(
            "DELETE FROM UserLocations WHERE IsSynced = 1 AND RecordedAt < ?", cutoff);
    }

    // ──────────────────────────────────────────
    // ANALYTICS HELPERS
    // ──────────────────────────────────────────

    /// <summary>Top N POI được nghe nhiều nhất (offline analytics).</summary>
    public Task<List<PoiPlayCount>> GetTopPoisAsync(int n = 10)
        => Db.QueryAsync<PoiPlayCount>(
            @"SELECT PoiId, COUNT(*) AS PlayCount
              FROM PlaybackLogs
              WHERE IsSuccess = 1
              GROUP BY PoiId
              ORDER BY PlayCount DESC
              LIMIT ?", n);

    /// <summary>Thời gian nghe trung bình của một POI (giây).</summary>
    public async Task<double> GetAvgListenTimeAsync(int poiId)
    {
        var result = await Db.ExecuteScalarAsync<double>(
            "SELECT AVG(DurationListened) FROM PlaybackLogs WHERE PoiId = ? AND IsSuccess = 1",
            poiId);
        return result;
    }

    // ──────────────────────────────────────────
    // DISPOSE
    // ──────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        if (_db is not null)
            await _db.CloseAsync();
    }
}

/// <summary>Helper class cho raw query kết quả analytics.</summary>
public class PoiPlayCount
{
    public int PoiId { get; set; }
    public int PlayCount { get; set; }
}
