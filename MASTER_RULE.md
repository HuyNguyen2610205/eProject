# MASTER_RULE.md — Quy tắc làm việc của AI trong dự án eProject

> **Mục đích:** Quy định cách AI coding assistant khảo sát, lập kế hoạch, chỉnh sửa, kiểm thử và báo cáo khi làm việc trong repository eProject.
>
> **Nguyên tắc cốt lõi:** Hiểu trước → Lập kế hoạch → Xác nhận khi cần → Thay đổi tối thiểu → Kiểm thử → Báo cáo trung thực.

---

## 1. Bối cảnh dự án

Định hướng ban đầu của eProject:

- Backend: .NET 10 / ASP.NET Core Web API.
- Kiến trúc: hướng microservice, nhưng chỉ tách service khi có ranh giới nghiệp vụ và lợi ích rõ ràng.
- Database: SQL Server; ORM dự kiến là Entity Framework Core.
- Frontend dự kiến: React + TypeScript.
- API Gateway dự kiến: YARP.
- Các vùng chức năng khởi đầu: Auth, Identity/User, Notification; Admin và Client là các ứng dụng giao diện.
- Git dự kiến: `main` cho bản ổn định; `develop` cho tích hợp; mỗi task làm trên branch phù hợp.

Đây là định hướng, không phải giả định rằng mọi thành phần đã tồn tại. **Luôn kiểm tra repository và cấu hình thực tế trước khi hành động.** Nếu công nghệ hoặc kiến trúc thực tế khác với phần trên, ưu tiên trạng thái đã được xác minh trong code và tài liệu hiện hành. Không tự ý đổi framework, ORM, cấu trúc hoặc kiến trúc nếu task không yêu cầu.

## 2. Bắt buộc lập kế hoạch trước khi sửa code

**AI phải trình bày kế hoạch trước khi sửa code, cấu hình, database schema/migration, pipeline hoặc tài liệu có ảnh hưởng đến cách vận hành dự án.** Task nhỏ có thể dùng kế hoạch ngắn; task lớn phải có kế hoạch chi tiết.

### 2.1. Khảo sát trước khi lập kế hoạch

Khi có quyền truy cập repository, AI phải:

1. Xác định repository root và đường dẫn liên quan.
2. Đọc `MASTER_RULE.md`, `README.md` và tài liệu kiến trúc liên quan nếu có.
3. Kiểm tra project/solution, package, cấu hình, API, entity, migration và test liên quan.
4. Kiểm tra branch và `git status` trước khi thay đổi.
5. Tìm pattern/helper/component hiện có trước khi tạo mới.
6. Phân biệt thông tin đã xác minh với giả định chưa xác minh.

Không tuyên bố đã đọc, chạy, kiểm tra hoặc xác minh điều gì nếu thực tế chưa thực hiện được.

### 2.2. Nội dung kế hoạch

Kế hoạch phải đề cập các mục phù hợp với độ lớn task:

- **Mục tiêu:** vấn đề và kết quả cần đạt.
- **Hiện trạng:** luồng/file/module liên quan, vấn đề đã xác định.
- **Phạm vi:** nội dung sẽ thay đổi và nội dung không thay đổi.
- **Các bước:** thứ tự triển khai và phụ thuộc.
- **File dự kiến:** đường dẫn và lý do; nếu chưa biết thì ghi rõ cần khảo sát.
- **Tác động:** API, database, security, compatibility và service liên quan.
- **Kiểm thử:** lệnh và tình huống cần xác minh.
- **Rủi ro/giả định:** điểm chưa chắc chắn và biện pháp giảm rủi ro.

Với task nhỏ, kế hoạch 2–4 bước là đủ. Với thay đổi nhiều file, kiến trúc, database, security hoặc nhiều service, lập kế hoạch chi tiết.

### 2.3. Khi phải dừng để hỏi người dùng

Phải xin xác nhận trước khi thực hiện nếu:

- Yêu cầu mơ hồ theo cách có thể dẫn tới các thiết kế khác nhau đáng kể.
- Có nguy cơ mất dữ liệu, thay đổi API không tương thích hoặc thay đổi hành vi quan trọng.
- Cần xóa file/dữ liệu, drop/rename bảng/cột, reset database, rewrite Git history, force-push hoặc thực hiện thao tác phá hủy.
- Cần dùng dịch vụ trả phí, thay đổi hạ tầng/quyền truy cập hoặc sử dụng credentials.
- Phạm vi cần mở rộng đáng kể ngoài yêu cầu ban đầu.
- Thiếu thông tin thiết yếu để chọn phương án an toàn.

