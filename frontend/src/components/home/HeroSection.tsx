import { Link } from 'react-router-dom'
import { ROUTES } from '@/routes/route-paths'
import {
  Gift,
  ArrowRight,
  ShieldCheck,
  Compass,
} from 'lucide-react'

export function HeroSection() {
  return (
    <section className="relative overflow-hidden pt-12 pb-20 lg:pt-20 lg:pb-28 bg-gradient-to-b from-[#F2ECE1] via-[#FAF7F2] to-[#FAF7F2] dark:from-[#1A1815] dark:via-[#121110] dark:to-[#121110] border-b border-[#E2D9C8] dark:border-[#2D2720]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-8 items-center">
          
          {/* Cột trái: Thông điệp & CTA */}
          <div className="lg:col-span-7 space-y-6 text-center lg:text-left">
            <div className="inline-flex items-center gap-2 px-4 py-1.5 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
              <span>⚜</span>
              <span>TUYÊN NGÔN THIỆN NGUYỆN • ANNO MMXXVI</span>
              <span>⚜</span>
            </div>

            <h1 className="font-poppins text-4xl sm:text-5xl lg:text-6xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB] leading-[1.2]">
              Together, We Can <br className="hidden sm:inline" />
              <span className="text-[#8C6D37] dark:text-[#C5A059]">Make a Difference.</span>
            </h1>

            {/* Chỉ báo hoa văn di sản */}
            <div className="flex items-center justify-center lg:justify-start gap-4 my-2 text-[#C5A059]">
              <div className="h-[1px] w-16 bg-gradient-to-r from-transparent to-[#C5A059]" />
              <span className="text-sm font-serif select-none">❧ ❖ ☙</span>
              <div className="h-[1px] w-16 bg-gradient-to-l from-transparent to-[#C5A059]" />
            </div>

            <p className="font-inter text-lg text-[#524B42] dark:text-[#AAA295] max-w-xl mx-auto lg:mx-0 leading-relaxed font-normal">
              Every act of kindness creates a brighter future. Join Give-AID to support meaningful causes and help build a better world for everyone. Mỗi nghĩa cử nhân ái hôm nay là hạt mầm hy vọng vững bền cho ngày mai.
            </p>

            {/* Hai nút hành động CTA */}
            <div className="pt-2 flex flex-wrap items-center justify-center lg:justify-start gap-4 font-poppins">
              <Link
                to={ROUTES.DONATE}
                className="inline-flex items-center gap-2 px-8 py-3.5 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-bold text-xs tracking-wider uppercase border-2 border-[#C5A059] shadow-xl hover:shadow-2xl transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
              >
                <Gift className="h-4 w-4 text-[#C5A059]" />
                <span>Donate Now</span>
                <ArrowRight className="h-4 w-4 text-[#C5A059]" />
              </Link>

              <a
                href="#causes"
                className="inline-flex items-center gap-2 px-8 py-3.5 bg-white dark:bg-[#1C1A17] hover:bg-[#F4EFE6] dark:hover:bg-[#25221E] text-[#1F1D1A] dark:text-[#F5F2EB] font-semibold text-xs tracking-wider uppercase border border-[#D6CEBE] dark:border-[#383127] shadow-sm transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
              >
                <span>Explore Our Causes</span>
              </a>
            </div>

            {/* Thẻ minh chứng cộng đồng */}
            <div className="pt-4 flex items-center justify-center lg:justify-start gap-3 text-xs font-inter text-[#6B6256] dark:text-[#A69D8F]">
              <span className="text-[#C5A059] text-base">✦</span>
              <span>Hơn <strong className="font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">12,500+ lượt ủng hộ</strong> đã cùng chung tay hành động vì cộng đồng xã hội</span>
            </div>
          </div>

          {/* Cột phải: Bản khắc di sản & Thẻ nổi */}
          <div className="lg:col-span-5 relative">
            <div className="relative mx-auto max-w-md lg:max-w-none">
              
              {/* Khung bản tuyên ngôn di sản */}
              <div className="relative border-4 border-double border-[#C5A059] bg-[#1E3F2F] p-8 text-[#FAF7F2] shadow-2xl space-y-5 text-center">
                <div className="w-16 h-16 mx-auto border-2 border-[#C5A059] bg-[#142E22] flex items-center justify-center text-[#C5A059] shadow-md">
                  <span className="text-3xl">⚜</span>
                </div>

                <div>
                  <span className="font-poppins text-[10px] tracking-widest uppercase text-[#C5A059] block mb-1 font-semibold">
                    CHARITAS ET FIDES • MMXXVI
                  </span>
                  <h2 className="font-poppins text-2xl font-bold tracking-wider text-[#FAF7F2]">
                    GIVE-AID CHARTER
                  </h2>
                </div>

                <div className="h-[1px] w-24 mx-auto bg-[#C5A059]/50" />

                <p className="font-inter text-sm text-[#E5DDCF] leading-relaxed max-w-xs mx-auto italic">
                  &ldquo;Nền tảng quy tụ những tấm lòng quảng đại, phụng sự cộng đồng với lòng trắc ẩn và sự chính trực tuyệt đối.&rdquo;
                </p>

                <div className="pt-2 flex items-center justify-center gap-2 font-poppins text-[11px] text-[#C5A059] tracking-wider uppercase font-semibold">
                  <Compass className="h-4 w-4" />
                  <span>Xác Thực Minh Bạch Dòng Tiền</span>
                </div>
              </div>

              {/* Floating Badge 1: Minh bạch tuyệt đối */}
              <div className="absolute -bottom-5 -left-5 bg-[#FAF7F2] dark:bg-[#1A1816] p-3.5 border-2 border-[#C5A059] shadow-xl flex items-center gap-3">
                <div className="w-10 h-10 bg-[#1E3F2F] text-[#C5A059] flex items-center justify-center font-serif text-lg">
                  <ShieldCheck className="h-5 w-5" />
                </div>
                <div className="text-left font-poppins">
                  <span className="text-[9px] uppercase tracking-wider text-[#8C6D37] dark:text-[#C5A059] block font-semibold">BẢO CHỨNG</span>
                  <span className="text-xs font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">Minh Bạch 100%</span>
                </div>
              </div>

              {/* Floating Badge 2: Vận hành liên tục */}
              <div className="absolute -top-4 -right-4 bg-[#FAF7F2] dark:bg-[#1A1816] px-4 py-2 border border-[#C5A059] shadow-xl flex items-center gap-2">
                <span className="text-xs text-[#C5A059]">✦</span>
                <span className="font-poppins text-xs font-bold text-[#8C6D37] dark:text-[#C5A059]">Hệ Thống 24/7</span>
              </div>

            </div>
          </div>

        </div>
      </div>
    </section>
  )
}
