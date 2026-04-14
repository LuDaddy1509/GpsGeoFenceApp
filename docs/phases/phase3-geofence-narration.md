# Phase 3 Geofence and Narration

## Muc tieu

Giai doan 3 nang geofence va narration tu muc "demo chay duoc" len muc "on dinh, co policy ro rang, de du doan hon".

Pham vi thay doi tap trung vao:

- `AndroidGeofenceService`
- `GeofenceEventGate`
- `AndroidLocationService`
- `NarrationManager`
- `AudioCache`
- `MapPage`
- coordinator moi `GeofenceNarrationCoordinator`

## Policy moi da ap dung

### 1. Chon POI khi nhieu POI canh tranh

Rule chon POI da duoc chuan hoa trong `PoiProximitySelector`:

- chi xet POI nam trong ban kinh hop le
- uu tien `Priority` cao hon
- neu bang nhau thi uu tien POI gan hon
- neu van bang nhau thi giu on dinh theo thu tu ID

Ap dung:

- `Near` dung `NearRadiusMeters`
- geofence overlap dung `RadiusMeters`

Ket qua:

- tranh trigger dong thoi nhieu POI khi geofence overlap
- rule chon POI nhat quan giua location polling va geofence transition

### 2. Gate event ro hon

`GeofenceEventGate` hien co 3 lop bao ve:

- debounce theo tung `poi + eventType`
- cooldown theo tung `poi + eventType`
- suppress duplicate cheo event type cho cac event tu dong nhu `NEAR`, `ENTER`, `DWELL`

Ngoai ra, gate co session memory cho narration:

- neu cung POI vua phat narration trong mot khoang nho thi `auto narration` se bi suppress
- memory nay ap dung de tranh `ENTER` vua phat xong lai den `DWELL` phat tiep
- manual play cung duoc ghi nhan vao memory de giam lap lai auto ngay sau do

### 3. State model ro hon

Coordinator moi su dung model trang thai sau:

- `Idle`
- `Near`
- `Entered`
- `Dwelling`
- `Suppressed`
- `ManuallyPlaying`

Y nghia:

- `Near`: app chi goi y nhe, khong ep narration
- `Entered`: user vua di vao vung POI
- `Dwelling`: user o on dinh trong vung lau hon
- `Suppressed`: event co xay ra nhung policy khong cho phat de tranh gay phien
- `ManuallyPlaying`: uu tien trai nghiem nguoi dung khi ho tu bam nghe

### 4. Policy narration

`NarrationManager` da duoc chuan hoa lai theo huong predictability:

- `manual` uu tien cao hon `auto`
- `auto` khong cat narration manual dang phat
- `auto` khong cat `auto` khac dang phat de tranh stop/start lien tuc
- duplicate cung `POI + event + language` trong cung session se bi bo qua
- audio cache duoc uu tien truoc
- neu khong co audio cache/download duoc thi fallback TTS
- `ENTER` va `DWELL` dung narration ngan/muc auto
- `TAP` dung manual playback va co the xem la narration dai/hands-on

### 5. Re-register geofence

Geofence hien duoc dang ky lai o cac thoi diem:

- sau sync thanh cong
- khi `MapPage` xuat hien lai
- khi app resume
- khi reload lai danh sach POI local

Luu y:

- hien chua co `BOOT_COMPLETED receiver`
- trong code da dat TODO cho phase sau neu can song sau reboot

## Luong hoat dong hien tai

### Near

1. `TrackLoopAsync` lay GPS hien tai
2. `GeofenceNarrationCoordinator.EvaluateNearby(...)` chon POI tot nhat
3. UI hien current POI card va toast nhe neu can
4. khong auto narration

### Enter

1. Android geofence broadcast tra ve danh sach geofence overlap
2. `AndroidGeofenceService` chon 1 POI thang theo policy priority + distance
3. `GeofenceEventGate` quyet dinh co nhan event hay khong
4. `GeofenceNarrationCoordinator` cap nhat state `Entered`
5. neu khong bi suppress thi phat auto narration ngan

### Dwell

1. Android geofence phat `DWELL`
2. gate tiep tuc loc duplicate/cooldown
3. coordinator cap nhat state `Dwelling`
4. chi phat narration neu session memory cho phep

### Manual / Detail

1. User vao `PoiDetailPage`
2. user bam nghe hoac QR quick play vao detail
3. coordinator danh dau `ManuallyPlaying`
4. `NarrationManager` cho manual override auto neu can
5. auto event den sau do trong cua so uu tien manual se bi suppress

## Logging da bo sung

Da them logging muc vua du de debug:

- geofence register / clear / selected winner
- event gate accept / reject + ly do
- narration start / suppress + priority
- audio cache download success / fail
- location tracking start / stop
- map geofence re-register sau sync/resume

## Bug va rui ro da giam

### Da giam

- trigger nhieu POI cung luc khi geofence overlap
- chon sai POI do priority truoc day di nguoc yeu cau
- `ENTER` / `DWELL` / `NEAR` lap narration kho chiu
- manual play bi auto cat ngang
- stop/start narration lien tuc gay giat
- geofence khong duoc dang ky lai sau khi map quay lai hoac app resume
- tracking bi dung sau khi roi `MapPage` va quay lai
- audio download trung lap cho cung mot URL

### Van con gia dinh

- `Priority` lon hon nghia la uu tien cao hon
- narration ngan/dai hien duoc suy ra tu event type (`ENTER`/`DWELL` vs `TAP`) vi model hien tai chua tach truong duration ro rang
- `DWELL` co the dung chung nguon narration voi logic narration hien co neu backend chua co noi dung rieng
- Android geofence overlap co `TriggeringLocation`; neu mot so thiet bi tra thong tin khong day du, service se fallback theo `Priority`
- page level resume dang la diem dang ky lai geofence chinh; reboot-level persistence duoc de lai cho phase sau

## Ghi chu thuc thi

Refactor nay co y giam coupling giua UI va domain logic:

- `MapPage` khong con tu fetch narration text va tu quyet policy autoplay
- policy tap trung ve `GeofenceNarrationCoordinator`
- gate event va narration manager moi giai quyet phan lon edge-case stability