Không hỏi lại mọi chi tiết nhỏ nếu có thể chọn mặc định an toàn, phù hợp codebase và ghi rõ giả định.

### 2.4. Trình tự

1. Khảo sát repository và yêu cầu.
2. Trình bày kế hoạch.
3. Xin xác nhận với các quyết định/rủi ro cần người dùng chốt.
4. Thực hiện thay đổi theo kế hoạch.
5. Kiểm thử và rà soát diff.
6. Báo cáo kết quả và giới hạn còn lại.

Nếu môi trường yêu cầu làm ngay trong cùng một lượt, vẫn phải trình bày kế hoạch trước khi chỉnh sửa. Không tự tiếp tục khi đang chờ xác nhận về thao tác phá hủy hoặc quyết định có ảnh hưởng lớn.

## 3. Nguyên tắc sửa repository

1. **Đọc trước, sửa sau.** Không phỏng đoán nội dung file quan trọng.
2. **Thay đổi nhỏ nhất có thể.** Chỉ chỉnh phần cần thiết để hoàn thành task.
3. **Bảo vệ công việc của người dùng.** Không ghi đè, xóa hoặc loại bỏ thay đổi chưa commit.
4. **Tái sử dụng trước khi tạo mới.** Tìm helper, service, DTO, entity, extension, hook hoặc component tương tự.
5. **Theo phong cách codebase.** Tôn trọng naming, formatting, cấu trúc và pattern hiện có, trừ khi task yêu cầu chuẩn hóa.
6. **Không mở rộng phạm vi tùy tiện.** Không refactor diện rộng, nâng package hoặc đổi tên hàng loạt không liên quan.
7. Không tuyên bố hoàn thành khi code còn là stub/mock/placeholder; nếu có, phải nêu rõ.
8. Không tự động commit, push, merge hoặc release nếu người dùng chưa yêu cầu.
9. Xác minh đường dẫn/naming thực tế trước khi tạo file.
10. Khi yêu cầu mâu thuẫn với hiện trạng, nêu tác động và đề xuất lựa chọn thay vì âm thầm áp đặt.

## 4. Quy tắc kiến trúc .NET

### 4.1. ASP.NET Core

- Tuân theo framework version khai báo trong `.csproj`, `global.json` và CI; định hướng là .NET 10 nhưng phải xác minh.
- Dùng Dependency Injection của ASP.NET Core; không tự tạo Service Locator.
- Dùng `async`/`await` cho I/O và truyền `CancellationToken` ở các tầng phù hợp.
- Tránh `.Result`/`.Wait()` trong request path.
- Giữ Controller/Endpoint mỏng; đặt nghiệp vụ đúng tầng.
- Dùng Options Pattern cho cấu hình phức tạp và validate cấu hình khi phù hợp.
- Chuẩn hóa lỗi theo cơ chế đang dùng; không trả stack trace hoặc thông tin nội bộ cho client.
- Dùng structured logging, không log dữ liệu nhạy cảm.

### 4.2. Phân tầng

Nếu service đang dùng Clean Architecture/layered architecture, duy trì nguyên tắc:

- `Domain`: entity/value object và quy tắc nghiệp vụ; không phụ thuộc ASP.NET Core, EF Core hoặc hạ tầng cụ thể.
- `Application`: use case, DTO/command/query, interface/port và điều phối nghiệp vụ.
- `Infrastructure`: EF Core, repository implementation, email provider, JWT implementation và tích hợp bên ngoài.
- `Api`: HTTP endpoint/controller, middleware, cấu hình authentication/authorization và composition root.

Chỉ tạo các project/tầng phù hợp với kiến trúc đang dùng. Không tạo tầng máy móc nếu dự án đã có thiết kế khác hợp lý.

### 4.3. Microservice

- Mỗi service sở hữu dữ liệu/schema/database của mình.
- Không cho service khác truy cập trực tiếp bảng nội bộ của service.
- Giao tiếp qua API/contract hoặc message broker phù hợp.
- Không tạo service mới nếu chưa có ranh giới nghiệp vụ và lợi ích rõ ràng.
- Giữ shared libraries nhỏ; không gom nghiệp vụ của nhiều service vào một thư viện dùng chung gây phụ thuộc chặt.
- Khi sửa contract, kiểm tra consumer và khả năng tương thích.

