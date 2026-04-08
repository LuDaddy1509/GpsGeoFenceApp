using Microsoft.Data.Sqlite;

namespace MauiApp1.Data;

public sealed class SyncMetadataRepository
{
    private bool _inited;

    private async Task InitAsync()
    {
        if (_inited) return;

        await using var conn = new SqliteConnection(Constants.ConnectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS SyncMeta(
    Key   TEXT PRIMARY KEY,
    Value TEXT NOT NULL
);";
        await cmd.ExecuteNonQueryAsync();

        _inited = true;
    }

    private static string MakeKey(string scope) => $"LastSyncUtc:{scope}";

    public async Task<DateTime?> GetLastSyncUtcAsync(string scope)
    {
        await InitAsync();

        await using var conn = new SqliteConnection(Constants.ConnectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Value FROM SyncMeta WHERE Key = $k LIMIT 1;";
        cmd.Parameters.AddWithValue("$k", MakeKey(scope));

        var obj = await cmd.ExecuteScalarAsync();
        if (obj is null) return null;

        var s = obj.ToString();
        if (string.IsNullOrWhiteSpace(s)) return null;

        if (DateTime.TryParse(s, out var dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return null;
    }

    public async Task SetLastSyncUtcAsync(string scope, DateTime utc)
    {
        await InitAsync();

        await using var conn = new SqliteConnection(Constants.ConnectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO SyncMeta(Key, Value) VALUES ($k, $v)
ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;";
        cmd.Parameters.AddWithValue("$k", MakeKey(scope));
        cmd.Parameters.AddWithValue("$v", utc.ToUniversalTime().ToString("O"));

        await cmd.ExecuteNonQueryAsync();
    }
}