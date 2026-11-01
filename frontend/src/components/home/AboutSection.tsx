import { Link } from 'react-router-dom'
import { ROUTES } from '@/routes/route-paths'
import {
  Heart,
  ArrowRight,
} from 'lucide-react'

export function AboutSection() {
  return (
    <section id="about" className="py-20 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 border-t border-[#E2D9C8] dark:border-[#2D2720]">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
        
        {/* Cột trái: Khung tuyên ngôn di sản */}
        <div className="lg:col-span-5 relative">
          <div className="border-4 border-double border-[#C5A059] bg-[#1E3F2F] p-8 text-[#FAF7F2] space-y-6 shadow-2xl">
            <div className="w-14 h-14 border-2 border-[#C5A059] bg-[#142E22] flex items-center justify-center text-[#C5A059]">
              <Heart className="h-7 w-7 fill-[#C5A059]" />
            </div>
            
            <h3 className="font-poppins text-2xl font-bold leading-snug tracking-wider text-[#FAF7F2]">
              Lan tỏa yêu thương, <br />
              <span className="text-[#C5A059]">Kết nối triệu tấm lòng.</span>
            </h3>

            <div className="h-[1px] w-16 bg-[#C5A059]/60" />

            <p className="font-inter text-sm text-[#E5DDCF] leading-relaxed italic">
              Give-AID khởi xướng sứ mệnh kết nối bền vững giữa nhà hảo tâm và các tổ chức phi chính phủ uy tín, đưa sự giúp đỡ đến đúng người, đúng thời điểm và trọn vẹn sự minh bạch.
            </p>

            <div className="pt-4 border-t border-[#C5A059]/40 grid grid-cols-2 gap-4 text-center font-poppins">
              <div>
                <div className="text-2xl font-bold text-[#C5A059]">100%</div>
                <div className="text-[10px] uppercase tracking-wider text-[#E5DDCF]/80 font-semibold">Minh bạch tài chính</div>
              </div>
              <div>
                <div className="text-2xl font-bold text-[#C5A059]">MMXXVI</div>
                <div className="text-[10px] uppercase tracking-wider text-[#E5DDCF]/80 font-semibold">Niên giám vận hành</div>
              </div>
            </div>
          </div>
        </div>

        {/* Cột phải: Ba trụ cột giá trị cốt lõi */}
        <div className="lg:col-span-7 space-y-6">
          <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
            <span>⚜</span>
            <span>VỀ CHÚNG TÔI • ABOUT GIVE-AID</span>
            <span>⚜</span>
          </div>

          <h2 className="font-poppins text-3xl sm:text-4xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] leading-tight">
            Sứ Mệnh Vì Một Cộng Đồng Vững Bền
          </h2>

          <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295] leading-relaxed">
            Give-AID là nền tảng quản trị và kết nối thiện nguyện chuẩn mực, nơi các tổ chức phi chính phủ (NGO) uy tín được xác thực và cấp quyền kêu gọi hỗ trợ một cách minh bạch, an toàn và hiệu quả nhất.
          </p>

          {/* 3 Trụ cột giá trị với số La Mã */}
          <div className="space-y-4 pt-2">
            <div className="flex items-start gap-4">
              <div className="w-8 h-8 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] font-poppins font-bold text-xs flex items-center justify-center shrink-0 mt-1 shadow-sm">
                I
              </div>
              <div>
                <h4 className="font-poppins text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB] uppercase tracking-wider">
                  Cầu nối trực tiếp và chuẩn hóa
                </h4>
                <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] mt-1 font-normal">
                  Mọi tổ chức NGO tham gia đều được thẩm định hồ sơ thực địa, bảo đảm tư cách pháp nhân và tính minh bạch của các dự án kêu gọi.
                </p>
              </div>
            </div>

            <div className="flex items-start gap-4">
              <div className="w-8 h-8 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] font-poppins font-bold text-xs flex items-center justify-center shrink-0 mt-1 shadow-sm">
                II
              </div>
              <div>
                <h4 className="font-poppins text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB] uppercase tracking-wider">
                  Minh bạch dòng tiền tuyệt đối
                </h4>
                <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] mt-1 font-normal">
                  Mỗi lượt đóng góp đều tự động sinh Mã giao dịch riêng biệt, hỗ trợ nhà hảo tâm tra cứu và giám sát trực tiếp trên hệ thống điện tử.
                </p>
              </div>
            </div>

            <div className="flex items-start gap-4">
              <div className="w-8 h-8 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] font-poppins font-bold text-xs flex items-center justify-center shrink-0 mt-1 shadow-sm">
                III
              </div>
              <div>
                <h4 className="font-poppins text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB] uppercase tracking-wider">
                  Kiến tạo giá trị tương lai
                </h4>
                <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] mt-1 font-normal">
                  Tập trung bền vững vào giáo dục, phát triển sinh kế và chăm sóc y tế để người yếu thế tự chủ vươn lên xây dựng cuộc sống.
                </p>
              </div>
            </div>
          </div>

          <div className="pt-4 font-poppins">
            <Link
              to={ROUTES.DONATE}
              className="inline-flex items-center gap-2 px-7 py-3 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] text-xs font-bold tracking-wider uppercase border border-[#C5A059] shadow-md transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
            >
              <span>Chung tay hành động cùng Give-AID</span>
              <ArrowRight className="h-4 w-4 text-[#C5A059]" />
            </Link>
          </div>
        </div>

      </div>
    </section>
  )
}
