# CHANGELOG PHASE 5

## Cap nhat code da ap dung trong repo

### File da sua / tao

- `MapApi/wwwroot/admin/app.js`

### Hanh vi moi sau khi sua

- login admin co validate rong va khoa nut submit trong luc goi API
- form POI co validate client-side cho ten, lat/lng, radius, near radius, cooldown
- upload image/audio co validate type file co ban truoc khi gui
- thao tac save POI co busy state de giam submit lap
- save language bat buoc co noi dung, tranh tao ban dich rong khi demo

### Rui ro con lai

- admin van la web tinh mot file JS lon
- validation hien chi o muc demo, chua thay the validation backend

## Muc tieu

Them admin web toi thieu nhung du ro de demo kha nang van hanh noi dung cua he thong.

## Thay doi chinh

### Admin web moi

Them admin web tinh trong:

- `MapApi/wwwroot/admin/index.html`
- `MapApi/wwwroot/admin/styles.css`
- `MapApi/wwwroot/admin/app.js`

### Man hinh da co

- Login admin
- Dashboard
- POI List
- POI Form
- Narration / Media
- Logs

### Chuc nang da co

- dang nhap admin bang JWT
- kiem tra role admin qua `/api/v1/auth/me`
- xem dashboard metric tong quan
- tim / loc POI
- tao / sua / xoa POI
- bat / tat active
- upload anh
- upload audio
- cap nhat image/audio/map link
- them / sua / xoa narration da ngon ngu
- xem QR preview cua POI
- xem logs co ban theo POI

### Backend ho tro them

- them `GET /api/v1/admin/dashboard`
- them redirect `/admin` -> `/admin/index.html`

## Rang buoc con lai

- logs hien tai dung tren `HistoryPoi`, chua tach playback/visit dashboard rieng
- giao dien la admin web tinh, khong co build pipeline frontend rieng
- chua co user management / role management trong UI
- chua co chart nang cao, chi co metric/list de de demo

## Self-review

### 5 diem lam tot

- chon huong dat admin trong `MapApi/wwwroot/admin` la hop ly, re va nhanh
- bo man hinh da du de demo van hanh noi dung that
- admin da noi duoc vao backend phase 4, khong phai mock
- QR preview va media/language la dung thu hoi dong muon thay
- backend van build pass sau khi them admin

### 5 rui ro / diem yeu

- `app.js` gom qua nhieu state/UI logic trong mot file
- khong co framework/frontend build pipeline nen maintainability co han
- validation client-side va UX error handling con mong
- logs chi o muc co ban, chua thuyet phuc neu hoi ky ve analytics
- auth/session admin luu localStorage o muc demo, chua phai cach cung cap san pham that

### File co nguy co gay bug sau refactor

- `MapApi/wwwroot/admin/app.js`
- `MapApi/wwwroot/admin/index.html`
- `MapApi/Controllers/AdminController.cs`
- `MapApi/Program.cs`

### Next actions

- neu can di xa hon, tach admin web thanh frontend co cau truc hon
- bo sung validation va feedback UI cho form admin
- bo sung user/role management neu muon trinh bay van hanh day du hon
- can nhac dashboard logs ro hon neu hoi dong hoi sau ve usage
