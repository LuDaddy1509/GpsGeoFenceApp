using MapApi.Data;
using MapApi.Models;
using MapApi.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 0) KESTREL: Listen IP (LAN) cho cả HTTP + HTTPS
//    - HTTP  : 5150  (điện thoại gọi ổn nhất khi DEV)
//    - HTTPS : 7286  (điện thoại gọi bằng IP có thể fail cert dev)
// ============================================================
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5150);
    options.ListenAnyIP(7286, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Server=.\\SQLEXPRESS;Database=GpsApi;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
builder.Services.AddDbContext<AppDb>(opt =>
    opt.UseSqlServer(cs, sql =>
    {
        sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
        sql.CommandTimeout(120);
    })
);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddHttpClient();
builder.Services.AddScoped<TranslatorClient>();

var app = builder.Build();
app.UseCors();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/v1/pois", async (AppDb db) =>
{
    var items = await db.Pois
        .AsNoTracking()
        .Where(p => p.IsActive)
        .OrderBy(p => p.Priority)
        .ThenBy(p => p.Name)
        .ToListAsync();

    return Results.Ok(items);
});
app.MapGet("/api/v1/pois/{id}", async (string id, AppDb db) =>
{
    var poi = await db.Pois
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);

    return poi is null
        ? Results.NotFound(new { error = "POI not found" })
        : Results.Ok(poi);
});
app.MapGet("/api/v1/pois/{id}/narration", async (
    string id,
    string? lang,
    string? eventType,
    AppDb db,
    TranslatorClient translator,
    CancellationToken ct) =>
{
    var poi = await db.Pois.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
    if (poi is null)
        return Results.NotFound(new { error = "POI not found" });
    var et = ParseNarrationEventType(eventType);
    return Results.Ok(new
    {
        poiId = poi.Id,
        language = poi.Language,
        requestedLang = lang,
        eventType = et,
        narration = poi.NarrationText
    });
});
app.MapPost("/api/v1/playback", async (PlaybackCreateRequest req, AppDb db) =>
{
    if (string.IsNullOrWhiteSpace(req.PoiId))
        return Results.BadRequest(new { error = "PoiId is required" });
    var exists = await db.Pois.AsNoTracking().AnyAsync(p => p.Id == req.PoiId);
    if (!exists)
        return Results.NotFound(new { error = "POI not found" });
    return Results.Ok(new
    {
        ok = true,
        poiId = req.PoiId,
        trigger = ParsePlaybackEventType(req.TriggerType),
        req.DistanceMeters,
        req.DeviceId
    });
});
app.Run();
static byte ParseNarrationEventType(string? s)
{
    return (s ?? "").Trim().ToLowerInvariant() switch
    {
        "enter" => 0,
        "near" => 1,
        "tap" => 2,
        _ => 0
    };
}
static byte ParsePlaybackEventType(string? triggerType)
{
    // 0=Enter,1=Near,2=Tap,3=QR (QR chỉ dùng cho log)
    return (triggerType ?? "").Trim().ToUpperInvariant() switch
    {
        "ENTER" => 0,
        "NEAR" => 1,
        "TAP" => 2,
        "QR" => 3,
        _ => 0
    };
}
public sealed class PlaybackCreateRequest
{
    public string PoiId { get; set; } = "";
    public string? TriggerType { get; set; } // ENTER/NEAR/TAP/QR
    public int? DurationSeconds { get; set; } // hiện chưa lưu vì PlaybackLog model chưa có field này
    public int? DistanceMeters { get; set; }
    public string? DeviceId { get; set; }
    public bool? IsSuccess { get; set; } 
}
