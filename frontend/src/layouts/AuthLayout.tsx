import { Link, Outlet } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-[#FAF7F2] px-4 py-12 text-[#221F1C] dark:bg-[#121110] dark:text-[#EAE5D9] transition-colors duration-300 font-inter">
      {/* Brand Header */}
      <div className="mb-8 text-center space-y-2">
        <Link to="/" className="inline-block group">
          <div className="flex items-center justify-center gap-2">
            <span className="text-xl text-[#C5A059]">⚜</span>
            <span className="font-poppins text-2xl font-bold tracking-wider text-[#1F1D1A] dark:text-[#F5F2EB] group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors">
              GIVE-AID
            </span>
            <span className="text-xl text-[#C5A059]">⚜</span>
          </div>
          <span className="font-poppins text-[10px] tracking-widest uppercase text-[#8C6D37] dark:text-[#C5A059] block mt-1 font-semibold">
            CỔNG ĐỊNH DANH THIỆN NGUYỆN • MMXXVI
          </span>
        </Link>
        <div className="flex items-center justify-center gap-3 max-w-[180px] mx-auto text-[#C5A059]/60">
          <div className="h-[1px] flex-1 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-xs">❧ ❖ ☙</span>
          <div className="h-[1px] flex-1 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>
      </div>

      {/* Main Centered Form Card */}
      <div className="w-full max-w-md">
        <Outlet />
      </div>

      {/* Return Link */}
      <div className="mt-8 text-center text-xs font-poppins">
        <Link
          to="/"
          className="inline-flex items-center gap-1.5 text-[#8C6D37] dark:text-[#C5A059] hover:underline transition-colors font-medium"
        >
          <ArrowLeft className="h-3.5 w-3.5" />
          <span>Quay lại Trang chủ Give-AID</span>
        </Link>
      </div>
    </div>
  )
}
