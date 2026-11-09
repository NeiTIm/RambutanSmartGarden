# 05 — Quy tắc nghiệp vụ và toàn vẹn dữ liệu

## 1. Định danh, khóa và thời gian

- Mỗi bản ghi chính có khóa `Id` kiểu `Guid`, trừ khi dữ liệu liên kết thích hợp dùng PK ghép.
- Mọi FK phải trỏ đến bản ghi hợp lệ, cùng phạm vi vườn nếu đó là ràng buộc nghiệp vụ.
- Thời điểm lưu UTC (`datetime2(3)`); giao diện hiển thị `Asia/Ho_Chi_Minh`.
- Dùng `rowversion` và xử lý HTTP `409 Conflict` khi có cập nhật xung đột.
- Hạn chế cascade delete; ưu tiên chặn xóa khi còn lịch sử quan trọng.

## 2. Tài khoản và quyền

- `NormalizedUsername` duy nhất; số điện thoại chuẩn hóa duy nhất khi không NULL.
- Chỉ Admin tạo tài khoản/đặt lại mật khẩu; không có public register/email reset.
- Không lưu mật khẩu rõ ràng. Hash bằng thuật toán và thư viện phù hợp.
- Bootstrap Admin đầu tiên bằng cơ chế riêng, không commit mật khẩu mặc định.
- Kiểm soát membership theo `GardenId` ở mọi service/query liên quan đến vườn.

## 3. Cây và vị trí trồng

- `TreeCode` duy nhất `(GardenId, TreeCode)` **kể cả cây đã chết**.
- Một `PlantingSpot` tại một thời điểm chỉ có tối đa một cây đang sống; dùng filtered unique index nếu schema cho phép.
- `TreeReplacements` liên kết cây cũ với cây mới; không được là cùng một cây.
- Không ghi đè ngày trồng/giống/vị trí chưa xác nhận bằng dữ liệu giả.
- Phải kiểm tra cùng `GardenId` giữa vị trí, cây cũ và cây mới trong cùng transaction.

## 4. Scope vườn, khu và cây

Áp dụng cho `GardenTasks`, `CareActivities`, `TaskRecurrenceRules`:

| Scope | Nguyên tắc |
|---|---|
| `Garden` | Áp dụng toàn vườn; không cần danh sách cây |
| `GardenArea` | Bắt buộc có `GardenAreaId` hợp lệ và thuộc đúng vườn |
| `SelectedTrees` | Có danh sách cây cụ thể, mỗi cây thuộc đúng vườn |

`WeedObservations`: luôn thuộc vườn, `GardenAreaId` tùy chọn để thể hiện quan sát theo khu vực.

## 5. Công việc, quy tắc lặp và thông báo

- Nhiệm vụ dự kiến và nhật ký chăm sóc thực tế là hai sự kiện khác nhau.
- Với recurrence: `RecurrenceRuleTrees` lưu mẫu cây; `TaskTrees` lưu cây của từng lần phát sinh.
- Ngăn tạo trùng occurrence bằng unique index filtered trên `(RecurrenceRuleId, OccurrenceDueAt)` nếu mô hình cột tương ứng có sẵn.
- `TaskReminders` và `Notifications` cần khóa chống gửi trùng; đề xuất unique `(RecipientUserId,EventKey)` filtered khi có EventKey.
- Background job phải kiểm tra idempotency và xử lý restart; không dựa vào đồng hồ của client.

## 6. Tưới và thiết bị

- `IrrigationSessions` là phiên tưới **thực tế, nhập bằng tay**, không suy ra từ mô phỏng.
- Bơm và các loại dụng cụ cùng thuộc `EquipmentAssets`; phân loại theo loại tài sản.
- Nếu mỗi `CareActivity` tối đa một `IrrigationSession`, thể hiện bằng unique index filtered trên FK tùy chọn.

## 7. Thu hoạch, bán và công nợ

- Lưu kg ở `decimal(18,3)`; giá/tiền ở `decimal(18,2)`.
- Không xây kho trái cây riêng khi chưa có quyết định mở rộng.
- Sản lượng còn lại = sản lượng thu hoạch - phần bán đã xác nhận, phân bổ đúng nguồn - hao hụt; chỉ báo nếu dữ liệu đủ đối soát.
- Phải chống xuất bán vượt sản lượng khả dụng trong transaction.
- Khi không rõ `HarvestId` của một dòng bán, không tự phân bổ vào một đợt thu hoạch tùy ý.
- Thanh toán có trạng thái hợp lệ/void; không xóa vật lý chứng từ kế toán quan trọng.

## 8. Vật tư

- `StockMovements` ghi nhận mọi lần nhập/xuất/điều chỉnh; không tự sửa tồn kho thành giá trị tách khỏi lịch sử.
- `MaterialUsages` cho biết vật tư đã dùng trong công việc chăm sóc.
- Tránh tính trùng chi phí: mua vật tư là dòng tiền, sử dụng vật tư là tiêu hao tồn kho, không tự cộng thành hai chi phí tiền mặt.
- Chưa có bảng theo lô (`MaterialBatches`), vì vậy chưa bảo đảm FIFO/FEFO hay hạn dùng theo nhiều lô.

## 9. Ảnh và tệp đính kèm

- Tách metadata `MediaAssets` khỏi các bảng liên kết nghiệp vụ có FK.
- Kiểm soát quyền xem/xóa file theo đối tượng và vườn, không chỉ dựa vào URL.
- Validate loại tệp, kích thước, nội dung thực; không tin MIME do client gửi.

## 10. IoT simulator

- Một phiên đang chạy cho mỗi device: unique filtered index theo trạng thái Running nếu cần.
- Lệnh mô phỏng có idempotency key khi dễ gửi lặp.
- Báo cáo phải phân biệt thực tế / giả lập cả ở backend lẫn UI.
- Tuyệt đối không dùng một thao tác mô phỏng để cấp điện cho bơm vật lý.
