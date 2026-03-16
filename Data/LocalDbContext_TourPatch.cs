// ═══════════════════════════════════════════════════════════
// PATCH cho LocalDbContext.cs
// Thêm vào phần InitAsync() và bổ sung Tour methods
// ═══════════════════════════════════════════════════════════
//
// BƯỚC 1 — Trong InitAsync(), thêm 2 dòng sau CreateTableAsync<UserLocation>():
//
//     await _db.CreateTableAsync<Tour>();
//     await _db.CreateTableAsync<TourPoi>();
//
// ═══════════════════════════════════════════════════════════
// BƯỚC 2 — Thêm các methods sau vào class LocalDbContext:
// ═══════════════════════════════════════════════════════════

using GpsGeoFence.Models;
using SQLite;

namespace GpsGeoFence.Data;

/// <summary>
/// Các methods bổ sung cho Tour — copy vào LocalDbContext.cs.
/// </summary>
public partial class LocalDbContext
{
    // ──────────────────────────────────────────
    // TOUR OPERATIONS
    // ──────────────────────────────────────────

    /// <summary>Lấy tất cả tour đang active.</summary>
    public Task<List<Tour>> GetAllToursAsync()
        => Db.Table<Tour>().Where(t => t.IsActive).ToListAsync();

    /// <summary>Lấy tour theo ID kèm danh sách POI.</summary>
    public async Task<Tour?> GetTourByIdAsync(int id)
    {
        var tour = await Db.Table<Tour>()
                           .Where(t => t.Id == id)
                           .FirstOrDefaultAsync();
        if (tour is null) return null;

        // Load TourPois theo thứ tự
        tour.TourPois = await Db.Table<TourPoi>()
                                .Where(tp => tp.TourId == id)
                                .OrderBy(tp => tp.OrderIndex)
                                .ToListAsync();
        return tour;
    }

    /// <summary>Upsert danh sách tour từ API.</summary>
    public async Task UpsertToursAsync(IEnumerable<Tour> tours)
    {
        await Db.RunInTransactionAsync(conn =>
        {
            foreach (var tour in tours)
            {
                var existing = conn.Find<Tour>(tour.Id);
                if (existing is null) conn.Insert(tour);
                else conn.Update(tour);

                // Xoá TourPois cũ rồi insert lại
                conn.Execute("DELETE FROM TourPOIs WHERE TourId = ?", tour.Id);
                foreach (var tp in tour.TourPois)
                {
                    tp.TourId = tour.Id;
                    conn.Insert(tp);
                }
            }
        });
    }

    /// <summary>Xoá toàn bộ tours.</summary>
    public async Task DeleteAllToursAsync()
    {
        await Db.DeleteAllAsync<TourPoi>();
        await Db.DeleteAllAsync<Tour>();
    }

    // ──────────────────────────────────────────
    // HELPER — ExecuteRawAsync (dùng trong LocalCacheService)
    // ──────────────────────────────────────────

    /// <summary>Chạy raw SQL với parameters.</summary>
    public Task<int> ExecuteRawAsync(string sql, params object[] args)
        => Db.ExecuteAsync(sql, args);
}
