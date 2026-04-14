# API Reference

## Auth

### `POST /api/v1/auth/register`

Request:

```json
{
  "username": "demo_user",
  "mail": "demo@example.com",
  "password": "secret123"
}
```

Response `200`:

```json
{
  "token": "jwt-token",
  "userId": "00000000-0000-0000-0000-000000000000",
  "username": "demo_user",
  "mail": "demo@example.com",
  "role": "user",
  "expiresAt": "2026-04-15T12:00:00Z"
}
```

### `POST /api/v1/auth/login`

Request:

```json
{
  "username": "demo_user",
  "password": "secret123"
}
```

### `GET /api/v1/auth/me`

Bearer token required.

## POI

### `GET /api/v1/pois`

Dung cho mobile.

Query:

- `lang` optional

Response `200`:

```json
[
  {
    "id": 1,
    "name": "Cho Ben Thanh",
    "description": "Bieu tuong lich su cua Sai Gon",
    "latitude": 10.77245,
    "longitude": 106.69806,
    "radiusMeters": 120,
    "nearRadiusMeters": 240,
    "debounceSeconds": 3,
    "cooldownSeconds": 30,
    "priority": null,
    "isActive": true,
    "updatedAt": "2026-04-14T12:00:00Z",
    "language": "vi-VN",
    "narrationText": "Noi dung narration",
    "imageUrl": "/images/example.png",
    "audioUrl": "/audio/example.mp3",
    "mapLink": "https://maps.google.com/?q=10.77245,106.69806"
  }
]
```

### `GET /api/v1/pois/search`

Query:

- `search`
- `isActive`
- `lang`
- `page`
- `pageSize`

Response `200`:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 0,
  "totalPages": 0
}
```

### `GET /api/v1/pois/{id}`

Query:

- `lang` optional

Response `200`:

```json
{
  "id": 1,
  "name": "Cho Ben Thanh",
  "description": "Bieu tuong lich su cua Sai Gon",
  "latitude": 10.77245,
  "longitude": 106.69806,
  "radiusMeters": 120,
  "nearRadiusMeters": 240,
  "debounceSeconds": 3,
  "cooldownSeconds": 30,
  "priority": null,
  "isActive": true,
  "updatedAt": "2026-04-14T12:00:00Z",
  "language": "vi-VN",
  "narrationText": "Noi dung narration",
  "imageUrl": "/images/example.png",
  "audioUrl": "/audio/example.mp3",
  "mapLink": "https://maps.google.com/?q=10.77245,106.69806",
  "media": {
    "poiId": 1,
    "imageUrl": "/images/example.png",
    "audioUrl": "/audio/example.mp3",
    "mapLink": "https://maps.google.com/?q=10.77245,106.69806"
  },
  "languages": [
    {
      "id": 1,
      "poiId": 1,
      "languageTag": "vi-VN",
      "textToSpeech": "Noi dung narration"
    }
  ]
}
```

### `POST /api/v1/pois`

Admin only.

Request:

```json
{
  "name": "Nha tho Duc Ba",
  "description": "Cong trinh Gothic noi bat",
  "latitude": 10.77993,
  "longitude": 106.69933,
  "radiusMeters": 120,
  "nearRadiusMeters": 240,
  "cooldownSeconds": 30,
  "debounceSeconds": 3,
  "priority": 10,
  "isActive": true,
  "narrationText": "Noi dung narration tieng Viet"
}
```

### `PUT /api/v1/pois/{id}`

Admin only. Same body as create.

### `PATCH /api/v1/pois/{id}/status`

Admin only.

Request:

```json
{
  "isActive": false
}
```

### `DELETE /api/v1/pois/{id}`

Admin only.

## POI Languages

### `GET /api/v1/pois/{id}/languages`

### `PUT /api/v1/pois/{id}/languages/{languageTag}`

Admin only.

Request:

```json
{
  "languageTag": "en-US",
  "textToSpeech": "English narration text"
}
```

### `DELETE /api/v1/pois/{id}/languages/{languageTag}`

Admin only.

## POI Media

### `GET /api/v1/pois/{id}/media`

### `PUT /api/v1/pois/{id}/media/links`

Admin only.

Request:

```json
{
  "imageUrl": "https://cdn.example.com/poi.jpg",
  "audioUrl": "https://cdn.example.com/poi.mp3",
  "mapLink": "https://maps.google.com/?q=10.77245,106.69806"
}
```

### `POST /api/v1/pois/{id}/media/image`

Admin only. Multipart form-data with `file`.

### `POST /api/v1/pois/{id}/media/audio`

Admin only. Multipart form-data with `file`.

## Narration

### `GET /api/v1/pois/{id}/narration`

Query:

- `lang`
- `eventType`

Response:

```json
{
  "poiId": 1,
  "eventType": 0,
  "language": "vi-VN",
  "narrationText": "Ban da den Cho Ben Thanh. Noi dung narration",
  "cached": true
}
```

Event type mapping:

- `enter` => `0`
- `near` => `1`
- `tap` => `2`
- `dwell` => `3`

## History / Playback / Visit

### `POST /api/v1/history`

Compatibility route cho mobile playback log.

### `POST /api/v1/playbacks`

### `POST /api/v1/visits`

Request:

```json
{
  "poiId": 1,
  "userId": "00000000-0000-0000-0000-000000000000",
  "durationSeconds": 120
}
```

### `GET /api/v1/history/users/{userId}`

Bearer token required.

### `GET /api/v1/history/pois/{poiId}`

Admin only.

## Translator

### `POST /api/v1/translator/translate`

Request:

```json
{
  "text": "Xin chao",
  "toLang": "en-US",
  "fromLang": "vi-VN"
}
```

Response:

`text/plain`

## Admin

### `GET /api/v1/admin/seed/status`

Admin only.

### `POST /api/v1/admin/translate-all?overwrite=false`

Admin only.

## QR

### `GET /api/v1/qr/generate/{poiId}`

Response la file PNG chua payload:

`smarttourism://poi/{poiId}`

## Error response

Mau loi chuan:

```json
{
  "code": "not_found",
  "message": "POI not found.",
  "details": null,
  "traceId": "00-..."
}
```
