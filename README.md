# eProject — Give-AID NGO Platform

> Nền tảng Quản lý Quyên góp và Hoạt động Tổ chức Phi Chính phủ (NGO) xây dựng trên nền tảng Microservices hiện đại.

---

## 📚 Tài liệu dự án

* 🧭 **[Lộ trình phát triển (ROADMAP.md)](./ROADMAP.md)**: Ma trận tiến độ, các giai đoạn phát triển và kế hoạch hành động.
* 📜 **[Quy tắc làm việc (MASTER_RULE.md)](./MASTER_RULE.md)**: Quy tắc kỹ thuật, phân tầng kiến trúc và chuẩn mực thực hiện.
* 💻 **[Tài liệu Frontend (frontend/README.md)](./frontend/README.md)**: Cấu trúc Client Web React 19, hệ thống Routing & Component.

---

## 🏛️ Cấu trúc hệ thống

* **Frontend (`/frontend`)**: React 19 + TypeScript + Vite + Tailwind CSS v4 (Cổng mặc định: `5173`).
* **API Gateway (`/backend/Gateway/ApiGateway`)**: YARP Reverse Proxy (.NET 10, Cổng mặc định: `5169`).
* **Identity Service (`/backend/Services/Identity/Identity.Api`)**: Quản lý tài khoản, JWT, phân quyền (Cổng: `5131`).
* **Donation Service (`/backend/Services/Donation/Donation.Api`)**: Quản lý quyên góp và các danh mục từ thiện (Cổng: `5141`).
* **Content Service (`/backend/Services/Content/Content.Api`)**: Quản lý thông tin tổ chức NGO và chương trình (Cổng: `5151`).
* **Notification Service (`/backend/Services/Notification/Notification.Api`)**: Gửi email thông báo và biên lai (Cổng: `5121`).
* **Database (`/backend/Database`)**: Microsoft SQL Server (`IdentityDb`).
