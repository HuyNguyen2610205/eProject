import React from 'react'
import {
  Phone,
  Mail,
  Send,
  Loader2,
  HelpCircle,
} from 'lucide-react'

interface HelpCentreSectionProps {
  contactName: string
  contactEmail: string
  contactSubject: string
  contactMessage: string
  isSubmittingContact: boolean
  onNameChange: (val: string) => void
  onEmailChange: (val: string) => void
  onSubjectChange: (val: string) => void
  onMessageChange: (val: string) => void
  onSubmit: (e: React.FormEvent) => void
}

export function HelpCentreSection({
  contactName,
  contactEmail,
  contactSubject,
  contactMessage,
  isSubmittingContact,
  onNameChange,
  onEmailChange,
  onSubjectChange,
  onMessageChange,
  onSubmit,
}: HelpCentreSectionProps) {
  return (
    <section id="contact" className="py-20 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-12">
        
        {/* Cột trái: Thông tin trợ giúp Cổ Điển */}
        <div className="lg:col-span-5 space-y-6">
          <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-medium tracking-[0.2em] uppercase">
            <span>⚜</span>
            <span>HỘP THƯ TRỢ GIÚP • HELP CENTRE</span>
            <span>⚜</span>
          </div>

          <h2 className="font-poppins text-3xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
            Chúng Tôi Luôn Sẵn Sàng Lắng Nghe
          </h2>

          <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295] leading-relaxed">
            Bạn có thắc mắc về quy trình đóng góp, cần hướng dẫn nhận biên lai điện tử, hoặc tổ chức của bạn muốn ký kết thỏa thuận hợp tác cùng Give-AID? Ban Điều Hành luôn sẵn sàng phản hồi trong 24 giờ.
          </p>

          <div className="space-y-4 pt-2">
            <div className="flex items-center gap-3.5 p-4 border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] shadow-sm">
              <div className="w-10 h-10 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] text-[#8C6D37] dark:text-[#C5A059] flex items-center justify-center shrink-0">
                <Phone className="h-5 w-5" />
              </div>
              <div className="font-poppins">
                <span className="text-[10px] text-[#8C6D37] dark:text-[#C5A059] tracking-wider uppercase block">Đường dây nóng cứu trợ</span>
                <span className="text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">0243.999.888 / 1900.6868</span>
              </div>
            </div>

            <div className="flex items-center gap-3.5 p-4 border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] shadow-sm">
              <div className="w-10 h-10 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] text-[#8C6D37] dark:text-[#C5A059] flex items-center justify-center shrink-0">
                <Mail className="h-5 w-5" />
              </div>
              <div className="font-poppins">
                <span className="text-[10px] text-[#8C6D37] dark:text-[#C5A059] tracking-wider uppercase block">Hộp thư công vụ trực tuyến</span>
                <span className="text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB] font-inter">contact@giveaid.org</span>
              </div>
            </div>

            <div className="flex items-center gap-3.5 p-4 border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] shadow-sm">
              <div className="w-10 h-10 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] text-[#8C6D37] dark:text-[#C5A059] flex items-center justify-center shrink-0">
                <HelpCircle className="h-5 w-5" />
              </div>
              <div className="font-poppins">
                <span className="text-[10px] text-[#8C6D37] dark:text-[#C5A059] tracking-wider uppercase block">Thời gian xử lý phúc đáp</span>
                <span className="text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">Trong vòng 24 giờ làm việc</span>
              </div>
            </div>
          </div>
        </div>

        {/* Cột phải: Form gửi thắc mắc liên hệ Cổ Điển */}
        <div className="lg:col-span-7">
          <div className="border-2 border-double border-[#C5A059] bg-[#FAF7F2] dark:bg-[#181614] p-8 shadow-xl">
            <div className="text-center pb-4 mb-6 border-b border-[#E2D9C8] dark:border-[#2D2720]">
              <span className="font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059] tracking-widest uppercase block mb-1">
                PHIẾU LIÊN LẠC THƯ ĐIỆN TỬ • MMXXVI
              </span>
              <h3 className="font-poppins text-2xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
                Gửi Câu Hỏi Tới Ban Quản Trị Give-AID
              </h3>
            </div>

            <form onSubmit={onSubmit} className="space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label htmlFor="contactName" className="block font-poppins text-xs font-semibold text-[#1F1D1A] dark:text-[#F5F2EB] mb-1 tracking-wider uppercase">
                    Họ và tên <span className="text-red-500">*</span>
                  </label>
                  <input
                    id="contactName"
                    type="text"
                    placeholder="Nguyễn Văn A"
                    value={contactName}
                    onChange={(e) => onNameChange(e.target.value)}
                    disabled={isSubmittingContact}
                    className="w-full border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] px-3.5 py-2.5 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
                  />
                </div>

                <div>
                  <label htmlFor="contactEmail" className="block font-poppins text-xs font-semibold text-[#1F1D1A] dark:text-[#F5F2EB] mb-1 tracking-wider uppercase">
                    Địa chỉ Email <span className="text-red-500">*</span>
                  </label>
                  <input
                    id="contactEmail"
                    type="email"
                    placeholder="email@example.com"
                    value={contactEmail}
                    onChange={(e) => onEmailChange(e.target.value)}
                    disabled={isSubmittingContact}
                    className="w-full border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] px-3.5 py-2.5 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="contactSubject" className="block font-poppins text-xs font-semibold text-[#1F1D1A] dark:text-[#F5F2EB] mb-1 tracking-wider uppercase">
                  Chủ đề thắc mắc <span className="text-red-500">*</span>
                </label>
                <input
                  id="contactSubject"
                  type="text"
                  placeholder="Ví dụ: Tra cứu biên lai đóng góp #GD-2026..."
                  value={contactSubject}
                  onChange={(e) => onSubjectChange(e.target.value)}
                  disabled={isSubmittingContact}
                  className="w-full border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] px-3.5 py-2.5 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
                />
              </div>

              <div>
                <label htmlFor="contactMessage" className="block font-poppins text-xs font-semibold text-[#1F1D1A] dark:text-[#F5F2EB] mb-1 tracking-wider uppercase">
                  Nội dung chi tiết <span className="text-red-500">*</span>
                </label>
                <textarea
                  id="contactMessage"
                  rows={4}
                  placeholder="Nhập nội dung cần hỗ trợ chi tiết..."
                  value={contactMessage}
                  onChange={(e) => onMessageChange(e.target.value)}
                  disabled={isSubmittingContact}
                  className="w-full border border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] px-3.5 py-2.5 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059] resize-none"
                />
              </div>

              <button
                type="submit"
                disabled={isSubmittingContact}
                className="w-full py-3.5 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-poppins text-xs font-bold tracking-widest uppercase border border-[#C5A059] flex items-center justify-center gap-2 shadow-md transition-all focus:outline-none focus:ring-2 focus:ring-[#C5A059]"
              >
                {isSubmittingContact ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin text-[#C5A059]" />
                    <span>Đang gửi thông tin...</span>
                  </>
                ) : (
                  <>
                    <Send className="h-4 w-4 text-[#C5A059]" />
                    <span>Gửi văn bản thắc mắc</span>
                  </>
                )}
              </button>
            </form>
          </div>
        </div>

      </div>
    </section>
  )
}
