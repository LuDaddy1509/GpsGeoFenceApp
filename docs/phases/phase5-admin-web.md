# Phase 5 Admin Web

## Muc tieu

Giai doan 5 tao mot admin web toi thieu nhung du ro de demo rang he thong co kha nang van hanh noi dung nhu mot san pham that.

Huong trien khai duoc chon:

- khong tao project frontend rieng
- dung admin web tinh gon trong `MapApi/wwwroot/admin`
- goi truc tiep API backend phase 4 tren cung origin

Ly do:

- giam chi phi wiring
- khong them stack moi vao repo
- de demo nhanh va de bao tri trong scope MVP

## Man hinh da co

### 1. Login admin

Muc dich:

- dang nhap bang tai khoan admin
- kiem tra role thong qua `GET /api/v1/auth/me`

Hanh vi:

- luu token vao `localStorage`
- neu user khong co role `admin` thi khong vao duoc admin shell

### 2. Dashboard

Noi dung:

- tong POI
- so POI active / inactive
- tong translation
- tong kich hoat
- tong so giay nghe
- top POI theo activation
- phu ngon ngu hien co

API dung:

- `GET /api/v1/admin/dashboard`
- `GET /api/v1/admin/seed/status`

### 3. POI List

Noi dung:

- tim kiem co ban
- loc active/inactive
- danh sach POI
- quick action:
  - sua
  - bat/tat active
  - xoa

API dung:

- `GET /api/v1/pois/search`
- `PATCH /api/v1/pois/{id}/status`
- `DELETE /api/v1/pois/{id}`

### 4. POI Form

Noi dung:

- tao moi POI
- sua POI
- cap nhat ten/mo ta/narration goc
- cap nhat toa do, radius, cooldown
- bat/tat active

API dung:

- `POST /api/v1/pois`
- `PUT /api/v1/pois/{id}`
- `GET /api/v1/pois/{id}`

### 5. Narration / Media

Noi dung:

- upload anh
- upload audio
- cap nhat image/audio/map link
- xem preview media
- them/sua/xoa ngon ngu narration

API dung:

- `POST /api/v1/pois/{id}/media/image`
- `POST /api/v1/pois/{id}/media/audio`
- `PUT /api/v1/pois/{id}/media/links`
- `PUT /api/v1/pois/{id}/languages/{languageTag}`
- `DELETE /api/v1/pois/{id}/languages/{languageTag}`

### 6. QR Preview

Noi dung:

- xem QR cua tung POI ngay trong form
- hien payload can quet

API dung:

- `GET /api/v1/qr/generate/{poiId}`

### 7. Logs

Noi dung:

- chon POI
- xem tong quan:
  - so lan kich hoat
  - tong giay nghe
  - so dong history
- xem chi tiet log row theo POI

API dung:

- `GET /api/v1/history/pois/{poiId}`

## UX va ky thuat da ap dung

- giao dien 1 trang, dieu huong bang nav noi bo
- loading state co ban o dashboard, list, form, logs
- error handling co ban bang global status bar
- login state tach rieng khoi app shell
- khong them CMS workflow phuc tap
- khong them rich text editor, drag-drop, versioning

## Endpoint backend bo sung de ho tro admin

- them `GET /api/v1/admin/dashboard`
- them redirect `GET /admin` -> `/admin/index.html`

## Cach demo

1. Chay `MapApi`
2. Mo `/admin`
3. Dang nhap bang tai khoan admin
4. Vao Dashboard de xem tong quan
5. Vao POI List de chon mot POI
6. Vao POI Form de sua noi dung, upload media, them language
7. Xem QR preview
8. Vao Logs de xem thong ke co ban theo POI

## Muc do hoan thien

### Day du cho demo

- login admin
- dashboard co metric
- poi list
- create/edit/delete POI
- bat/tat active
- upload anh
- upload audio
- cap nhat media link
- quan ly narration da ngon ngu
- xem QR
- xem logs co ban theo POI

### Van o muc demo/MVP

- chua co pagination UI chi tiet cho list
- chua co auto-refresh theo realtime
- chua co rich validation tren client ngoai scope co ban
- chua co workflow role/user management
- chua co dashboard chart nang cao
- chua co batch actions cho nhieu POI
