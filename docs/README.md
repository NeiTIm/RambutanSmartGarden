# RambutanSmartGarden — Bộ tài liệu dự án

**Loại dự án:** ứng dụng web quản lý vườn chôm chôm gia đình, đồng thời là sản phẩm portfolio ASP.NET Core Fullstack.

**Trạng thái:** thiết kế nghiệp vụ và Database Blueprint đã được phê duyệt; đang triển khai khung Backend .NET 10; **chưa tạo database hoặc chạy migration**.

## Mục lục

| File | Nội dung |
|---|---|
| `00-PROJECT-OVERVIEW.md` | Mục tiêu, phạm vi và thực trạng vườn |
| `01-REQUIREMENTS.md` | Yêu cầu chức năng, phi chức năng, phạm vi P0–P2 |
| `02-ACTORS-AND-USE-CASES.md` | Vai trò và danh mục 74 use case theo nhóm |
| `03-ARCHITECTURE.md` | Tech stack, Feature-First + Layered, luồng xử lý |
| `04-DATABASE-BLUEPRINT.md` | Danh mục 60 bảng, phân kỳ triển khai |
| `05-DATA-RULES.md` | Toàn vẹn dữ liệu, nghiệp vụ và lưu ý EF Core |
| `06-API-DESIGN.md` | Quy ước API, đề xuất route, response, auth |
| `07-FRONTEND-UX.md` | Giao diện người dùng và định hướng PWA |
| `08-IRRIGATION-AND-SIMULATOR.md` | Tưới thực tế, ghi chép thủ công và mô phỏng IoT |
| `09-IMPLEMENTATION-ROADMAP.md` | Giai đoạn, thứ tự làm và cổng phê duyệt |
| `10-DEVELOPMENT-GUIDE.md` | Môi trường, lệnh chạy, Git, an toàn dữ liệu |
| `11-TEST-AND-ACCEPTANCE.md` | Kiểm thử thủ công, tiêu chí nghiệm thu |
| `12-DECISION-LOG.md` | Các quyết định đã chốt và vấn đề chưa xác minh |
| `13-DATA-DICTIONARY-TEMPLATE.md` | Mẫu bảng từ điển dữ liệu chi tiết cần điền |

## Phân biệt mức độ chắc chắn

- **Đã chốt:** các quyết định từ Phase 00 và Database Blueprint Phase 01.6, bao gồm số lượng 60 bảng, cách chia P0/P1/P2, 26 mã cây ban đầu, phạm vi tính năng.
- **Đề xuất triển khai:** tên endpoint, tên DTO, danh sách màn hình, quy ước code, checklist kiểm thử; có thể điều chỉnh trong quá trình code.
- **Chưa xác minh:** diện tích chính xác, vị trí và tuổi từng cây; các cột chi tiết của mỗi bảng nếu không có bản Phase 01.3 nguyên văn; dữ liệu hoạt động thật chưa được nhập.

> Không dùng bộ tài liệu này như bằng chứng rằng 60 bảng hay các API đã được tạo trong source. Bộ tài liệu mô tả mục tiêu thiết kế và kế hoạch triển khai.

## Đường dẫn đặt file

Sao chép **các file Markdown ở cùng cấp** vào `E:\Project\RambutanSmartGarden\docs\`.

## Quy trình cập nhật

1. Khi thay đổi nghiệp vụ, cập nhật `01-REQUIREMENTS.md` và `12-DECISION-LOG.md`.
2. Khi thay đổi schema, cập nhật `04-DATABASE-BLUEPRINT.md`, `05-DATA-RULES.md` và từ điển dữ liệu chi tiết.
3. Khi code API xong, bổ sung endpoint thực tế trong `06-API-DESIGN.md`.
4. Chỉ đánh dấu hoàn tất một phase sau khi có bằng chứng build/chạy và người dùng xác nhận **OKE**.
