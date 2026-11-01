import { Link } from 'react-router-dom'
import { Cause } from '@/types/donation'
import { ROUTES } from '@/routes/route-paths'
import { formatCurrency } from '@/utils/format'
import {
  Baby,
  GraduationCap,
  Accessibility,
  Sparkles,
  Users,
  Clock,
  HeartHandshake,
  ArrowRight,
  ChevronRight,
  Loader2,
} from 'lucide-react'

interface CausesSectionProps {
  causes: Cause[]
  isLoading: boolean
}

export function CausesSection({ causes, isLoading }: CausesSectionProps) {
  const getCauseCategoryIcon = (slug: string) => {
    switch (slug.toLowerCase()) {
      case 'children':
        return <Baby className="h-6 w-6 text-[#C5A059]" />
      case 'education':
        return <GraduationCap className="h-6 w-6 text-[#C5A059]" />
      case 'disabled':
        return <Accessibility className="h-6 w-6 text-[#C5A059]" />
      case 'women':
        return <Sparkles className="h-6 w-6 text-[#C5A059]" />
      case 'youth':
        return <Users className="h-6 w-6 text-[#C5A059]" />
      case 'elderly':
        return <Clock className="h-6 w-6 text-[#C5A059]" />
      default:
        return <HeartHandshake className="h-6 w-6 text-[#C5A059]" />
    }
  }

  return (
    <section id="causes" className="py-20 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="text-center max-w-3xl mx-auto space-y-3 mb-14">
        <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
          <span>⚜</span>
          <span>DANH MỤC BẢO TRỢ THIỆN NGUYỆN</span>
          <span>⚜</span>
        </div>

        <h2 className="font-poppins text-3xl sm:text-4xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
          Our Causes
        </h2>

        {/* Chỉ báo hoa văn di sản */}
        <div className="flex items-center justify-center gap-4 my-2 text-[#C5A059]">
          <div className="h-[1px] w-12 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
          <div className="h-[1px] w-12 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>

        <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295]">
          Lựa chọn danh mục bạn nguyện lòng chung tay bảo trợ. Toàn bộ nguồn tiền được quản lý minh bạch và giải ngân trực tiếp đến đối tượng thụ hưởng.
        </p>
      </div>

      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-16 space-y-3">
          <Loader2 className="h-8 w-8 animate-spin text-[#C5A059]" />
          <p className="font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059]">Đang tải danh mục từ SQL Server...</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {causes.map((cause) => (
            <div
              key={cause.id}
              className="border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] p-7 shadow-sm hover:shadow-xl hover:border-[#C5A059] transition-all flex flex-col justify-between group relative"
            >
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div className="w-12 h-12 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] flex items-center justify-center group-hover:scale-105 transition-transform shadow-sm">
                    {getCauseCategoryIcon(cause.slug)}
                  </div>
                  <span className="font-poppins text-[10px] font-bold text-[#8C6D37] dark:text-[#C5A059] tracking-wider uppercase px-2.5 py-1 border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#1E1B17]">
                    #{cause.slug.toUpperCase()}
                  </span>
                </div>

                <div>
                  <h3 className="font-poppins text-xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors">
                    {cause.name}
                  </h3>
                  <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] mt-2 line-clamp-3 leading-relaxed font-normal">
                    {cause.description || 'Chung tay giúp đỡ và kiến tạo các giá trị nhân văn bền vững cho xã hội.'}
                  </p>
                </div>

                {/* Thống kê tiến độ */}
                <div className="pt-4 border-t border-[#E2D9C8] dark:border-[#2D2720] space-y-1.5 font-poppins text-xs">
                  <div className="flex justify-between items-center">
                    <span className="text-[#6B6256] dark:text-[#A69D8F] font-inter">Đã quyên góp:</span>
                    <span className="font-bold text-[#8C6D37] dark:text-[#C5A059] text-sm">
                      {formatCurrency(cause.totalRaised)}
                    </span>
                  </div>
                  <div className="flex justify-between items-center text-[11px] text-[#6B6256] dark:text-[#A69D8F]">
                    <span className="font-inter">Lượt ủng hộ:</span>
                    <span className="font-semibold">{cause.totalDonationsCount} lượt</span>
                  </div>
                </div>
              </div>

              <div className="pt-6">
                <Link
                  to={`${ROUTES.DONATE}?causeId=${cause.id}&causeSlug=${cause.slug}`}
                  className="w-full inline-flex items-center justify-center gap-2 py-3 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-poppins text-xs font-bold tracking-wider uppercase border border-[#C5A059] shadow-md transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
                >
                  <span>Donate to {cause.name}</span>
                  <ArrowRight className="h-3.5 w-3.5 text-[#C5A059]" />
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="mt-12 text-center">
        <Link
          to={ROUTES.CAUSES}
          className="inline-flex items-center gap-1.5 font-poppins text-xs font-bold text-[#8C6D37] dark:text-[#C5A059] hover:underline tracking-wider uppercase"
        >
          <span>Xem chi tiết danh sách tất cả các mục tiêu quyên góp</span>
          <ChevronRight className="h-4 w-4" />
        </Link>
      </div>
    </section>
  )
}
