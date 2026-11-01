import { Ngo } from '@/types/content'
import {
  Building2,
  ExternalLink,
  Loader2,
} from 'lucide-react'

interface PartnersSectionProps {
  ngos: Ngo[]
  isLoading: boolean
}

export function PartnersSection({ ngos, isLoading }: PartnersSectionProps) {
  return (
    <section id="partners" className="py-20 bg-[#F4EFE6] dark:bg-[#181614] border-t border-b border-[#E2D9C8] dark:border-[#2D2720]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center max-w-3xl mx-auto space-y-3 mb-14">
          <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#FAF7F2] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
            <span>⚜</span>
            <span>LIÊN MINH ĐỐI TÁC THIỆN NGUYỆN</span>
            <span>⚜</span>
          </div>

          <h2 className="font-poppins text-3xl sm:text-4xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
            Our NGO Partners
          </h2>

          <div className="flex items-center justify-center gap-4 my-2 text-[#C5A059]">
            <div className="h-[1px] w-12 bg-gradient-to-r from-transparent to-[#C5A059]" />
            <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
            <div className="h-[1px] w-12 bg-gradient-to-l from-transparent to-[#C5A059]" />
          </div>

          <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295]">
            Các tổ chức phi chính phủ uy tín hợp tác triển khai các chương trình bảo trợ và hoạt động thiện nguyện thực tế.
          </p>
        </div>

        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-12 space-y-3">
            <Loader2 className="h-8 w-8 animate-spin text-[#C5A059]" />
            <p className="font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059]">Đang tải dữ liệu đối tác từ hệ thống...</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {ngos.map((ngo) => (
              <div
                key={ngo.id}
                className="border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#141210] p-7 flex flex-col justify-between hover:shadow-xl hover:border-[#C5A059] transition-all group"
              >
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <div className="w-12 h-12 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059]">
                      <Building2 className="h-6 w-6" />
                    </div>
                    <span className="font-poppins inline-flex items-center gap-1 text-[10px] font-bold text-[#8C6D37] dark:text-[#C5A059] border border-[#C5A059]/40 bg-[#F4EFE6] dark:bg-[#1E1B17] px-2.5 py-1 tracking-wider uppercase">
                      <span>⚜ VERIFIED NGO ⚜</span>
                    </span>
                  </div>

                  <div>
                    <h3 className="font-poppins text-xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors">
                      {ngo.name}
                    </h3>
                    <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] mt-2 line-clamp-3 leading-relaxed font-normal">
                      {ngo.description || 'Tổ chức thiện nguyện vì sự tiến bộ xã hội và quyền con người.'}
                    </p>
                  </div>

                  {/* Lĩnh vực hoạt động */}
                  {ngo.causes && ngo.causes.length > 0 && (
                    <div className="flex flex-wrap gap-1.5 pt-2 font-poppins">
                      {ngo.causes.map((c) => (
                        <span
                          key={c.causeId}
                          className="text-[10px] font-semibold bg-[#F4EFE6] text-[#8C6D37] dark:bg-[#1E1B17] dark:text-[#C5A059] px-2 py-0.5 border border-[#DCD1BA] dark:border-[#383126]"
                        >
                          {c.name}
                        </span>
                      ))}
                    </div>
                  )}
                </div>

                <div className="pt-6 border-t border-[#E2D9C8] dark:border-[#2D2720] mt-4 flex items-center justify-between font-poppins">
                  <span className="text-[11px] text-[#6B6256] dark:text-[#A69D8F] font-inter">
                    {ngo.totalProgrammesCount} chiến dịch đang chạy
                  </span>
                  {ngo.website && (
                    <a
                      href={ngo.website}
                      target="_blank"
                      rel="noreferrer"
                      className="inline-flex items-center gap-1 text-xs font-bold text-[#8C6D37] dark:text-[#C5A059] hover:underline uppercase tracking-wider"
                    >
                      <span>Website chính thức</span>
                      <ExternalLink className="h-3 w-3" />
                    </a>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
