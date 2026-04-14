using MapApi.Common;
using MapApi.Data;
using MapApi.Dtos.Pois;
using MapApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MapApi.Services;

public sealed class PoiManagementService
{
    private readonly AppDb _db;
    private readonly TranslatorClient _translator;

    public static readonly string[] TargetLanguages =
    [
        "en-US",
        "zh-Hans",
        "ja-JP",
        "ko-KR",
        "de-DE"
    ];

    public PoiManagementService(AppDb db, TranslatorClient translator)
    {
        _db = db;
        _translator = translator;
    }

    public async Task<PagedResponse<PoiSummaryResponse>> GetPagedAsync(PoiListQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var poiQuery = _db.Pois.AsNoTracking().AsQueryable();
        if (query.IsActive.HasValue)
            poiQuery = poiQuery.Where(x => x.IsActive == query.IsActive.Value);
        else
            poiQuery = poiQuery.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var keyword = query.Search.Trim();
            poiQuery = poiQuery.Where(x =>
                x.Name.Contains(keyword) ||
                (x.Description != null && x.Description.Contains(keyword)));
        }

        var totalItems = await poiQuery.CountAsync(ct);
        var pois = await poiQuery
            .OrderByDescending(x => x.UpdatedAt)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var responses = await BuildSummaryResponsesAsync(pois, query.Lang, ct);
        return new PagedResponse<PoiSummaryResponse>
        {
            Items = responses,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        _db.Pois.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public async Task<PoiDetailResponse?> GetDetailAsync(int id, string? lang, CancellationToken ct)
    {
        var poi = await _db.Pois.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (poi is null)
            return null;

        var media = await _db.PoiMedia.AsNoTracking().FirstOrDefaultAsync(x => x.IdPoi == id, ct);
        var languages = await _db.PoiLanguages.AsNoTracking()
            .Where(x => x.IdPoi == id)
            .OrderBy(x => x.LanguageTag)
            .ToListAsync(ct);

        var preferred = ResolvePreferredLanguage(poi, languages, lang);
        return new PoiDetailResponse
        {
            Id = poi.Id,
            Name = poi.Name,
            Description = poi.Description,
            Latitude = poi.Latitude,
            Longitude = poi.Longitude,
            RadiusMeters = poi.RadiusMeters,
            NearRadiusMeters = poi.RadiusMeters * 2,
            DebounceSeconds = 3,
            CooldownSeconds = poi.CooldownSeconds,
            Priority = null,
            IsActive = poi.IsActive,
            UpdatedAt = poi.UpdatedAt,
            Language = preferred?.LanguageTag ?? "vi-VN",
            NarrationText = preferred?.TextToSpeech,
            ImageUrl = media?.Image,
            AudioUrl = media?.Audio,
            MapLink = media?.MapLink,
            Media = media is null ? null : new PoiMediaResponse
            {
                PoiId = poi.Id,
                ImageUrl = media.Image,
                AudioUrl = media.Audio,
                MapLink = media.MapLink
            },
            Languages = languages.Select(MapLanguage).ToList()
        };
    }

    public async Task<PoiDetailResponse> CreateAsync(PoiUpsertRequest request, CancellationToken ct)
    {
        var poi = new Poi
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusMeters = request.RadiusMeters,
            CooldownSeconds = request.CooldownSeconds,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Pois.Add(poi);
        await _db.SaveChangesAsync(ct);

        await AddOrUpdatePoiWithAutoTranslationAsync(poi, request.NarrationText, request.Description, null, ct);
        return (await GetDetailAsync(poi.Id, "vi-VN", ct))!;
    }

    public async Task<PoiDetailResponse?> UpdateAsync(int id, PoiUpsertRequest request, CancellationToken ct)
    {
        var poi = await _db.Pois.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (poi is null)
            return null;

        poi.Name = request.Name.Trim();
        poi.Description = request.Description?.Trim();
        poi.Latitude = request.Latitude;
        poi.Longitude = request.Longitude;
        poi.RadiusMeters = request.RadiusMeters;
        poi.CooldownSeconds = request.CooldownSeconds;
        poi.IsActive = request.IsActive;
        poi.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await AddOrUpdatePoiWithAutoTranslationAsync(poi, request.NarrationText, request.Description, null, ct);

        return await GetDetailAsync(id, "vi-VN", ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var poi = await _db.Pois.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (poi is null)
            return false;

        _db.Pois.Remove(poi);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PoiDetailResponse?> SetStatusAsync(int id, bool isActive, CancellationToken ct)
    {
        var poi = await _db.Pois.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (poi is null)
            return null;

        poi.IsActive = isActive;
        poi.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await GetDetailAsync(id, "vi-VN", ct);
    }

    public async Task<IReadOnlyList<PoiLanguageResponse>> GetLanguagesAsync(int poiId, CancellationToken ct)
    {
        var languages = await _db.PoiLanguages.AsNoTracking()
            .Where(x => x.IdPoi == poiId)
            .OrderBy(x => x.LanguageTag)
            .ToListAsync(ct);

        return languages.Select(MapLanguage).ToList();
    }

    public async Task<PoiLanguageResponse> UpsertLanguageAsync(int poiId, PoiLanguageUpsertRequest request, CancellationToken ct)
    {
        var row = await _db.PoiLanguages
            .FirstOrDefaultAsync(x => x.IdPoi == poiId && x.LanguageTag == request.LanguageTag, ct);

        if (row is null)
        {
            row = new PoiLanguage
            {
                IdPoi = poiId,
                LanguageTag = request.LanguageTag.Trim(),
                TextToSpeech = request.TextToSpeech?.Trim()
            };
            _db.PoiLanguages.Add(row);
        }
        else
        {
            row.TextToSpeech = request.TextToSpeech?.Trim();
        }

        await _db.SaveChangesAsync(ct);
        return MapLanguage(row);
    }

    public async Task<bool> DeleteLanguageAsync(int poiId, string languageTag, CancellationToken ct)
    {
        var row = await _db.PoiLanguages
            .FirstOrDefaultAsync(x => x.IdPoi == poiId && x.LanguageTag == languageTag, ct);
        if (row is null)
            return false;

        _db.PoiLanguages.Remove(row);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PoiMediaResponse> UpsertMediaLinksAsync(int poiId, PoiMediaLinksUpdateRequest request, CancellationToken ct)
    {
        var row = await _db.PoiMedia.FirstOrDefaultAsync(x => x.IdPoi == poiId, ct);
        if (row is null)
        {
            row = new PoiMedia { IdPoi = poiId };
            _db.PoiMedia.Add(row);
        }

        row.Image = request.ImageUrl ?? row.Image;
        row.Audio = request.AudioUrl ?? row.Audio;
        row.MapLink = request.MapLink ?? row.MapLink;

        await _db.SaveChangesAsync(ct);
        return new PoiMediaResponse
        {
            PoiId = poiId,
            ImageUrl = row.Image,
            AudioUrl = row.Audio,
            MapLink = row.MapLink
        };
    }

    public async Task<PoiMediaResponse?> GetMediaAsync(int poiId, CancellationToken ct)
    {
        var media = await _db.PoiMedia.AsNoTracking().FirstOrDefaultAsync(x => x.IdPoi == poiId, ct);
        return media is null
            ? null
            : new PoiMediaResponse
            {
                PoiId = poiId,
                ImageUrl = media.Image,
                AudioUrl = media.Audio,
                MapLink = media.MapLink
            };
    }

    public async Task<PoiMediaResponse> SetImageAsync(int poiId, string imageUrl, CancellationToken ct)
    {
        var media = await EnsureMediaAsync(poiId, ct);
        media.Image = imageUrl;
        await _db.SaveChangesAsync(ct);
        return new PoiMediaResponse { PoiId = poiId, ImageUrl = media.Image, AudioUrl = media.Audio, MapLink = media.MapLink };
    }

    public async Task<PoiMediaResponse> SetAudioAsync(int poiId, string audioUrl, CancellationToken ct)
    {
        var media = await EnsureMediaAsync(poiId, ct);
        media.Audio = audioUrl;
        await _db.SaveChangesAsync(ct);
        return new PoiMediaResponse { PoiId = poiId, ImageUrl = media.Image, AudioUrl = media.Audio, MapLink = media.MapLink };
    }

    public async Task AddOrUpdatePoiWithAutoTranslationAsync(
        Poi poi,
        string? viNarration,
        string? viDesc,
        IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        if (_db.Entry(poi).State == EntityState.Detached)
        {
            var existing = await _db.Pois.FirstOrDefaultAsync(x => x.Id == poi.Id, ct);
            if (existing is null)
                _db.Pois.Add(poi);
            else
                _db.Entry(existing).CurrentValues.SetValues(poi);

            await _db.SaveChangesAsync(ct);
        }

        var viTts = CombineTts(viNarration, viDesc);
        await UpsertLanguageInternalAsync(poi.Id, "vi-VN", viTts, ct);
        progress?.Report($"[Poi] Saved vi-VN for {poi.Id}");

        foreach (var lang in TargetLanguages)
        {
            try
            {
                var translatedNarration = string.IsNullOrWhiteSpace(viNarration)
                    ? null
                    : await _translator.TryTranslateAsync(viNarration, lang, "vi-VN", ct);
                var translatedDescription = string.IsNullOrWhiteSpace(viDesc)
                    ? null
                    : await _translator.TryTranslateAsync(viDesc, lang, "vi-VN", ct);

                await UpsertLanguageInternalAsync(poi.Id, lang, CombineTts(translatedNarration, translatedDescription), ct);
                progress?.Report($"[Poi] Saved {lang} for {poi.Id}");
            }
            catch (Exception ex)
            {
                progress?.Report($"[Poi] Translate failed {lang} for {poi.Id}: {ex.Message}");
            }
        }
    }

    private async Task<PoiMedia> EnsureMediaAsync(int poiId, CancellationToken ct)
    {
        var media = await _db.PoiMedia.FirstOrDefaultAsync(x => x.IdPoi == poiId, ct);
        if (media is not null)
            return media;

        media = new PoiMedia { IdPoi = poiId };
        _db.PoiMedia.Add(media);
        await _db.SaveChangesAsync(ct);
        return media;
    }

    private async Task<IReadOnlyList<PoiSummaryResponse>> BuildSummaryResponsesAsync(
        IReadOnlyList<Poi> pois,
        string? lang,
        CancellationToken ct)
    {
        if (pois.Count == 0)
            return [];

        var poiIds = pois.Select(x => x.Id).ToList();
        var mediaLookup = await _db.PoiMedia.AsNoTracking()
            .Where(x => poiIds.Contains(x.IdPoi))
            .ToDictionaryAsync(x => x.IdPoi, ct);

        var languageLookup = await _db.PoiLanguages.AsNoTracking()
            .Where(x => poiIds.Contains(x.IdPoi))
            .GroupBy(x => x.IdPoi)
            .ToDictionaryAsync(x => x.Key, x => x.ToList(), ct);

        return pois.Select(poi =>
        {
            mediaLookup.TryGetValue(poi.Id, out var media);
            languageLookup.TryGetValue(poi.Id, out var languages);
            var preferred = ResolvePreferredLanguage(poi, languages ?? [], lang);

            return new PoiSummaryResponse
            {
                Id = poi.Id,
                Name = poi.Name,
                Description = poi.Description,
                Latitude = poi.Latitude,
                Longitude = poi.Longitude,
                RadiusMeters = poi.RadiusMeters,
                NearRadiusMeters = poi.RadiusMeters * 2,
                DebounceSeconds = 3,
                CooldownSeconds = poi.CooldownSeconds,
                Priority = null,
                IsActive = poi.IsActive,
                UpdatedAt = poi.UpdatedAt,
                Language = preferred?.LanguageTag ?? "vi-VN",
                NarrationText = preferred?.TextToSpeech,
                ImageUrl = media?.Image,
                AudioUrl = media?.Audio,
                MapLink = media?.MapLink
            };
        }).ToList();
    }

    private static PoiLanguage? ResolvePreferredLanguage(Poi poi, IReadOnlyCollection<PoiLanguage> languages, string? lang)
    {
        if (!string.IsNullOrWhiteSpace(lang))
        {
            var preferred = languages.FirstOrDefault(x => x.LanguageTag == lang);
            if (preferred is not null)
                return preferred;
        }

        return languages.FirstOrDefault(x => x.LanguageTag == "vi-VN")
            ?? languages.FirstOrDefault();
    }

    private static PoiLanguageResponse MapLanguage(PoiLanguage row) => new()
    {
        Id = row.IdLang,
        PoiId = row.IdPoi,
        LanguageTag = row.LanguageTag,
        TextToSpeech = row.TextToSpeech
    };

    private static string? CombineTts(string? narration, string? description)
    {
        var parts = new[] { narration?.Trim(), description?.Trim() }
            .Where(x => !string.IsNullOrWhiteSpace(x));
        var combined = string.Join(". ", parts);
        return string.IsNullOrWhiteSpace(combined) ? null : combined;
    }

    private async Task UpsertLanguageInternalAsync(int poiId, string languageTag, string? tts, CancellationToken ct)
    {
        var row = await _db.PoiLanguages
            .FirstOrDefaultAsync(x => x.IdPoi == poiId && x.LanguageTag == languageTag, ct);

        if (row is null)
        {
            _db.PoiLanguages.Add(new PoiLanguage
            {
                IdPoi = poiId,
                LanguageTag = languageTag,
                TextToSpeech = tts
            });
        }
        else
        {
            row.TextToSpeech = tts;
        }

        await _db.SaveChangesAsync(ct);
    }
}
