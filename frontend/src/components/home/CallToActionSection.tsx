import { Link } from 'react-router-dom'
import { ROUTES } from '@/routes/route-paths'
import {
  Gift,
  Users,
} from 'lucide-react'

export function CallToActionSection() {
  return (
    <section className="py-20 bg-[#1E3F2F] text-[#FAF7F2] border-t-4 border-b-4 border-double border-[#C5A059]">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center space-y-6">
        <div className="w-14 h-14 mx-auto border-2 border-[#C5A059] bg-[#142E22] flex items-center justify-center text-[#C5A059] shadow-lg">
          <span className="text-3xl">⚜</span>
        </div>

        <h2 className="font-poppins text-3xl sm:text-5xl font-bold tracking-tight leading-tight text-[#FAF7F2]">
          Be the Reason Someone Smiles Today.
        </h2>

        {/* Chỉ báo hoa văn di sản */}
        <div className="flex items-center justify-center gap-4 text-[#C5A059]">
          <div className="h-[1px] w-20 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
          <div className="h-[1px] w-20 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>

        <p className="font-inter text-lg text-[#E5DDCF] max-w-2xl mx-auto leading-relaxed italic font-normal">
          &ldquo;Your support can help create new opportunities and bring hope to communities in need. Hãy cùng Give-AID nối dài những nhịp cầu nhân ái và thắp sáng tương lai cho những mảnh đời kém may mắn.&rdquo;
        </p>

        <div className="pt-4 flex flex-wrap items-center justify-center gap-4 font-poppins">
          <Link
            to={ROUTES.DONATE}
            className="inline-flex items-center gap-2 px-8 py-3.5 bg-[#C5A059] hover:bg-[#B08B3E] text-[#1F1D1A] font-bold text-xs tracking-wider uppercase shadow-2xl transition-all focus:outline-none focus:ring-2 focus:ring-amber-300"
          >
            <Gift className="h-4 w-4" />
            <span>Donate Now</span>
          </Link>

          <Link
            to={ROUTES.REGISTER}
            className="inline-flex items-center gap-2 px-8 py-3.5 bg-transparent hover:bg-[#142E22] text-[#FAF7F2] font-semibold text-xs tracking-wider uppercase border-2 border-[#C5A059] transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
          >
            <Users className="h-4 w-4 text-[#C5A059]" />
            <span>Become a Volunteer</span>
          </Link>
        </div>
      </div>
    </section>
  )
}
