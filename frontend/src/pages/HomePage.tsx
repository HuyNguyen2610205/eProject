import { Link } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/routes/route-paths'
import { Shield, Compass, Crown, ArrowRight, Feather, CheckCircle2 } from 'lucide-react'

export function HomePage() {
  const { isAuthenticated, user } = useAuth()

  return (
    <div className="min-h-screen bg-[#FAF7F2] text-[#221F1C] dark:bg-[#121110] dark:text-[#EAE5D9] transition-colors duration-500 overflow-x-hidden selection:bg-[#C5A059]/25 selection:text-[#8C6D37]">
      
      {/* 1. TOP EPIGRAPH BAR (DÒNG ĐỀ TỪ CỔ ĐIỂN) */}
      <div className="border-b border-[#E6DEC9] dark:border-[#2C2720] bg-[#F4EFE6]/70 dark:bg-[#181614]/80 py-2.5 px-4 text-center">
        <p className="font-cinzel text-[11px] sm:text-xs tracking-[0.25em] text-[#8C6D37] dark:text-[#C5A059] uppercase font-semibold">
          ✦ &nbsp; ARCHIVUM ET HERITAS • KHỞI NGUYÊN NĂM MMXXVI • DI SẢN BẤT BIẾN &nbsp; ✦
        </p>
      </div>

      {/* 2. GRAND HERO SECTION (ĐẠI ĐIỆN & KHỞI NGUYÊN) */}
      <section className="relative px-4 sm:px-6 lg:px-8 pt-16 sm:pt-24 pb-20 sm:pb-28 max-w-6xl mx-auto text-center">
        
        {/* Heraldic Emblem / Royal Seal (Ấn tín Hoàng gia cổ điển) */}
        <div className="flex justify-center mb-6">
          <div className="relative p-4 rounded-full border-2 border-[#C5A059]/50 bg-[#FAF7F2] dark:bg-[#181614] shadow-md shadow-[#C5A059]/10">
            <svg
              className="w-12 h-12 text-[#9E7D3B] dark:text-[#C5A059]"
              viewBox="0 0 48 48"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
            >
              {/* Classical Laurel Wreath & Monogram */}
              <circle cx="24" cy="24" r="21" strokeDasharray="2 3" strokeWidth="1" opacity="0.6" />
              <circle cx="24" cy="24" r="17" strokeWidth="1.2" />
              {/* Crown / Crest */}
              <path d="M18 17L24 13L30 17L27 22H21L18 17Z" fill="currentColor" fillOpacity="0.15" />
              {/* Monogram eP */}
              <text
                x="24"
                y="31"
                textAnchor="middle"
                fontSize="12"
                fontFamily="Cinzel, serif"
                fontWeight="bold"
                fill="currentColor"
              >
                eP
              </text>
            </svg>
            <div className="absolute -top-1 -right-1 text-[#C5A059]">❖</div>
            <div className="absolute -bottom-1 -left-1 text-[#C5A059]">❖</div>
          </div>
        </div>

        {/* Eyebrow Inscription */}
        <div className="inline-flex items-center gap-2 mb-4 px-3 py-1 border border-[#DCD1BA] dark:border-[#383126] bg-[#F7F2E8] dark:bg-[#1C1A17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-cinzel tracking-[0.2em] uppercase">
          <span>⚜</span>
          <span>TIÊU CHUẨN THƯỢNG LƯU • IDENTITY CLIENT PORTAL</span>
          <span>⚜</span>
        </div>

        {/* Grand Headline */}
        <h1 className="font-classical text-4xl sm:text-6xl lg:text-7xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB] leading-[1.15] max-w-4xl mx-auto">
          Nơi Giá Trị Cổ Điển Khắc Họa{' '}
          <span className="italic font-normal text-[#8C6D37] dark:text-[#D4AF37] block sm:inline">
            Chuẩn Mực Vĩnh Cửu
          </span>
        </h1>

        {/* Decorative Divider with Fleuron */}
        <div className="flex items-center justify-center gap-4 my-7 max-w-xs mx-auto text-[#C5A059]">
          <div className="h-[1px] flex-1 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-sm font-serif select-none">❧ ❖ ☙</span>
          <div className="h-[1px] flex-1 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>

        {/* Literary Subtitle */}
        <p className="font-classical text-lg sm:text-2xl text-[#524C44] dark:text-[#B5ACA0] max-w-2xl mx-auto leading-relaxed">
          Được kiến tạo từ tinh thần hoàn mỹ của nghệ thuật Phục Hưng và sự chuẩn xác tuyệt đối của kỹ nghệ hiện đại. Một không gian dành riêng cho những trải nghiệm quản trị chuẩn mực và tinh tế.
        </p>

        {/* Action Buttons */}
        <div className="mt-10 flex flex-wrap items-center justify-center gap-4 sm:gap-6">
          {isAuthenticated ? (
            <Link
              to={ROUTES.PROFILE}
              className="group relative inline-flex items-center gap-2 px-8 py-3.5 bg-[#1F1D1A] dark:bg-[#F2ECE1] text-[#FAF7F2] dark:text-[#181614] font-cinzel text-xs sm:text-sm font-semibold tracking-wider uppercase border border-[#C5A059] shadow-lg shadow-[#1F1D1A]/10 hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] dark:hover:text-[#181614] transition-all duration-300"
            >
              <span>Vào Hồ Sơ Cá Nhân ({user?.fullName || 'Tài khoản'})</span>
              <ArrowRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
            </Link>
          ) : (
            <>
              <Link
                to={ROUTES.LOGIN}
                className="group relative inline-flex items-center gap-2 px-8 py-3.5 bg-[#1F1D1A] dark:bg-[#EAE4D8] text-[#FAF7F2] dark:text-[#1A1816] font-cinzel text-xs sm:text-sm font-semibold tracking-wider uppercase border border-[#C5A059] shadow-lg shadow-[#1F1D1A]/15 hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] dark:hover:text-[#181614] transition-all duration-300"
              >
                <span>Đăng Nhập Hệ Thống</span>
                <ArrowRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
              </Link>

              <Link
                to={ROUTES.REGISTER}
                className="inline-flex items-center gap-2 px-8 py-3.5 bg-transparent text-[#1F1D1A] dark:text-[#EAE4D8] font-cinzel text-xs sm:text-sm font-semibold tracking-wider uppercase border-2 border-[#C5A059]/60 hover:border-[#C5A059] hover:bg-[#C5A059]/10 transition-all duration-300"
              >
                <span>Đăng Ký Thành Viên</span>
              </Link>
            </>
          )}
        </div>

        {/* Small Latin Motto footer in hero */}
        <div className="mt-12 text-xs font-cinzel tracking-widest text-[#8A8175] dark:text-[#7A7368]">
          • PER ASPERA AD ASTRA • CHÍNH TRỰC • BẢO TOÀN • TRƯỜNG TỒN •
        </div>
      </section>

      {/* 3. THREE HERITAGE PILLARS (BA TRỤ CỘT TINH HOA LA MÃ) */}
      <section className="py-20 border-t border-b border-[#E6DEC9] dark:border-[#2A251E] bg-[#F5EFEB]/60 dark:bg-[#161412]/80">
        <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
          
          <div className="text-center mb-16">
            <span className="font-cinzel text-xs tracking-[0.25em] text-[#8C6D37] dark:text-[#C5A059] uppercase block mb-2">
              TRIẾT LÝ NỀN TẢNG
            </span>
            <h2 className="font-classical text-3xl sm:text-4xl lg:text-5xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
              Ba Trụ Cột Của Sự Hoàn Hảo
            </h2>
            <div className="w-16 h-[1.5px] bg-[#C5A059] mx-auto mt-4" />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            
            {/* Cột I */}
            <div className="relative p-8 border border-[#DFD5BE] dark:border-[#332D23] bg-[#FAF7F2] dark:bg-[#1A1816] shadow-sm hover:shadow-xl transition-all duration-300 group">
              <div className="absolute top-4 right-4 font-cinzel text-xs text-[#C5A059] font-bold">
                CỘT I
              </div>
              <div className="w-12 h-12 mb-6 rounded-full border border-[#C5A059] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059] bg-[#F7F2E7] dark:bg-[#201D1A]">
                <Compass className="h-6 w-6 stroke-[1.5]" />
              </div>
              <h3 className="font-classical text-2xl font-bold mb-3 text-[#1F1D1A] dark:text-[#F5F2EB]">
                Tinh Hoa Nghệ Thuật
              </h3>
              <p className="font-classical text-base text-[#615B52] dark:text-[#AAA295] leading-relaxed">
                Từng đường nét, tỉ lệ và khoảng cách thị giác được thiết lập dựa trên tỉ lệ vàng của trường phái cổ điển, mang lại sự điềm tĩnh và cân bằng tuyệt mỹ.
              </p>
              <div className="mt-6 pt-4 border-t border-[#EFE8D6] dark:border-[#2C2720] text-xs font-cinzel tracking-wider text-[#8C6D37] dark:text-[#C5A059]">
                ARTISANAL ACCURACY
              </div>
            </div>

            {/* Cột II */}
            <div className="relative p-8 border-2 border-[#C5A059]/70 dark:border-[#C5A059]/60 bg-[#FAF7F2] dark:bg-[#1A1816] shadow-md shadow-[#C5A059]/5 hover:shadow-xl transition-all duration-300 group">
              <div className="absolute top-4 right-4 font-cinzel text-xs text-[#C5A059] font-bold">
                CỘT II
              </div>
              <div className="w-12 h-12 mb-6 rounded-full border border-[#C5A059] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059] bg-[#F7F2E7] dark:bg-[#201D1A]">
                <Shield className="h-6 w-6 stroke-[1.5]" />
              </div>
              <h3 className="font-classical text-2xl font-bold mb-3 text-[#1F1D1A] dark:text-[#F5F2EB]">
                Pháo Đài Bất Khả Xâm
              </h3>
              <p className="font-classical text-base text-[#615B52] dark:text-[#AAA295] leading-relaxed">
                Hệ thống xác thực đa tầng ASP.NET Core Identity, mã hóa bảo mật chuẩn OWASP và luồng khôi phục phiên nghiêm ngặt bảo vệ quyền riêng tư tuyệt đối.
              </p>
              <div className="mt-6 pt-4 border-t border-[#EFE8D6] dark:border-[#2C2720] text-xs font-cinzel tracking-wider text-[#8C6D37] dark:text-[#C5A059]">
                FORTIFIED INTEGRITY
              </div>
            </div>

            {/* Cột III */}
            <div className="relative p-8 border border-[#DFD5BE] dark:border-[#332D23] bg-[#FAF7F2] dark:bg-[#1A1816] shadow-sm hover:shadow-xl transition-all duration-300 group">
              <div className="absolute top-4 right-4 font-cinzel text-xs text-[#C5A059] font-bold">
                CỘT III
              </div>
              <div className="w-12 h-12 mb-6 rounded-full border border-[#C5A059] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059] bg-[#F7F2E7] dark:bg-[#201D1A]">
                <Crown className="h-6 w-6 stroke-[1.5]" />
              </div>
              <h3 className="font-classical text-2xl font-bold mb-3 text-[#1F1D1A] dark:text-[#F5F2EB]">
                Đặc Quyền Quý Tộc
              </h3>
              <p className="font-classical text-base text-[#615B52] dark:text-[#AAA295] leading-relaxed">
                Quản lý hồ sơ, cập nhật mật khẩu độc lập và tương tác trực quan với quyền hạn tài khoản được phân định chuẩn mực, tạo sự tin cậy bền vững.
              </p>
              <div className="mt-6 pt-4 border-t border-[#EFE8D6] dark:border-[#2C2720] text-xs font-cinzel tracking-wider text-[#8C6D37] dark:text-[#C5A059]">
                NOBLE EXCELLENCE
              </div>
            </div>

          </div>

        </div>
      </section>

      {/* 4. EDITORIAL MASTERPIECE SECTION (CHƯƠNG MỤC TINH TUYỂN) */}
      <section className="py-20 max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
          
          {/* Cột bên trái: Khung tranh cổ điển / Huy hiệu số */}
          <div className="lg:col-span-5">
            <div className="relative p-6 sm:p-8 border-4 border-double border-[#C5A059] bg-[#FAF7F2] dark:bg-[#171513] shadow-xl">
              {/* Corner Ornaments */}
              <div className="absolute -top-3 -left-3 text-[#C5A059] text-xl">⚜</div>
              <div className="absolute -top-3 -right-3 text-[#C5A059] text-xl">⚜</div>
              <div className="absolute -bottom-3 -left-3 text-[#C5A059] text-xl">⚜</div>
              <div className="absolute -bottom-3 -right-3 text-[#C5A059] text-xl">⚜</div>

              <div className="text-center py-8 px-4 border border-[#D8CDB3] dark:border-[#383125]">
                <span className="font-cinzel text-xs tracking-widest text-[#8C6D37] dark:text-[#C5A059] uppercase">
                  ẤN TÍCH DI SẢN • MMXXVI
                </span>
                <div className="my-6 flex justify-center">
                  <div className="w-20 h-20 rounded-full border border-dashed border-[#C5A059] flex items-center justify-center">
                    <Feather className="h-10 w-10 text-[#C5A059]" />
                  </div>
                </div>
                <h4 className="font-classical text-2xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] mb-2">
                  Chuẩn Tố Khách Hàng
                </h4>
                <p className="font-classical text-sm text-[#736B60] dark:text-[#9A9184]">
                  Xác thực an toàn • Kết nối liền mạch • Vận hành ổn định
                </p>

                <div className="mt-8 pt-6 border-t border-[#EFE8D6] dark:border-[#2C2720] grid grid-cols-2 gap-4 text-center">
                  <div>
                    <div className="font-cinzel text-xl font-bold text-[#8C6D37] dark:text-[#C5A059]">
                      99.98%
                    </div>
                    <div className="font-classical text-xs text-[#736B60] dark:text-[#9A9184] mt-0.5">
                      Độ sẵn sàng
                    </div>
                  </div>
                  <div>
                    <div className="font-cinzel text-xl font-bold text-[#8C6D37] dark:text-[#C5A059]">
                      0.00s
                    </div>
                    <div className="font-classical text-xs text-[#736B60] dark:text-[#9A9184] mt-0.5">
                      Độ trễ xác thực
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Cột bên phải: Đoạn văn học cổ điển với Drop Cap */}
          <div className="lg:col-span-7 space-y-6">
            <span className="font-cinzel text-xs tracking-[0.2em] text-[#8C6D37] dark:text-[#C5A059] uppercase block">
              CHƯƠNG MỤC THỨ NHẤT
            </span>
            <h2 className="font-classical text-3xl sm:text-4xl lg:text-5xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] leading-tight">
              Sự Hòa Hợp Tuyệt Đối Giữa Trật Tự Và Linh Hồn
            </h2>

            {/* Classical Drop Cap Paragraph */}
            <p className="font-classical text-lg sm:text-xl text-[#4A443C] dark:text-[#CBC4B6] leading-relaxed">
              <span className="float-left font-cinzel text-5xl sm:text-6xl leading-[0.8] pr-3 pt-1 text-[#8C6D37] dark:text-[#C5A059] font-bold">
                B
              </span>
              ất kỳ một kiệt tác nào trường tồn qua thời gian đều mang trong mình linh hồn của sự tỉ mỉ. Tại eProject, chúng tôi không xem giao diện chỉ là những khối lệnh máy móc, mà là một bức họa sống động nơi từng nhịp tương tác mang lại sự an tâm, vững chãi và danh dự cho người sử dụng.
            </p>

            <p className="font-classical text-base sm:text-lg text-[#5A544C] dark:text-[#AEA596] leading-relaxed">
              Từ quá trình mã hóa mật khẩu theo tiêu chuẩn cao cấp, cơ chế tạo mã bảo vệ độc lập, đến việc quản lý trạng thái phiên làm việc không bị gián đoạn — mọi khía cạnh đều được trau chuốt như cách các nghệ nhân chế tác một cỗ máy đồng hồ cơ học tinh xảo.
            </p>

            <div className="pt-4 flex items-center gap-6">
              <div className="flex items-center gap-2 text-sm font-classical text-[#8C6D37] dark:text-[#C5A059]">
                <CheckCircle2 className="h-4 w-4" />
                <span>Bảo mật OWASP</span>
              </div>
              <div className="flex items-center gap-2 text-sm font-classical text-[#8C6D37] dark:text-[#C5A059]">
                <CheckCircle2 className="h-4 w-4" />
                <span>Identity .NET 9</span>
              </div>
              <div className="flex items-center gap-2 text-sm font-classical text-[#8C6D37] dark:text-[#C5A059]">
                <CheckCircle2 className="h-4 w-4" />
                <span>Phiên làm việc bền vững</span>
              </div>
            </div>
          </div>

        </div>
      </section>

      {/* 5. ROYAL CALL-TO-HERITAGE BANNER (LỜI HIỆU TRIỆU GIA NHẬP) */}
      <section className="py-16 bg-[#F4EFE6]/90 dark:bg-[#181614] border-t border-[#E6DEC9] dark:border-[#2A251E]">
        <div className="max-w-4xl mx-auto px-4 text-center">
          <div className="text-xl text-[#C5A059] mb-3">✦ ❖ ✦</div>
          <h2 className="font-classical text-3xl sm:text-4xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] mb-4">
            Khởi Đầu Hành Trình Đẳng Cấp Ngay Hôm Nay
          </h2>
          <p className="font-classical text-base sm:text-lg text-[#615B52] dark:text-[#AAA295] max-w-xl mx-auto mb-8 leading-relaxed">
            Gia nhập cộng đồng người dùng ưu tú của eProject để trải nghiệm trọn vẹn sự chuẩn xác và bảo mật tối cao.
          </p>

          <div className="flex flex-wrap items-center justify-center gap-4">
            {isAuthenticated ? (
              <Link
                to={ROUTES.PROFILE}
                className="px-8 py-3 bg-[#1F1D1A] dark:bg-[#F2ECE1] text-[#FAF7F2] dark:text-[#181614] font-cinzel text-xs font-semibold tracking-wider uppercase border border-[#C5A059] hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] transition-colors"
              >
                Trang Hồ Sơ Của Bạn
              </Link>
            ) : (
              <>
                <Link
                  to={ROUTES.REGISTER}
                  className="px-8 py-3 bg-[#1F1D1A] dark:bg-[#EAE4D8] text-[#FAF7F2] dark:text-[#1A1816] font-cinzel text-xs font-semibold tracking-wider uppercase border border-[#C5A059] hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] dark:hover:text-[#181614] transition-colors"
                >
                  Tạo Tài Khoản Mới
                </Link>
                <Link
                  to={ROUTES.LOGIN}
                  className="px-8 py-3 bg-transparent text-[#1F1D1A] dark:text-[#EAE4D8] font-cinzel text-xs font-semibold tracking-wider uppercase border border-[#C5A059]/70 hover:bg-[#C5A059]/10 transition-colors"
                >
                  Đăng Nhập
                </Link>
              </>
            )}
          </div>
        </div>
      </section>

      {/* 6. CLASSICAL COLOPHON & FOOTNOTE (LỜI KẾT & ẤN TRIỆN CỔ ĐIỂN) */}
      <footer className="border-t border-[#E6DEC9] dark:border-[#25201A] bg-[#FAF7F2] dark:bg-[#121110] py-8 text-center text-xs font-classical text-[#8A8175] dark:text-[#7A7368]">
        <div className="max-w-6xl mx-auto px-4 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="font-cinzel text-[11px] tracking-wider text-[#8C6D37] dark:text-[#C5A059]">
            eProject Client Portal • MMXXVI
          </div>
          <div>
            Nghệ thuật cổ điển • Kỹ thuật hiện đại • Bảo toàn chuẩn mực
          </div>
          <div className="text-[11px] font-cinzel tracking-widest">
            ALL RIGHTS RESERVED
          </div>
        </div>
      </footer>

    </div>
  )
}
