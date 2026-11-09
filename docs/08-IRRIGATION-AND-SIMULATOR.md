# 08 — Hệ thống tưới thực tế và IoT Simulator

## 1. Hiện trạng hệ thống vật lý

- Vườn có **giếng khoan**, **máy bơm sử dụng điện 220V**, hệ thống **béc tưới cố định chung cho toàn vườn**.
- Có nhánh cấp nước sinh hoạt ngoài đời thực nhưng **không quản lý trong ứng dụng**.
- Chưa có thiết bị cảm biến, bộ điều khiển, relay, ESP32 hoặc MQTT được xác nhận sử dụng trong dự án.

## 2. P0 — Nhật ký tưới thực

`IrrigationSessions` ghi nhận hoạt động thực tế nhập tay, ví dụ ngày giờ, ghi chú, người thực hiện. `EquipmentAssets`/`EquipmentInspections` quản lý thông tin thiết bị và kiểm tra.

**Không suy đoán:** lượng nước, áp suất hay thời gian chạy nếu người dùng chưa đo/nhập thực tế.

## 3. P2 — Giả lập hoàn toàn bằng phần mềm

Gồm bảy bảng:

1. `IoTDevices` — thiết bị giả lập hoặc abstraction cho tương lai.
2. `IrrigationCommands` — lệnh logic mô phỏng.
3. `SimulationSessions` — phiên chạy/kịch bản.
4. `SimulationSensors` — cảm biến ảo.
5. `SimulationReadings` — các giá trị cảm biến ảo theo thời gian.
6. `SimulationSchedules` — lịch chạy logic.
7. `SimulationAlerts` — cảnh báo sự cố giả lập.

## 4. Kịch bản demo dự kiến

- Khởi động/tắt bơm ảo.
- Đặt lịch mô phỏng tự chạy khi backend đang hoạt động.
- Tăng/giảm độ ẩm đất giả lập theo công thức/kịch bản định trước.
- Phát sự kiện áp suất bất thường hoặc lưu lượng thấp **giả lập**.
- SignalR cập nhật UI realtime.
- Lưu lịch sử phiên, readings và alerts để trình bày demo.

## 5. Kiến trúc bảo vệ an toàn

```text
Frontend /api/simulator
       ↓
Service (phân quyền + validate + idempotency)
       ↓
Simulation Engine (chỉ tính toán dữ liệu)
       ├── Database Simulation*
       └── SignalR UI events
```

- Không đặt mã điều khiển GPIO, relay hay thiết bị 220V vào phiên bản demo này.
- Nếu sau này tích hợp thiết bị thật, phải có dự án/phase riêng về xác thực, fail-safe, giám sát, ngắt khẩn cấp và an toàn điện.
- API mô phỏng khác namespace/DTO với nhật ký tưới thủ công.
- Mọi giá trị độ ẩm/áp suất/lưu lượng từ P2 có `Source=Simulation` hoặc phân biệt ở bảng/endpoint.
- Với SQL Server, ngăn nhiều phiên Running trên cùng device bằng ràng buộc phù hợp.
- Lịch chạy server-side chỉ được đảm bảo khi tiến trình backend và job processor hoạt động; không hứa chạy 24/7 nếu máy tắt.
