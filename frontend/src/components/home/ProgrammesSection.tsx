import { Link } from 'react-router-dom'
import { Programme } from '@/types/content'
import { ROUTES } from '@/routes/route-paths'
import { formatDate } from '@/utils/format'
import {
  Calendar,
  MapPin,
  Building2,
  Loader2,
  ArrowRight,
} from 'lucide-react'

interface ProgrammesSectionProps {
  programmes: Programme[]
  isLoading: boolean
}

export function ProgrammesSection({ programmes, isLoading }: ProgrammesSectionProps) {
  return (
    <section id="programmes" className="py-20 bg-[#F4EFE6] dark:bg-[#181614] border-t border-b border-[#E2D9C8] dark:border-[#2D2720]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center max-w-3xl mx-auto space-y-3 mb-14">
          <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#FAF7F2] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
            <span>⚜</span>
            <span>CHƯƠNG TRÌNH THỰC ĐỊA SẮP DIỄN RA</span>
            <span>⚜</span>
          </div>

          <h2 className="font-poppins text-3xl sm:text-4xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
            Featured NGO Programmes
          </h2>

          <div className="flex items-center justify-center gap-4 my-2 text-[#C5A059]">
            <div className="h-[1px] w-12 bg-gradient-to-r from-transparent to-[#C5A059]" />
            <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
            <div className="h-[1px] w-12 bg-gradient-to-l from-transparent to-[#C5A059]" />
          </div>

          <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295]">
            Các chiến dịch cứu trợ và chương trình tình nguyện thực địa đang mở cổng tiếp nhận đăng ký tham gia và ủng hộ tài chính.
          </p>
        </div>

        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-16 space-y-3">
            <Loader2 className="h-8 w-8 animate-spin text-[#C5A059]" />
            <p className="font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059]">Đang tải dữ liệu chiến dịch thiện nguyện...</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {programmes.map((p) => (
              <div
                key={p.id}
                className="border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#141210] p-6 flex flex-col justify-between hover:shadow-xl hover:border-[#C5A059] transition-all group"
              >
                <div className="space-y-4">
                  <div className="flex items-center justify-between text-xs font-poppins">
                    <span className="inline-flex items-center gap-1.5 text-[#8C6D37] dark:text-[#C5A059] font-bold border border-[#C5A059]/40 bg-[#F4EFE6] dark:bg-[#1E1B17] px-2.5 py-1">
                      <Calendar className="h-3.5 w-3.5 text-[#C5A059]" />
                      <span>{p.startDate ? formatDate(p.startDate).split(' ')[1] || p.startDate : 'Sắp diễn ra'}</span>
                    </span>
                    <span className="text-[10px] font-bold text-[#8C6D37] dark:text-[#C5A059] uppercase tracking-wider">
                      {p.category}
                    </span>
                  </div>

                  <h3 className="font-poppins text-lg font-bold text-[#1F1D1A] dark:text-[#F5F2EB] line-clamp-2 group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors leading-snug">
                    {p.title}
                  </h3>

                  <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] line-clamp-3 leading-relaxed font-normal">
                    {p.description}
                  </p>

                  <div className="space-y-2 pt-2 text-xs font-inter text-[#6B6256] dark:text-[#A69D8F] border-t border-[#E2D9C8] dark:border-[#2D2720]">
                    {p.location && (
                      <div className="flex items-center gap-1.5 text-xs">
                        <MapPin className="h-3.5 w-3.5 text-[#C5A059] shrink-0" />
                        <span className="truncate">{p.location}</span>
                      </div>
                    )}
                    <div className="flex items-center gap-1.5 text-xs">
                      <Building2 className="h-3.5 w-3.5 text-[#8C6D37] dark:text-[#C5A059] shrink-0" />
                      <span className="truncate">Chủ quản: <strong className="text-[#1F1D1A] dark:text-[#F5F2EB]">{p.ngoName}</strong></span>
                    </div>
                  </div>
                </div>

                <div className="pt-6 grid grid-cols-2 gap-3 font-poppins">
                  <Link
                    to={ROUTES.DONATE}
                    className="py-2.5 text-center bg-white dark:bg-[#1E1B17] border border-[#DCD1BA] dark:border-[#383126] hover:border-[#C5A059] text-[#1F1D1A] dark:text-[#F5F2EB] text-xs font-semibold uppercase tracking-wider transition-colors"
                  >
                    Chi tiết
                  </Link>
                  <Link
                    to={ROUTES.DONATE}
                    className="py-2.5 text-center bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] text-xs font-bold uppercase tracking-wider border border-[#C5A059] transition-colors inline-flex items-center justify-center gap-1.5 shadow-sm"
                  >
                    <span>Tham gia</span>
                    <ArrowRight className="h-3 w-3 text-[#C5A059]" />
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
