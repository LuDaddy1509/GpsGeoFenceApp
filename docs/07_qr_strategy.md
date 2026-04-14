# 07 QR Strategy

## Vai tro cua QR

QR khong phai tinh nang phu. Trong ban hien tai, no dong 3 vai tro:

- cach mo POI nhanh va on dinh
- fallback demo khi geofence tai cho khong on dinh
- cach lien ket noi dung vat ly tai dia diem voi app

## Payload chuan

Payload hien tai:

- `smarttourism://poi/{poiId}`

Quick play la mo rong tuy chon, vi du:

- query `mode=quickplay`
- hoac payload co co quick play ro rang

## QR flow

1. user mo `QrScanPage`
2. scan payload
3. app resolve `poiId`
4. app tim POI trong local database
5. neu co -> mo `PoiDetailPage`
6. nguoi dung manual play/open map/chi duong tu detail

## Rule hien tai

- QR khong autoplay mac dinh
- chi quick play khi payload noi ro
- neu POI chua co offline local data, app bao can sync truoc
- QR khong lam vo geofence flow; no di vao detail flow

## Vi sao quan trong khi demo

Neu demo trong phong hoc/hoi dong, GPS va geofence co the:

- lech vi tri
- bi indoor attenuation
- khong duoc Android cho update on dinh

Khi do, QR la phuong an fallback de van demo duoc:

- mo dung POI
- nghe narration
- xem media
- chung minh backend/admin da tao noi dung that