## 5. Database và Entity Framework Core

1. Kiểm tra schema, mapping và migration hiện có trước khi sửa.
2. Nếu schema do EF Core quản lý, ưu tiên migration có tên rõ ràng và lưu cùng source code.
3. Rà soát migration về thứ tự, dữ liệu hiện tại, triển khai và khả năng rollback.
4. Không chạy `DROP DATABASE`, `DROP TABLE`, xóa dữ liệu, reset database hoặc migration phá hủy nếu chưa có xác nhận rõ ràng.
5. Không đưa password, API key, JWT signing key, connection string có credentials hoặc token raw vào source control.
6. Dùng PK/FK, unique constraints/indexes và check constraints cho các bất biến phù hợp; không thêm index tràn lan.
7. Với dữ liệu cần duy nhất, không chỉ dựa vào validation ứng dụng; thêm constraint/index phù hợp trong database.
8. Dùng UTC nhất quán cho timestamp hệ thống, trừ khi domain có yêu cầu khác.
9. Không lưu password plaintext; dùng password hasher phù hợp, không tự phát minh thuật toán hash mật khẩu.
10. Không lưu refresh/reset/verification token raw khi thiết kế yêu cầu lưu token; ưu tiên lưu hash.
11. Khi đổi tên/xóa cột, cân nhắc expand/contract để tránh làm hỏng phiên bản đang chạy.
12. Seed data phải có tính lặp an toàn khi có thể; không seed tài khoản quản trị bằng mật khẩu yếu/cố định trong repository.
13. Không để SQL script thủ công và EF migration trở thành hai nguồn schema độc lập nếu chưa quy định nguồn nào là chuẩn.

## 6. Bảo mật

- Không commit hoặc in ra secrets, credentials, access/refresh token, password hay dữ liệu cá nhân nhạy cảm.
- Dùng secret manager/environment variables; `.env.example` chỉ có tên biến và giá trị mẫu giả.
- Phân biệt authentication (xác thực danh tính) và authorization (kiểm tra quyền).
- JWT validation phải kiểm tra signature, issuer, audience và expiry theo cấu hình tin cậy.
- Không để client tự quyết định thuật toán hoặc signing key.
- Không đặt secret trong frontend bundle.
- Bảo vệ admin/API nhạy cảm ở backend; ẩn nút UI không phải là authorization.
- Thông báo login/reset password phải hạn chế rò rỉ thông tin tài khoản.
- Validate input phía server; dùng EF Core/parameterized query đúng cách để giảm nguy cơ SQL injection.
- Áp dụng rate limiting và bảo vệ brute-force cho endpoint nhạy cảm khi phù hợp phạm vi.
- Không log password, token raw, `Authorization` header hoặc thông tin bí mật.
- Nếu phát hiện lỗ hổng, nêu tác động và biện pháp xử lý rõ ràng.

## 7. API và tương thích

- Giữ quy ước route, status code, DTO và error format hiện có.
- Không trả trực tiếp EF entity từ API nếu có nguy cơ lộ field nội bộ hoặc tạo phụ thuộc schema.
- Dùng status code phù hợp; ví dụ thường là `201 Created` khi tạo tài nguyên thành công.
- Phân biệt lỗi validation, authentication, authorization, không tồn tại và lỗi hệ thống.
- Không đổi route/field/enum/response contract nếu chưa kiểm tra consumer.
- Với thay đổi không tương thích, nêu phương án versioning/nâng cấp và cập nhật test/tài liệu.
- Cập nhật OpenAPI/Swagger/Scalar và ví dụ API khi contract thay đổi.

## 8. Frontend

Khi sửa React/TypeScript:

- Kiểm tra framework, router, component library và cơ chế gọi API hiện có trước khi thêm package.
- Tái sử dụng component/hook hiện có và tách UI khỏi logic gọi API khi phù hợp.
- Không đưa secret vào client bundle.
- Không coi kiểm tra quyền trên frontend là lớp bảo vệ chính.
- Xử lý loading, success, validation error, API error và empty state phù hợp.
- Giữ accessibility cơ bản: label, keyboard navigation, focus và thông báo lỗi.
- Không thay template/thiết kế diện rộng khi task chỉ yêu cầu chỉnh một chức năng.
- Không mặc định lưu token trong `localStorage` mà không cân nhắc threat model. Chọn cách lưu token phù hợp; nếu dùng cookie cần cân nhắc `HttpOnly`, `Secure`, `SameSite` và CSRF.
- Cập nhật TypeScript types khi API contract thay đổi; tránh `any` nếu có thể định nghĩa type cụ thể.
### 8.1. Nguyên tắc làm việc với UI/UX Agent Skills

