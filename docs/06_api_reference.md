# 06 API Reference

File nay la ban tom tat nhanh. Ban day du hon nam o:

- `docs/api/reference.md`

## Auth

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

## POI

- `GET /api/v1/pois`
- `GET /api/v1/pois/search`
- `GET /api/v1/pois/{id}`
- `POST /api/v1/pois`
- `PUT /api/v1/pois/{id}`
- `PATCH /api/v1/pois/{id}/status`
- `DELETE /api/v1/pois/{id}`

## Language / Narration

- `GET /api/v1/pois/{id}/languages`
- `PUT /api/v1/pois/{id}/languages/{languageTag}`
- `DELETE /api/v1/pois/{id}/languages/{languageTag}`
- `GET /api/v1/pois/{id}/narration?lang=...&eventType=...`

## Media

- `GET /api/v1/pois/{id}/media`
- `PUT /api/v1/pois/{id}/media/links`
- `POST /api/v1/pois/{id}/media/image`
- `POST /api/v1/pois/{id}/media/audio`

## History / Playback / Visit

- `POST /api/v1/history`
- `POST /api/v1/playbacks`
- `POST /api/v1/visits`
- `GET /api/v1/history/users/{userId}`
- `GET /api/v1/history/pois/{poiId}`

## Admin

- `GET /api/v1/admin/dashboard`
- `GET /api/v1/admin/seed/status`
- `POST /api/v1/admin/translate-all`

## Translator / QR

- `POST /api/v1/translator/translate`
- `GET /api/v1/qr/generate/{poiId}`

## Error response chuan

```json
{
  "code": "validation_error",
  "message": "Validation failed.",
  "details": {},
  "traceId": "..."
}
```
