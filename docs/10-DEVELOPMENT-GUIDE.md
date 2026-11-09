# 10 — Hướng dẫn môi trường phát triển

## 1. Môi trường đã xác minh

| Công cụ | Phiên bản |
|---|---|
| .NET SDK | `10.0.401` |
| Node.js | `v22.14.0` |
| npm | `10.9.2` |
| Git | `2.49.0.windows.1` |
| PowerShell | Có sẵn |

Máy có thêm .NET SDK 8.0.408 và 9.0.318. Backend nhắm `net10.0`.

## 2. Workspace và repository

```text
E:\Project\RambutanSmartGarden\
├── backend\RambutanSmartGarden.Api\
├── frontend\
├── docs\
├── RambutanSmartGarden.slnx
├── README.md
└── .gitignore
```

GitHub: `https://github.com/NeiTIm/RambutanSmartGarden.git`

Nhánh chính: `main`. Git commit message **tiếng Việt** theo Conventional Commits.

## 3. Các lệnh cơ bản

```powershell
cd E:\Project\RambutanSmartGarden

dotnet --version
dotnet sln RambutanSmartGarden.slnx list
dotnet restore RambutanSmartGarden.slnx
dotnet build RambutanSmartGarden.slnx
dotnet run --project backend/RambutanSmartGarden.Api
```

Backend từng khởi chạy trên `http://localhost:5062`. Đây chỉ là cổng quan sát ở một lần chạy; hãy xem Terminal mỗi lần khởi động.

Ở Windows PowerShell, nếu `npm` bị chặn do execution policy, dùng `npm.cmd`.

## 4. Git workflow

```powershell
git status
git diff --stat
git add backend docs
git diff --cached --stat
git commit -m "docs: bổ sung bộ tài liệu dự án"
git push origin main
```

Không stage toàn bộ `.env` hoặc secret. Kiểm tra `.gitignore` trước khi commit. Git không lưu thư mục trống, có thể dùng `.gitkeep`.

## 5. SQL Server — chỉ kế hoạch, chưa thao tác

- Instance dự kiến: `lpc:.\MSSQLSERVER01` (Windows Authentication, local).
- Database dự kiến: `RambutanSmartGardenDb`.
- Không dùng lại DB của MeChat/WebChat.
- **Chưa được phép tạo database/migration ở giai đoạn tài liệu này.**
- Khi được duyệt: xác nhận instance và database name, backup chính sách cần thiết, chỉ sau đó mới tạo migrations từng phase.

## 6. Swagger / API

- Template ASP.NET Core 10 có `AddOpenApi`, `MapOpenApi` trong Development.
- Giao diện Swagger UI cần cấu hình package riêng (ví dụ NSwag hoặc Swashbuckle) và trỏ đúng `/openapi/v1.json`.
- Đây là **kế hoạch kỹ thuật**; chưa có bằng chứng bước Swagger đã hoàn tất trên máy.

## 7. Lưu ý khi phát triển

- Không thay đổi source đang hoạt động hàng loạt.
- Mỗi feature triển khai một vòng Controller → Service → Repository khi cần → EF Core → test.
- Không tạo class/file rỗng chỉ để đủ cấu trúc.
- Không chia sẻ `appsettings.Development.json` nếu chứa secret; ưu tiên User Secrets hoặc biến môi trường.
- Các báo cáo về dữ liệu cây và tài chính không được tự suy đoán những giá trị chưa nhập.
