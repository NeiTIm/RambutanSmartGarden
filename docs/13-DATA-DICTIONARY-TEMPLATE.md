# 13 — Mẫu từ điển dữ liệu chi tiết

**Mục đích:** lưu chính xác schema từng bảng trước khi viết Entity, EF Core configuration và migration.

**Quan trọng:** Blueprint Phase 01.6 đã duyệt phạm vi **60 bảng**, nhưng tài liệu đang có ở đây chưa chứa toàn văn từng trường của Phase 01.3. Do đó KHÔNG được tự diễn giải bản mẫu này thành bảng SQL đã phê duyệt.

## 1. Mẫu chuẩn cho mỗi bảng

```markdown
### [TableName]
- Phase: P0 / P1 / P2
- Feature: ...
- Mô tả: ...
- Primary Key: ...
- Business Rules: ...

| Column | SQL type | NULL | Default | PK/FK | Mô tả |
|---|---|---|---|---|---|
| Id | uniqueidentifier | NOT NULL | ... | PK | Khóa chính |
| ... | ... | ... | ... | ... | ... |

Indexes:
- UNIQUE (...)
- FILTERED UNIQUE (... WHERE ...)

Constraints:
- CHECK (...)

Delete behavior:
- FK ... → NoAction / Restrict / ...

Concurrency:
- rowversion có/không

Ví dụ request/response liên quan:
- ...

Quyết định duyệt:
- Người duyệt:
- Ngày:
```

## 2. Các cột quan trọng phải đặc tả kỹ

- `Users`: username chuẩn hóa, số điện thoại chuẩn hóa tùy chọn, password hash, trạng thái, audit.
- `GardenMemberships`: GardenId, UserId, Role, uniqueness.
- `Trees`: GardenId, TreeCode, PlantingSpotId, lifecycle, variety nullable khi chưa xác định.
- `PlantingSpots`: GardenId, khu vực tùy chọn, vị trí ảnh/bản đồ nếu có.
- `WeedObservations`: GardenId bắt buộc, GardenAreaId tùy chọn và cùng GardenId.
- `GardenTasks` / `CareActivities` / `TaskRecurrenceRules`: `Scope`, optional `GardenAreaId`, selected trees qua bảng nối.
- `Notifications`: RecipientUserId, EventKey và idempotency.
- `IrrigationSessions`: chỉ dữ liệu tưới thật nhập tay.
- `Harvests` / `SaleLines` / `HarvestLosses`: kg, provenance, trạng thái để đối soát.
- `SimulationReadings`: nguồn giả lập, device/sensor/session, timestamp.

## 3. Quy trình điền bảng

1. Mở đặc tả Phase 01.3 và quy tắc Phase 01.4 nguyên văn.
2. Điền tất cả cột và constraint của **một nhóm P0**.
3. Đối chiếu quan hệ với các bảng có phụ thuộc.
4. Kiểm tra quy tắc GardenId nhất quán.
5. Người dùng duyệt bảng dữ liệu, rồi mới viết Entity/Fluent API.
6. Database/migration chỉ chạy khi có phê duyệt tách biệt.

## 4. Checklist schema trước migration

- [ ] Tất cả cột có type, length, precision, nullability.
- [ ] FK và principal key tồn tại đúng phase.
- [ ] Unique/filtered index giải quyết các trùng lặp nghiệp vụ.
- [ ] Check constraint không xung đột giá trị mặc định.
- [ ] Delete behavior không gây cascade cycle/multiple paths.
- [ ] `DateTime` lưu UTC và không phát sinh sai timezone.
- [ ] Transaction cho thao tác nhiều bảng đã được thiết kế.
- [ ] Không đụng database của project khác.
- [ ] Đã được duyệt riêng việc tạo DB/migration.
