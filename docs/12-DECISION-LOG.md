# 12 — Nhật ký quyết định (Decision Log)

## 1. Các quyết định đã chốt

| ID | Quyết định | Ghi chú |
|---|---|---|
| D-001 | Tên sản phẩm `RambutanSmartGarden` | Dự án độc lập |
| D-002 | Dùng cho vườn chôm chôm gia đình và portfolio Fullstack | Triển khai thật, mở rộng dần |
| D-003 | Backend ASP.NET Core Web API .NET 10 | Feature-First + Layered |
| D-004 | Frontend React JS + Vite + CSS thuần | Mobile-first, tiếng Việt, PWA |
| D-005 | SQL Server + EF Core | Database dự kiến RambutanSmartGardenDb |
| D-006 | Vai trò Admin/Member/Viewer theo vườn | `GardenMemberships` |
| D-007 | Admin tạo tài khoản | Login username/số điện thoại; không public register/email reset |
| D-008 | 26 cây ban đầu | 25 chôm chôm Thái, 1 chôm chôm thường |
| D-009 | `TreeCode` không tái sử dụng | `CC-001`–`CC-026` ban đầu |
| D-010 | Cây và vị trí trồng là hai thực thể khác nhau | Hỗ trợ lịch sử thay cây |
| D-011 | Cỏ dại theo vườn hoặc khu vực tùy chọn | Không bắt buộc gắn cây |
| D-012 | Task/Care/Recurrence hỗ trợ Garden/GardenArea/SelectedTrees | `GardenAreaId` có thể NULL |
| D-013 | Thiết bị hợp nhất trong `EquipmentAssets` | Bơm/béc/dụng cụ |
| D-014 | Tưới thật chỉ ghi nhật ký thủ công | Không điều khiển bơm thật |
| D-015 | IoT chỉ là simulator phần mềm trong P2 | Phân biệt simulated/real |
| D-016 | 60 bảng: 34 P0, 19 P1, 7 P2 | Blueprint Phase 01.6 đã duyệt |
| D-017 | Không tạo database/migration khi chưa duyệt riêng | Safety gate bắt buộc |
| D-018 | Người dùng tự viết code, làm từng bước | Chỉ chuyển khi OKE/tiếp |
| D-019 | Git commit bằng tiếng Việt | Giữ prefix Conventional Commits |
| D-020 | Root `E:\Project\RambutanSmartGarden` | GitHub NeiTIm/RambutanSmartGarden |

## 2. Các dữ kiện chưa xác minh

- Diện tích thật, ranh giới chính xác và số đo đo đạc của vườn.
- Tọa độ và tuổi/ngày trồng của từng cây.
- Mã cây nào thuộc giống chôm chôm thường; không gán ngẫu nhiên.
- Các định mức tưới hay thông số bơm vật lý; không lấy giá trị mô phỏng làm thông số thật.
- Chi tiết **từng trường/kiểu/nullability/check/index** của 60 bảng nếu không có toàn văn Phase 01.3/01.4.
- Swagger và API `/api/health` đã chạy thành công hay chưa (chưa có log xác nhận sau hướng dẫn).

## 3. Cách ghi quyết định mới

```markdown
### D-021 — [Tên quyết định]
- Ngày:
- Bối cảnh:
- Các lựa chọn:
- Quyết định:
- Lý do:
- Hệ quả/ảnh hưởng:
- Người duyệt: Chủ dự án
- Trạng thái: Proposed / Approved / Superseded
```
