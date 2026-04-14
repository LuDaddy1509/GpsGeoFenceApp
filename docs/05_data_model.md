# 05 Data Model

## Tong quan

He thong hien tai su dung:

- SQLite tren mobile cho local cache
- SQL Server tren backend cho du lieu van hanh

## Thuc the chinh backend

### `Pois`

Vai tro:

- bang goc cua dia diem
- chua ten, mo ta, toa do, radius, cooldown, active, time stamp

Duoc dung boi:

- mobile map
- geofence
- admin CRUD
- history linkage

### `PoiLanguage`

Vai tro:

- luu narration/noi dung theo tung ngon ngu
- hien dang la nguon cho narration da ngon ngu

Duoc dung boi:

- `GET /api/v1/pois/{id}/languages`
- `GET /api/v1/pois/{id}/narration`
- admin language form

### `PoiMedia`

Vai tro:

- luu image path/url
- luu audio path/url
- luu map link

Duoc dung boi:

- mobile POI detail
- admin media upload / media link

### `Users`

Vai tro:

- auth login/register
- nguoi dung mobile
- xac dinh role admin theo config hien tai

### `HistoryPoi`

Vai tro:

- log tong hop co ban theo user va POI
- so lan kich hoat/so lan nghe
- last visited
- tong duration

Luu y:

- hien la bang aggregate MVP
- playback va visit chua tach thanh bang event rieng

## Local mobile cache

### `Pois` local

- luu danh sach POI active sau sync

### `PoiNarrationCache`

- cache narration text theo `PoiId + EventType + LanguageTag`

### `SyncMetadata`

- luu thong tin last sync de hien sync status

## Quan he cap cao

- `Pois` 1 - n `PoiLanguage`
- `Pois` 1 - 1/n `PoiMedia` theo implementation hien tai
- `Users` 1 - n `HistoryPoi`
- `Pois` 1 - n `HistoryPoi`
