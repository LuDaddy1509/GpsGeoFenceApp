# 08 Test Checklist

## Startup

- [ ] App mo vao `StartupPage`
- [ ] Local SQLite duoc khoi tao
- [ ] Sync status hien thi duoc
- [ ] Permission status hien thi duoc
- [ ] Co session -> vao `MapPage`
- [ ] Khong co session -> vao `LoginPage`

## Login / Register

- [ ] Register user moi thanh cong
- [ ] Khong register duoc khi username/email trung
- [ ] Login dung thi vao map
- [ ] Login sai thi hien loi hop ly
- [ ] Logout quay ve login

## Sync

- [ ] Sync tay tu map/settings chay thanh cong
- [ ] Sau sync, POI local duoc cap nhat
- [ ] Sau sync, geofence duoc dang ky lai
- [ ] Sync status cap nhat last sync time

## GPS

- [ ] App xin quyen vi tri
- [ ] Khi cap quyen, map hien user location
- [ ] Khi tu choi quyen, app bao trang thai ro rang
- [ ] Tracking co the tiep tuc khi map dang mo

## Geofence

- [ ] Near chi goi y nhe, khong autoplay
- [ ] Enter phat narration ngan neu policy cho phep
- [ ] Dwell khong lap narration kho chiu
- [ ] Exit dua card/trang thai ve muc hop ly
- [ ] Khi nhieu POI overlap, chi 1 POI duoc chon

## Narration

- [ ] Manual play tu detail hoat dong
- [ ] Manual uu tien hon auto
- [ ] Audio cache duoc uu tien truoc TTS
- [ ] Offline ma co cache van nghe duoc
- [ ] Khong bi stop/start lien tuc khi event lap

## Map

- [ ] Hien POI pin tren map
- [ ] Current POI card hien/hidden hop ly
- [ ] Tu map vao duoc QR, Settings, POI detail
- [ ] Empty state hien dung khi chua co POI local

## QR

- [ ] Scan QR dung payload mo dung POI detail
- [ ] QR khong autoplay mac dinh
- [ ] QR quick play chi chay khi payload chi ro
- [ ] QR voi POI chua sync local hien thong bao hop ly

## Offline

- [ ] Tat mang sau khi da sync van xem map/detail co ban duoc
- [ ] Narration text cache van su dung duoc neu co
- [ ] Sync state bao ro dang offline
- [ ] QR mo POI local van dung duoc khi offline

## Admin

- [ ] Dang nhap admin vao duoc `/admin`
- [ ] Dashboard hien metric
- [ ] Tao POI moi thanh cong
- [ ] Sua POI thanh cong
- [ ] Bat/tat active thanh cong
- [ ] Upload anh thanh cong
- [ ] Upload audio thanh cong
- [ ] Them/sua/xoa ngon ngu thanh cong
- [ ] QR preview hien dung
- [ ] Logs theo POI tai duoc

## Regression

- [ ] Mobile van login va sync duoc sau refactor backend
- [ ] `GET /api/v1/pois` van dung cho mobile
- [ ] `GET /api/v1/pois/{id}/narration` van dung cho mobile
- [ ] `POST /api/v1/history` van dung cho mobile log
- [ ] Admin web khong lam vo API mobile flow
