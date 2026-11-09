# 07 — Frontend, UX và PWA

## 1. Công nghệ và ngôn ngữ

- React **JavaScript**, Vite, CSS thuần.
- Không sử dụng TypeScript trừ khi có quyết định thay đổi.
- UI tiếng Việt; tên biến, component và API bằng tiếng Anh.
- Thiết kế mobile-first, tối ưu thao tác ngoài vườn bằng điện thoại.
- Dark/light mode; phản hồi rõ khi mất mạng, đang lưu hoặc thao tác thất bại.
- Hướng tới PWA, nhưng offline data sync không tự coi là có trong MVP.

## 2. Nhóm màn hình dự kiến

| Phase | Màn hình |
|---|---|
| P0 | Đăng nhập; Dashboard; Danh sách vườn; Sơ đồ vườn; Danh sách cây; Chi tiết cây; Sức khỏe/sâu bệnh; Quan sát cỏ dại; Công việc; Nhật ký chăm sóc; Nhắc việc; Thông báo; Nhật ký tưới; Thiết bị; Người dùng và thành viên |
| P1 | Mùa vụ; Thu hoạch; Bán hàng; Thanh toán/công nợ; Chi phí; Vật tư/tồn kho; Báo cáo; Sao lưu |
| P2 | Bảng điều khiển mô phỏng bơm; Cảm biến ảo; Lịch mô phỏng; Lịch sử cảnh báo |

## 3. Sơ đồ vườn

- Có thể sử dụng ảnh nền do gia đình cung cấp rồi tự đặt các điểm `PlantingSpots`.
- Các điểm tương đối không được thể hiện là GPS chính xác.
- Hiển thị mã `CC-001`...`CC-026`, tình trạng cây và vùng khi có dữ liệu.
- Khi cây thay thế, vị trí cũ không bị xóa; UI cho xem lịch sử các đời cây tại vị trí đó.
- Phân loại màu/tượng hình theo sức khỏe nhưng không chỉ dùng màu để truyền tải thông tin.

## 4. Quy tắc UX quan trọng

- Thao tác phá hủy cần xác nhận và giải thích ảnh hưởng.
- Màn hình báo rõ dữ liệu thiếu/chưa khảo sát, không đặt mặc định số 0 cho dữ liệu chưa biết.
- Bộ lọc theo vườn và vai trò phải rõ ràng.
- Trong P2, tất cả nút bơm ảo phải mang nhãn **MÔ PHỎNG**, không dùng từ ngữ khiến người xem tưởng bơm thật đang chạy.
- Với bản ghi chăm sóc, hiển thị người ghi và thời điểm để đối soát.

## 5. Quy trình thiết kế

1. Chốt user flow theo UC.
2. Wireframe mobile + desktop cho màn hình chính.
3. Component inventory (Button, Input, Dialog, Table, EmptyState...).
4. Design tokens CSS và dark/light.
5. Mock data không gán thành số liệu thực tế.
6. Kết nối API theo feature, xử lý loading/error/empty.
7. Kiểm tra màn hình điện thoại thật qua cùng mạng Wi-Fi.