Khi phát triển hoặc sửa đổi giao diện frontend, AI Agent tham vấn các skills đã cài đặt trong `.agents/skills/` (`ui-ux-pro-max`, `frontend-design`, `shadcn`, `impeccable`, `lucian-uxui`) và tuân thủ các nguyên tắc:

- **Đọc yêu cầu trước khi thiết kế:** Hiểu rõ đối tượng người dùng, ngữ cảnh và luồng trải nghiệm trước khi code.
- **Khảo sát trước khi tạo mới:** Kiểm tra component, style và design system hiện có; không tạo trùng lặp.
- **Ưu tiên tái sử dụng:** Tận dụng tối đa component, utility classes và quy ước của dự án.
- **Phân tách ngữ cảnh:** Phân biệt rõ phong cách Admin Dashboard (chú trọng mật độ dữ liệu, tính chính xác) và Client UI (chú trọng thẩm mỹ, trải nghiệm mượt mà) nếu cả hai cùng tồn tại.
- **Chất lượng cốt lõi:** Xây dựng giao diện responsive (mobile-first hoặc thích ứng đa kích thước), accessible (tương phản màu, focus states, aria-labels) và dễ bảo trì.
- **Bao phủ trạng thái:** Xử lý đầy đủ các trạng thái: loading, empty, error, success, disabled và validation errors.
- **Giữ vững kiến trúc:** Không tự ý thay đổi kiến trúc, framework hoặc bổ sung dependencies lớn khi chưa có yêu cầu.
- **Kiểm thử chặt chẽ:** Kiểm tra lint, type-check, build và test giao diện phù hợp sau khi thay đổi.
- **Báo cáo minh bạch:** Báo cáo chi tiết các file đã sửa và lý do sửa theo đúng quy chuẩn.

## 9. Git workflow

- Kiểm tra branch và `git status` trước khi sửa nếu môi trường cho phép.
- Không chuyển branch, stash, reset, clean hoặc discard thay đổi người dùng khi chưa được phép.
- Dùng branch task phù hợp, ví dụ `feature/auth-register`, `fix/login-validation`, `chore/database-setup`, và tuân theo workflow thực tế của repository.
- Không push trực tiếp vào `main` trừ khi người dùng yêu cầu và quy trình cho phép.
- Commit message nên rõ ràng, ví dụ:
  - `feat(auth): add registration endpoint`
  - `fix(auth): reject expired refresh tokens`
  - `test(auth): cover login validation`
  - `docs: update local setup`
- Không tự commit/push.
- Trước khi kết thúc, rà soát `git diff` để tìm file ngoài phạm vi, secrets, debug code và thay đổi ngoài ý muốn.

## 10. Kiểm thử và xác minh

Sau khi sửa, chọn và chạy các kiểm tra phù hợp:

1. Build project/solution bị ảnh hưởng.
2. Chạy unit test/integration test liên quan nếu có.
3. Chạy formatter/linter/analyzer nếu repository đã cấu hình.
4. Kiểm tra migration/script database nếu schema thay đổi.
5. Kiểm tra luồng thành công và các trường hợp lỗi quan trọng.
6. Rà soát diff cuối cùng.

Các lệnh .NET sau chỉ là ví dụ; điều chỉnh đường dẫn và solution theo repository thực tế:

```bash
dotnet restore
dotnet build
dotnet test
```

Không tuyên bố build/test thành công nếu chưa thực sự chạy. Nếu bị chặn bởi SDK, database, credentials, package feed hoặc môi trường, ghi rõ lệnh nào chưa chạy và nguyên nhân. Không che giấu lỗi test.

### Definition of Done

Task chỉ được báo hoàn thành khi:

- Đạt mục tiêu và đúng phạm vi.
- Không cố ý để lại implementation giả mà không thông báo.
- Test/kiểm tra liên quan đã chạy hoặc giới hạn kiểm tra được nêu rõ.
- Tài liệu/config/schema được cập nhật khi cần.
- Diff đã được rà soát.
- Có hướng dẫn chạy/thử nghiệm khi người dùng cần thực hiện bước tiếp theo.

## 11. Tài liệu và cấu hình

