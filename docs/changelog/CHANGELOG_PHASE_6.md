# CHANGELOG PHASE 6

## Cap nhat bo sung theo code thuc te

### File da cap nhat / su dung de tong hop

- `README.md`
- `docs/architecture/phase1-scope-cleanup.md`
- `docs/product/phase2-core-flow.md`
- `docs/behavior/phase3-geofence-narration.md`
- `docs/api/phase4-backend.md`
- `docs/admin/phase5-admin-web.md`
- `docs/review/tech-lead-review-all-phases.md`

### Gia tri cho demo / nop do an

- tai lieu da gan sat hon voi code hien co sau cac thay doi startup/QR/geofence/backend/admin
- changelog va review giup doi chieu file code da sua truoc khi build/demo

## Muc tieu

Hoan thien mat tai lieu va quy trinh de du an san sang demo / nop do an.

## Tai lieu moi da tao

- `docs/01_problem_and_goal.md`
- `docs/02_scope.md`
- `docs/03_architecture.md`
- `docs/04_core_business_rules.md`
- `docs/05_data_model.md`
- `docs/06_api_reference.md`
- `docs/07_qr_strategy.md`
- `docs/08_test_checklist.md`
- `docs/09_known_issues.md`
- `docs/10_demo_script.md`

## Cap nhat them

- tao `README.md` moi de phan anh dung san pham sau refactor

## Gia tri cua phase 6

- repo co bo tai lieu gon, de doc, de trinh bay
- test checklist ro de tu kiem tra truoc demo
- known issues duoc ghi minh bach
- demo script co fallback QR neu geofence tai cho khong on dinh
- README du de nguoi doc moi hieu du an trong vai phut

## Self-review

### 5 diem lam tot

- flow mobile da ro hon truoc: startup, auth, map, QR, detail, settings
- geofence/narration da co policy ro, giam bot hanh vi lap va stop/start kho chiu
- backend da tach ra controller/service/dto thay vi de business logic nam lan trong `Program.cs`
- admin web du toi thieu nhung da demo duoc kha nang van hanh noi dung that
- bo docs 01-10 dung du cho demo, nop do an va handoff nhanh

### 5 rui ro / diem yeu

- `MapPage.xaml.cs` van con la file orchestration lon, de phat sinh bug state/runtime
- admin web tinh trong `wwwroot/admin` de demo nhanh nhung state management va validation con mong
- backend history van dung `HistoryPoi` aggregate cho ca playback va visit, de mo rong analytics se bi can
- role admin hien van suy ra tu config, chua phai auth model ben vung
- mobile Android chua build/verify end-to-end tren may hien tai vi thieu Android SDK

### File co nguy co gay bug sau refactor

- `Application/Pages/MapPage.xaml.cs`
- `Application/Services/Geofencing/GeofenceNarrationCoordinator.cs`
- `Application/Services/Narration/NarrationManager.cs`
- `Application/Platforms/Android/Services/AndroidGeofenceService.cs`
- `MapApi/Program.cs`
- `MapApi/Services/PoiManagementService.cs`
- `MapApi/wwwroot/admin/app.js`

### Next actions

- tach them state khoi `MapPage` sang ViewModel/coordinator de giam code-behind
- verify mobile tren Android that, nhat la geofence, audio cache va permission flow
- tach role admin ra khoi config sang DB/role table
- tach playback/visit logs thanh model rieng neu can analytics thuyet phuc hon
- bo sung smoke test cho backend API va checklist manual cho admin web
