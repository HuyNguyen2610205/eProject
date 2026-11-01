import React from 'react'
import { Link } from 'react-router-dom'
import { ROUTES } from '@/routes/route-paths'

interface FooterSectionProps {
  newsletterEmail: string
  onNewsletterEmailChange: (val: string) => void
  onNewsletterSubmit: (e: React.FormEvent) => void
}

export function FooterSection({
  newsletterEmail,
  onNewsletterEmailChange,
  onNewsletterSubmit,
}: FooterSectionProps) {
  return (
    <footer className="border-t border-[#E2D9C8] bg-[#F4EFE6] dark:border-[#2D2720] dark:bg-[#141210] pt-16 pb-12 text-[#524B42] dark:text-[#AAA295]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-12 gap-10 pb-12 border-b border-[#DCD1BA] dark:border-[#2D2720]">
          
          {/* Nhóm 1: About Give-AID Cổ Điển */}
          <div className="lg:col-span-4 space-y-4">
            <Link to={ROUTES.HOME} className="flex items-center gap-2.5">
              <div className="flex h-8 w-8 items-center justify-center border border-[#C5A059] bg-[#1E3F2F] text-[#C5A059]">
                <span>⚜</span>
              </div>
              <span className="font-poppins text-base font-bold tracking-[0.2em] text-[#1F1D1A] dark:text-[#F5F2EB]">
                GIVE-AID NGO
              </span>
            </Link>

            <p className="font-inter text-sm leading-relaxed text-[#524B42] dark:text-[#AAA295]">
              Nền tảng thiện nguyện số kết nối cộng đồng nhà hảo tâm với các tổ chức phi chính phủ (NGO) uy tín tại Việt Nam. Vận hành theo nguyên tắc minh bạch dòng tiền 100% và an toàn tối thượng.
            </p>

            <p className="font-inter text-xs text-[#8C6D37] dark:text-[#C5A059] italic">
              *Mọi số liệu thành tựu và tác động cộng đồng được tổng hợp minh bạch từ báo cáo các dự án liên kết.
            </p>
          </div>

          {/* Nhóm 2: Quick Links */}
          <div className="lg:col-span-2 space-y-3 font-poppins">
            <h4 className="font-bold text-xs uppercase tracking-wider text-[#1F1D1A] dark:text-[#F5F2EB]">
              Quick Links
            </h4>
            <ul className="space-y-2 text-xs font-inter">
              <li>
                <Link to={ROUTES.HOME} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Home
                </Link>
              </li>
              <li>
                <Link to={ROUTES.CAUSES} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Our Causes
                </Link>
              </li>
              <li>
                <Link to={ROUTES.DONATE} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Donate Now
                </Link>
              </li>
              <li>
                <a href="#programmes" className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Programmes
                </a>
              </li>
              <li>
                <a href="#about" className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  About Us
                </a>
              </li>
              <li>
                <a href="#partners" className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Our Partners
                </a>
              </li>
            </ul>
          </div>

          {/* Nhóm 3: Support */}
          <div className="lg:col-span-3 space-y-3 font-poppins">
            <h4 className="font-bold text-xs uppercase tracking-wider text-[#1F1D1A] dark:text-[#F5F2EB]">
              Support
            </h4>
            <ul className="space-y-2 text-xs font-inter">
              <li>
                <a href="#contact" className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Help Centre
                </a>
              </li>
              <li>
                <a href="#contact" className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Contact Us
                </a>
              </li>
              <li>
                <Link to={ROUTES.LOGIN} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Donor Login
                </Link>
              </li>
              <li>
                <Link to={ROUTES.REGISTER} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Register Account
                </Link>
              </li>
              <li>
                <Link to={ROUTES.FORGOT_PASSWORD} className="hover:text-[#8C6D37] dark:hover:text-[#C5A059] transition-colors">
                  Forgot Password
                </Link>
              </li>
              <li>
                <span className="text-[#8C6D37] dark:text-[#C5A059]/60">Privacy Policy</span>
              </li>
            </ul>
          </div>

          {/* Nhóm 4: Newsletter */}
          <div className="lg:col-span-3 space-y-3 font-poppins">
            <h4 className="font-bold text-xs uppercase tracking-wider text-[#1F1D1A] dark:text-[#F5F2EB]">
              Bản Tin Định Kỳ
            </h4>
            <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295]">
              Nhận thông cáo cập nhật về các chiến dịch cứu trợ khẩn cấp và báo cáo tổng kết thường niên.
            </p>
            <form onSubmit={onNewsletterSubmit} className="space-y-2">
              <input
                type="email"
                placeholder="Nhập email của bạn..."
                value={newsletterEmail}
                onChange={(e) => onNewsletterEmailChange(e.target.value)}
                className="w-full border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#181614] px-3 py-2 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] focus:outline-none focus:border-[#C5A059]"
              />
              <button
                type="submit"
                className="w-full py-2.5 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-poppins text-xs font-bold uppercase tracking-wider border border-[#C5A059] transition-colors"
              >
                Đăng ký nhận tin
              </button>
            </form>
          </div>

        </div>

        <div className="pt-8 flex flex-col sm:flex-row items-center justify-between gap-4 font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059]">
          <div>
            ⚜ &copy; MMXXVI GIVE-AID NGO WEBSITE • TOGETHER, WE MAKE A DIFFERENCE ⚜
          </div>
          <div className="flex gap-4">
            <span>NỀN TẢNG THIỆN NGUYỆN DI SẢN • ASP.NET CORE & REACT 19</span>
          </div>
        </div>
      </div>
    </footer>
  )
}