Cập nhật tài liệu khi thay đổi ảnh hưởng đến cài đặt/chạy project, environment variables, API contract, database schema, kiến trúc service, Docker, CI/CD hoặc quy trình triển khai.

Không ghi secret thật vào tài liệu. Dùng dữ liệu mẫu giả như `user@example.com` hoặc `REPLACE_WITH_LOCAL_SECRET`.

## 12. Mẫu phản hồi bắt buộc khi nhận task

### Trước khi sửa

Trình bày kế hoạch theo mẫu sau; rút gọn hoặc mở rộng tùy độ phức tạp:

**Kế hoạch**
1. **Khảo sát:** file/luồng cần đọc và điều cần xác minh.
2. **Thiết kế:** giải pháp dự kiến và lý do.
3. **Thay đổi:** file/module dự kiến tạo hoặc sửa.
4. **Xác minh:** build/test/lệnh và tình huống kiểm tra.
5. **Rủi ro:** tác động và các điểm cần người dùng xác nhận.

Không khẳng định chắc chắn danh sách file khi chưa khảo sát repository; hãy ghi là dự kiến.

### Sau khi sửa

**Đã thực hiện**
- Tóm tắt thay đổi và lý do.
- Liệt kê file quan trọng đã thêm/sửa.

**Kiểm tra**
- Ghi chính xác các lệnh đã chạy và kết quả.
- Nêu kiểm tra chưa chạy và lý do.

**Tác động/lưu ý**
- Ghi migration, biến môi trường, thay đổi API, thao tác thủ công hoặc rủi ro còn lại.

**Bước tiếp theo**
- Chỉ đề xuất bước trực tiếp, liên quan nhất; không tự mở rộng phạm vi.

Không lặp lại toàn bộ code trong báo cáo nếu đường dẫn và mô tả thay đổi là đủ.

## 13. Xử lý sự không chắc chắn

- Phân biệt **đã xác minh**, **suy luận** và **đề xuất**.
- Khi thiếu thông tin, khảo sát repository trước; chỉ hỏi người dùng khi câu trả lời ảnh hưởng đáng kể đến thiết kế, dữ liệu, bảo mật hoặc phạm vi.
- Không bịa file, class, endpoint, package, test result hoặc trạng thái triển khai.
- Nếu có nhiều phương án, nêu một phương án chính, lý do và đánh đổi quan trọng.
- Ưu tiên giải pháp dễ đọc, dễ kiểm thử và dễ bảo trì hơn giải pháp phức tạp chỉ để “đúng kiến trúc”.
- Không thêm framework, package, message broker hoặc cache nếu chưa có nhu cầu rõ ràng.
- Nếu yêu cầu của người dùng mâu thuẫn với quy tắc này, tuân thủ yêu cầu trực tiếp trong phạm vi an toàn; giải thích ngắn gọn tác động nếu có rủi ro.

## 14. Checklist trước khi kết thúc

- [ ] Đã hiểu yêu cầu và tiêu chí hoàn thành.
- [ ] Đã lập kế hoạch trước khi sửa code.
- [ ] Đã khảo sát code hiện có và thay đổi Git chưa commit.
- [ ] Chỉ thay đổi các file cần thiết trong phạm vi.
- [ ] Không làm mất dữ liệu hoặc thay đổi của người dùng.
- [ ] Không thêm secrets/dữ liệu nhạy cảm vào source control hoặc log.
- [ ] Đã cập nhật test/tài liệu/migration khi phù hợp.
- [ ] Đã chạy kiểm tra phù hợp hoặc ghi rõ lý do chưa chạy.
- [ ] Đã rà soát diff cuối.
- [ ] Báo cáo trung thực những gì đã làm, đã kiểm tra và còn tồn tại.

## 15. Thứ tự ưu tiên

Khi quyết định cách làm, ưu tiên theo thứ tự:

1. An toàn dữ liệu, bảo mật và bảo vệ thay đổi của người dùng.
2. Yêu cầu trực tiếp và tiêu chí hoàn thành do người dùng đưa ra.
3. Tương thích với code/kiến trúc hiện hữu.
4. Tính đúng đắn và khả năng kiểm thử.
5. Khả năng bảo trì, mở rộng và sự đơn giản.
6. Tốc độ thực hiện.

**Không được bỏ qua bước lập kế hoạch vì task có vẻ đơn giản.** Với task nhỏ, kế hoạch có thể ngắn; AI vẫn phải xác định phạm vi và cách xác minh trước khi sửa.
