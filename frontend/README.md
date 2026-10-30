# eProject — Client Web Application (Kiến trúc đầy đủ)

Nền tảng Client Web cho dự án **eProject**, được thiết lập theo kiến trúc phân tầng chuẩn mực, linh hoạt và hoàn toàn trung lập (Neutral), sẵn sàng tiếp nhận bất kỳ đề tài chính thức nào.

---

## 1. Công nghệ sử dụng

- **Core Framework:** React 19 + TypeScript (Strict Mode).
- **Build Tool:** Vite 8 (HMR, Rollup code splitting).
- **Client Routing:** React Router v7 (Lazy loading từng trang với Suspense fallback).
- **Styling Foundation:** Tailwind CSS v4 + Design tokens trung tính (Light/Dark mode).
- **UI & Shared Components:** Bộ primitives chuẩn (`Button`, `Input`, `FormField`, `Container`, `PageHeader`, `Spinner`, `Card`, `Alert`, `Badge`, `Sonner`, `ErrorBoundary`, `LoadingState`, `EmptyState`, `ErrorState`).

---

## 2. Bản đồ Routing & Layout System

| Tuyến đường (Route) | Trang (Page) | Layout áp dụng | Cơ chế bảo vệ | Mô tả & Trách nhiệm |
| :--- | :--- | :--- | :--- | :--- |
| `/` | `HomePage` | `PublicLayout` | Public | Trang giới thiệu nền tảng Client Web |
| `/login` | `LoginPage` | `AuthLayout` | `GuestRoute` | Form đăng nhập tài khoản |
| `/register` | `RegisterPage` | `AuthLayout` | `GuestRoute` | Form đăng ký tài khoản mới |
| `/forgot-password` | `ForgotPasswordPage` | `AuthLayout` | Public | Form yêu cầu khôi phục mật khẩu |
| `/reset-password` | `ResetPasswordPage` | `AuthLayout` | Public | Form đặt lại mật khẩu mới |
| `/verify-email` | `VerifyEmailPage` | `AuthLayout` | Public | Tiếp nhận xác thực kích hoạt tài khoản |
| `/app` | `DashboardPage` | `AppLayout` | `ProtectedRoute` | Cổng điều khiển khách hàng sau đăng nhập |
| `/profile` | `ProfilePage` | `AppLayout` | `ProtectedRoute` | Xem & quản lý thông tin hồ sơ cá nhân |
| `/error` | `ErrorPage` | `PublicLayout` | Public | Trang hiển thị khi gặp sự cố hệ thống |
| `*` | `NotFoundPage` | `PublicLayout` | Public | Trang 404 thân thiện kèm nút về trang chủ |

---

## 3. Cấu trúc thư mục kiến trúc phân tầng

```text
src/
├── app/                       # Application bootstrap & provider composition
│   ├── App.tsx                # Root component mount Providers & AppRoutes
│   ├── providers.tsx          # ErrorBoundary, BrowserRouter, Toaster
│   └── index.ts
├── routes/                    # Quản lý routing tập trung & lazy loading
│   ├── route-paths.ts         # Khai báo hằng số ROUTES
│   ├── AppRoutes.tsx          # Cấu hình routes, chia Layouts & Suspense
│   └── index.ts
├── layouts/                   # Hệ thống bố cục dùng chung
│   ├── PublicLayout.tsx       # Bố cục công khai (Header, Main Container, Footer)
│   ├── AuthLayout.tsx         # Bố cục xác thực tập trung form card
│   ├── AppLayout.tsx          # Bố cục cổng dịch vụ (Top nav, Mobile bottom tabs, Footer)
│   └── index.ts
├── pages/                     # Các trang ứng dụng ánh xạ 1-1 với route
│   ├── HomePage.tsx           # /
│   ├── LoginPage.tsx          # /login
│   ├── RegisterPage.tsx       # /register
│   ├── ForgotPasswordPage.tsx # /forgot-password
│   ├── ResetPasswordPage.tsx  # /reset-password
│   ├── VerifyEmailPage.tsx    # /verify-email
│   ├── DashboardPage.tsx      # /app
│   ├── ProfilePage.tsx        # /profile
│   ├── NotFoundPage.tsx       # *
│   ├── ErrorPage.tsx          # /error
│   └── index.ts
├── components/                # Shared Components
│   ├── ui/                    # UI Primitives
│   │   ├── button.tsx         # Button component (variants, size, isLoading, asChild)
│   │   ├── button-variants.ts # Biến thể Tailwind cho Button
│   │   ├── input.tsx          # Input field có accessible focus ring
│   │   ├── label.tsx          # Form label
│   │   ├── form-field.tsx     # Label + Input + HelperText + ErrorMessage accessible
│   │   ├── container.tsx      # Khung giới hạn chiều rộng trang responsive
│   │   ├── page-header.tsx    # Tiêu đề trang, mô tả, breadcrumbs, actions
│   │   ├── spinner.tsx        # SVG spinner loading trợ năng
│   │   ├── card.tsx           # Card composition (Header/Title/Content/Footer)
│   │   ├── alert.tsx          # Thông báo lỗi / thành công / thông tin
│   │   ├── badge.tsx          # Huy hiệu trạng thái
│   │   └── sonner.tsx         # Toast notification container
│   ├── common/                # Component bổ trợ hệ thống
│   │   ├── ErrorBoundary.tsx  # Bắt ngoại lệ runtime toàn ứng dụng
│   │   ├── LoadingState.tsx   # Trạng thái chờ linh hoạt (Full/Inline)
│   │   ├── EmptyState.tsx     # Trạng thái dữ liệu trống kèm Icon, Title, CTA
│   │   ├── ErrorState.tsx     # Trạng thái lỗi kèm nút thử lại onRetry
│   │   ├── ProtectedRoute.tsx # Route guard yêu cầu xác thực
│   │   ├── GuestRoute.tsx     # Route guard cho khách chưa đăng nhập
│   │   └── index.ts
│   └── index.ts
├── hooks/                     # Custom hooks dùng chung
│   └── useMediaQuery.ts       # Hook kiểm tra responsive media query
├── utils/                     # Hàm tiện ích thuần
│   ├── cn.ts                  # Hàm gộp class Tailwind (clsx + tailwind-merge)
│   ├── format.ts              # Định dạng ngày, số tiền, chuỗi
│   └── index.ts
├── types/                     # Định nghĩa TypeScript dùng chung
│   ├── api.ts                 # ApiResponse<T>
│   ├── user.ts                # UserProfile model
│   └── index.ts
├── styles/                    # Nền tảng Styling
│   └── index.css              # Tailwind CSS v4 & CSS Variables trung tính
└── main.tsx                   # Điểm khởi chạy của ứng dụng
```

---

## 4. Hướng dẫn cài đặt và khởi chạy

```bash
# 1. Cài đặt dependencies (nếu chưa có)
npm install

# 2. Khởi động server phát triển
npm run dev

# 3. Kiểm tra kiểu dữ liệu TypeScript
npx tsc --noEmit

# 4. Kiểm tra linter
npm run lint

# 5. Đóng gói sản phẩm (Production Build)
npm run build
```
Truy cập ứng dụng tại: `http://localhost:5173`.
