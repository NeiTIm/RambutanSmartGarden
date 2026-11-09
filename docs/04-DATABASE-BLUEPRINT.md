# 04 — Database Blueprint: 60 bảng

## Trạng thái phê duyệt

Database Blueprint Phase 01.6 với **60 bảng** đã được người dùng duyệt về phạm vi và nguyên tắc quan hệ. Đây là **danh mục bảng + nguyên tắc thiết kế**, không phải SQL migration đã chạy.

- **P0:** 34 bảng.
- **P1:** 19 bảng.
- **P2:** 7 bảng.

## 1. Danh mục đầy đủ

| STT | Tên bảng | Phase | Mục đích |
|---:|---|---|---|
| 01 | `Users` | P0 | Tài khoản người dùng |
| 02 | `UserSessions` | P0 | Phiên đăng nhập và thu hồi |
| 03 | `PasswordResetRecords` | P0 | Lịch sử đặt lại mật khẩu do Admin xử lý |
| 04 | `AuditLogs` | P0 | Nhật ký thay đổi quan trọng |
| 05 | `GardenMemberships` | P0 | Phân quyền người dùng theo vườn |
| 06 | `Gardens` | P0 | Hồ sơ vườn |
| 07 | `GardenAreas` | P0 | Khu vực thuộc vườn |
| 08 | `PlantSpecies` | P0 | Loài cây |
| 09 | `TreeVarieties` | P0 | Giống cây |
| 10 | `PlantingSpots` | P0 | Vị trí trồng qua các đời cây |
| 11 | `Trees` | P0 | Cá thể cây và mã cây |
| 12 | `TreeReplacements` | P0 | Lịch sử thay cây |
| 13 | `TreeHealthObservations` | P0 | Quan sát sức khỏe cây |
| 14 | `PestIssues` | P0 | Sự cố sâu bệnh |
| 15 | `PestTreatments` | P0 | Biện pháp xử lý sâu bệnh |
| 16 | `WeedObservations` | P0 | Quan sát cỏ dại theo vườn hoặc khu |
| 17 | `GardenTasks` | P0 | Nhiệm vụ cần làm |
| 18 | `TaskTrees` | P0 | Cây được chọn cho nhiệm vụ |
| 19 | `TaskParticipants` | P0 | Người tham gia nhiệm vụ |
| 20 | `TaskRecurrenceRules` | P0 | Quy tắc công việc lặp |
| 21 | `CareActivities` | P0 | Nhật ký chăm sóc thực tế |
| 22 | `CareActivityTrees` | P0 | Cây gắn với lần chăm sóc |
| 23 | `TaskReminders` | P0 | Lịch nhắc việc |
| 24 | `Notifications` | P0 | Thông báo trong ứng dụng |
| 25 | `PushSubscriptions` | P1 | Đăng ký Web Push |
| 26 | `RecurrenceRuleTrees` | P0 | Cây được chọn cho mẫu lặp |
| 27 | `IrrigationSessions` | P0 | Phiên tưới thực tế ghi thủ công |
| 28 | `EquipmentAssets` | P0 | Bơm, béc, dụng cụ và thiết bị |
| 29 | `EquipmentInspections` | P0 | Kiểm tra thiết bị |
| 30 | `GrowingSeasons` | P1 | Mùa vụ |
| 31 | `TreeGrowthObservations` | P1 | Quan sát sinh trưởng |
| 32 | `Harvests` | P1 | Đợt thu hoạch |
| 33 | `HarvestTreeDetails` | P1 | Sản lượng thu hoạch theo cây |
| 34 | `HarvestLosses` | P1 | Hao hụt sau thu hoạch |
| 35 | `Buyers` | P1 | Người mua |
| 36 | `Sales` | P1 | Giao dịch bán |
| 37 | `SaleLines` | P1 | Dòng sản phẩm bán và nguồn thu hoạch |
| 38 | `SalePayments` | P1 | Thanh toán và công nợ |
| 39 | `ExpenseCategories` | P1 | Danh mục chi phí |
| 40 | `Expenses` | P1 | Khoản chi |
| 41 | `Materials` | P1 | Vật tư |
| 42 | `StockMovements` | P1 | Nhập/xuất kho |
| 43 | `MaterialUsages` | P1 | Sử dụng vật tư |
| 44 | `MediaAssets` | P0 | Tệp ảnh và siêu dữ liệu lưu trữ |
| 45 | `TreeMedia` | P0 | Ảnh liên kết cây |
| 46 | `TreeHealthMedia` | P0 | Ảnh quan sát sức khỏe |
| 47 | `PestIssueMedia` | P0 | Ảnh vấn đề sâu bệnh |
| 48 | `WeedObservationMedia` | P0 | Ảnh cỏ dại |
| 49 | `CareActivityMedia` | P0 | Ảnh hoạt động chăm sóc |
| 50 | `HarvestMedia` | P1 | Ảnh thu hoạch |
| 51 | `ExpenseMedia` | P1 | Ảnh chi phí/chứng từ |
| 52 | `SaleMedia` | P1 | Ảnh giao dịch bán |
| 53 | `IoTDevices` | P2 | Thiết bị mô phỏng/adapter tương lai |
| 54 | `IrrigationCommands` | P2 | Lệnh tưới trong mô phỏng |
| 55 | `SimulationSessions` | P2 | Phiên mô phỏng |
| 56 | `SimulationSensors` | P2 | Cảm biến mô phỏng |
| 57 | `SimulationReadings` | P2 | Chỉ số giả lập |
| 58 | `SimulationSchedules` | P2 | Lịch hoạt động mô phỏng |
| 59 | `SimulationAlerts` | P2 | Cảnh báo giả lập |
| 60 | `BackupJobs` | P1 | Bản ghi công việc sao lưu |

