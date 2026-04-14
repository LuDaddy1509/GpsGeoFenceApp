# CHANGELOG PHASE 3

## Cap nhat code da ap dung trong repo

### File da sua / tao

- `Application/Services/GeofenceEventGate.cs`
- `Application/Services/Geofencing/GeofenceNarrationCoordinator.cs`
- `Application/Services/Api/PlaybackApiClient.cs`
- `Application/Configuration/ApiRoutes.cs`

### Hanh vi moi sau khi sua

- geofence gate co them suppress theo competing POI de giam trigger chong nhau
- stale session state duoc cleanup chu ky, giam lap event/session memory bi phinh
- geofence enter/dwell bat dau log visit rieng ve backend
- playback log gui ve endpoint ro hon cho playback thay vi dung route compatibility

### Rui ro con lai

- chua test tren Android that cho overlap geofence
- history model backend van la aggregate MVP, chua tach playback/visit table rieng

## Muc tieu

On dinh hoa geofence va narration de app co policy ro rang hon thay vi chi dung o muc demo.

## Thay doi chinh

### Geofence policy

- Chuan hoa rule chon POI khi overlap:
  - trong ban kinh hop le
  - priority cao hon thi thang
  - bang priority thi chon POI gan hon
- `AndroidGeofenceService` xu ly batch geofence transition va chon 1 POI thang
- Them TODO ro rang cho truong hop reboot-level re-register

### Event gate

- `GeofenceEventGate` khong con chi debounce/cooldown don gian
- Bo sung:
  - session debounce
  - cooldown
  - suppress duplicate cheo event type
  - session memory cho auto narration

### Narration

- `NarrationManager` da co policy uu tien:
  - manual > auto
  - auto khong cat manual
  - auto khong cat auto khac dang phat
  - duplicate session bi suppress
- Bo sung `Dwell` vao `PoiEventType`
- Fallback text va logging narration ro hon

### Coordinator

- Them `GeofenceNarrationCoordinator`
- Dua policy geofence + narration ra khoi `MapPage`
- Su dung state model:
  - `Idle`
  - `Near`
  - `Entered`
  - `Dwelling`
  - `Suppressed`
  - `ManuallyPlaying`

### Audio cache

- Bo sung in-flight dedupe de tranh download cung file audio nhieu lan
- Logging khi download thanh cong / that bai

### Map/runtime wiring

- `MapPage` dang ky lai geofence:
  - sau sync
  - khi app resume
  - khi page quay lai
- Sua bug tracking bi dung sau khi roi `MapPage` va quay lai
- `PoiSyncService` phat event `SyncCompleted` de flow runtime cap nhat geofence

## Tai lieu

- Them `docs/behavior/phase3-geofence-narration.md`

## Rang buoc con lai

- Chua build xac nhan mobile vi may hien tai thieu Android SDK (`XA5300`)
- Chua co boot receiver de phuc hoi geofence sau reboot
- Chua tach toan bo UI state sang ViewModel

## Self-review

### 5 diem lam tot

- policy geofence da ro, khong con trigger "may man"
- chon POI overlap da dung voi yeu cau san pham hon
- manual narration duoc uu tien dung cach
- event gate va audio cache giai quyet nhieu edge-case kho chiu
- da co coordinator de hut bot logic domain khoi UI

### 5 rui ro / diem yeu

- `MapPage` van can nang du da boc bot policy
- geofence/runtime behavior van phu thuoc manh vao Android thuc te
- narration model van suy ra ngan/dai tu event type, chua that su du lieu hoa
- chua co boot recovery cho geofence sau reboot
- mobile chua duoc verify end-to-end tren thiet bi that

### File co nguy co gay bug sau refactor

- `Application/Pages/MapPage.xaml.cs`
- `Application/Services/Geofencing/GeofenceNarrationCoordinator.cs`
- `Application/Services/Narration/NarrationManager.cs`
- `Application/Services/GeofenceEventGate.cs`
- `Application/Platforms/Android/Services/AndroidGeofenceService.cs`
- `Application/Platforms/Android/GeofenceBroadcastReceiver.cs`

### Next actions

- test tren Android that trong moi truong indoor/outdoor
- can nhac tach playback state ra khoi page-level orchestration them nua
- bo sung boot receiver neu can geofence song sau reboot
- neu backend cho phep, tach narration per event type ro hon