## 2. Các nhóm liên hệ chính

### Nhân sự và quyền

`Users` → `UserSessions`, `PasswordResetRecords`, `AuditLogs`; `Users` ↔ `Gardens` thông qua `GardenMemberships` (vai trò theo vườn).

### Vườn và cây

`Gardens` → `GardenAreas` → `PlantingSpots` (khu vực là tùy chọn tùy thiết kế); `PlantingSpots` → nhiều đời `Trees`; `TreeReplacements` ghi liên kết cây cũ ↔ cây mới. `PlantSpecies` và `TreeVarieties` phục vụ đa loài/đa giống.

### Quan sát và chăm sóc

`Trees` → `TreeHealthObservations`; `PestIssues` → `PestTreatments`; `WeedObservations` thuộc vườn, có thể thuộc khu vực. `GardenTasks` ↔ `Trees` thông qua `TaskTrees`; `TaskRecurrenceRules` ↔ `Trees` qua `RecurrenceRuleTrees`; `CareActivities` ↔ `Trees` qua `CareActivityTrees`.

### Truyền thông

Một `MediaAssets` dùng các bảng liên kết có FK rõ ràng như `TreeMedia`, `TreeHealthMedia`, `PestIssueMedia`, `WeedObservationMedia`, `CareActivityMedia`, `HarvestMedia`, `ExpenseMedia`, `SaleMedia`.

### Sản lượng và tiền

`GrowingSeasons` → `Harvests` → `HarvestTreeDetails` / `HarvestLosses`; `Sales` → `SaleLines` → tham chiếu `Harvests` khi biết nguồn; `Sales` → `SalePayments`; `Expenses` → `ExpenseCategories`.

### Vật tư

`Materials` → `StockMovements`, `MaterialUsages`. **Chưa có MaterialBatches** nên chưa cam kết đối soát hạn dùng theo lô.

### Mô phỏng

`IoTDevices` liên hệ lệnh/phiên/lịch; `SimulationSessions` → `SimulationReadings`; `SimulationSensors` → `SimulationReadings`; `SimulationAlerts` ghi sự kiện bất thường.

## 3. Thứ tự migration dự kiến — chỉ lập kế hoạch

| Phase | Bảng cần làm | Phụ thuộc |
|---|---|---|
| P0.1 | Users, UserSessions, PasswordResetRecords, AuditLogs, Gardens, GardenMemberships | Foundation |
| P0.2 | PlantSpecies, TreeVarieties, GardenAreas, PlantingSpots, Trees, TreeReplacements | P0.1 |
| P0.3 | TreeHealthObservations, PestIssues, WeedObservations, MediaAssets và link ảnh liên quan | P0.2 |
| P0.4 | GardenTasks, TaskTrees, TaskParticipants, TaskRecurrenceRules, RecurrenceRuleTrees, CareActivities, CareActivityTrees, TaskReminders, Notifications, PestTreatments | P0.3 |
| P0.5 | IrrigationSessions, EquipmentAssets, EquipmentInspections | P0.4 theo phụ thuộc chọn |
| P1 | PushSubscriptions, mùa vụ, tài chính, vật tư, media liên quan, BackupJobs | P0 |
| P2 | Bảy bảng IoT simulator | P0/P1 phù hợp |

**Lưu ý vòng phụ thuộc:** `PestTreatments` có thể liên hệ `CareActivities`. Vì `CareActivities` nằm ở P0.4, cần tạo FK ở P0.4 hoặc dời tạo bảng `PestTreatments` sang P0.4. Không tạo FK trỏ đến bảng chưa tồn tại.

## 4. Kiểu dữ liệu quy ước (không thay bảng từ điển cột)

- PK/FK: `uniqueidentifier` với Guid.
- Ngày giờ: `datetime2(3)` lưu UTC; ngày thuần: `date`.
- Tiền: `decimal(18,2)`; kg: `decimal(18,3)`.
- Mã chuẩn hoá: `varchar` theo chiều dài cụ thể; tiếng Việt: `nvarchar`.
- Thay đổi đồng thời: `rowversion` trên entity chỉnh sửa nhiều.
- Delete FK: ưu tiên `NoAction` để tránh xóa dây chuyền lịch sử.

## 5. Điều chưa đủ thông tin để khẳng định

Tài liệu tóm tắt ở đây **không thay thế toàn văn Phase 01.3**: chưa có tất cả tên cột, nullable, default value, độ dài, enum/status của từng bảng đã duyệt. Khi bắt đầu code Entities/Fluent API, phải đối chiếu bản Phase 01.3 nguyên văn hoặc duyệt lại bản từ điển dữ liệu chi tiết. Không tự coi schema do suy đoán là schema đã phê duyệt.
