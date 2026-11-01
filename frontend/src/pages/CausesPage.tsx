import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { Cause } from '@/types/donation'
import { donationService } from '@/services/donationService'
import { formatCurrency } from '@/utils/format'
import { ROUTES } from '@/routes/route-paths'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription } from '@/components/ui/alert'
import {
  HeartHandshake,
  GraduationCap,
  Baby,
  Accessibility,
  Users,
  Clock,
  Sparkles,
  ArrowRight,
  Search,
  Loader2,
  Gift,
  HelpCircle,
} from 'lucide-react'

// Hàm ánh xạ Icon tương ứng với từng nhóm đối tượng Cause
function getCauseIcon(slug: string) {
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

export function CausesPage() {
  const [causes, setCauses] = useState<Cause[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')

  useEffect(() => {
    loadCauses()
  }, [])

  const loadCauses = async () => {
    setIsLoading(true)
    setErrorMessage(null)
    try {
      const response = await donationService.getAllCauses()
      if (response.success && response.data) {
        setCauses(response.data)
      } else {
        setErrorMessage(response.message || 'Không thể tải danh sách mục tiêu quyên góp.')
      }
    } catch {
      setErrorMessage('Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại đường truyền mạng.')
    } finally {
      setIsLoading(false)
    }
  }

  const filteredCauses = causes.filter((cause) => {
    const query = searchTerm.toLowerCase().trim()
    if (!query) return true
    return (
      cause.name.toLowerCase().includes(query) ||
      (cause.description && cause.description.toLowerCase().includes(query))
    )
  })

  return (
    <div className="min-h-screen bg-[#FAF7F2] dark:bg-[#121110] text-[#221F1C] dark:text-[#EAE5D9] transition-colors duration-500 pb-20">
      
      {/* 1. Header Banner Phong cách Cổ Điển */}
      <section className="border-b border-[#E6DEC9] dark:border-[#2C2720] bg-[#F4EFE6]/70 dark:bg-[#181614]/80 py-16 px-4 text-center">
        <div className="max-w-4xl mx-auto space-y-4">
          <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#DCD1BA] dark:border-[#383126] bg-[#F7F2E8] dark:bg-[#1C1A17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-medium tracking-[0.2em] uppercase">
            <span>⚜</span>
            <span>MỤC TIÊU CỨU TRỢ & PHÁT TRIỂN CỘNG ĐỒNG</span>
            <span>⚜</span>
          </div>

          <h1 className="font-poppins text-4xl sm:text-5xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
            Các Danh Mục Quyên Góp Thiện Nguyện
          </h1>

          <div className="flex items-center justify-center gap-4 my-4 max-w-xs mx-auto text-[#C5A059]">
            <div className="h-[1px] flex-1 bg-gradient-to-r from-transparent to-[#C5A059]" />
            <span className="text-sm font-serif select-none">❧ ❖ ☙</span>
            <div className="h-[1px] flex-1 bg-gradient-to-l from-transparent to-[#C5A059]" />
          </div>

          <p className="font-inter text-base sm:text-lg text-[#615B52] dark:text-[#AAA295] max-w-2xl mx-auto leading-relaxed">
            Mỗi đóng góp của bạn là niềm hy vọng thắp sáng tương lai cho những mảnh đời khó khăn. Toàn bộ nguồn tiền được quản lý minh bạch và giải ngân trực tiếp đến đối tượng thụ hưởng.
          </p>

          {/* Ô tìm kiếm nhanh */}
          <div className="pt-4 max-w-md mx-auto">
            <div className="relative">
              <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-400" />
              <input
                type="text"
                placeholder="Tìm kiếm mục tiêu quyên góp..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full rounded-md border border-[#D8CDB3] dark:border-[#383126] bg-white dark:bg-[#1A1816] pl-10 pr-4 py-2 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] shadow-sm focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
              />
            </div>
          </div>
        </div>
      </section>

      {/* 2. Danh sách Cards */}
      <section className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 pt-12">
        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-20 space-y-4">
            <Loader2 className="h-8 w-8 animate-spin text-[#C5A059]" />
            <p className="font-inter text-sm text-zinc-500">Đang tải danh mục từ thiện...</p>
          </div>
        ) : errorMessage ? (
          <div className="py-12 max-w-lg mx-auto">
            <Alert variant="destructive">
              <AlertDescription className="space-y-3 font-inter">
                <p>{errorMessage}</p>
                <Button size="sm" variant="outline" onClick={loadCauses} className="font-poppins">
                  Thử lại
                </Button>
              </AlertDescription>
            </Alert>
          </div>
        ) : filteredCauses.length === 0 ? (
          <div className="text-center py-20 space-y-3 border border-dashed border-[#D8CDB3] dark:border-[#3A3328] rounded-lg">
            <HelpCircle className="h-10 w-10 text-zinc-400 mx-auto" />
            <h3 className="font-poppins text-xl font-semibold">Không tìm thấy mục tiêu phù hợp</h3>
            <p className="font-inter text-xs text-zinc-500">Vui lòng thử tìm kiếm với từ khóa khác.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {filteredCauses.map((cause) => (
              <Card
                key={cause.id}
                className="flex flex-col justify-between border-2 border-[#E6DEC9] dark:border-[#2C2720] bg-[#FAF7F2] dark:bg-[#181614] hover:border-[#C5A059] hover:shadow-xl transition-all duration-300 group"
              >
                <CardHeader>
                  <div className="flex items-center justify-between gap-2 mb-2">
                    <div className="p-3 rounded-full border border-[#C5A059]/40 bg-[#F5EFEB] dark:bg-[#201D1A]">
                      {getCauseIcon(cause.slug)}
                    </div>
                    <Badge variant="outline" className="border-[#C5A059] text-[#8C6D37] dark:text-[#C5A059] font-poppins text-[10px] font-semibold">
                      ƯU TIÊN #{cause.displayOrder}
                    </Badge>
                  </div>
                  <CardTitle className="font-poppins text-xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB] group-hover:text-[#8C6D37] dark:group-hover:text-[#D4AF37] transition-colors">
                    {cause.name}
                  </CardTitle>
                </CardHeader>

                <CardContent className="space-y-4 flex-1">
                  <p className="font-inter text-sm text-[#615B52] dark:text-[#AAA295] leading-relaxed line-clamp-3">
                    {cause.description || 'Chung tay giúp đỡ và tạo ra các giá trị nhân văn bền vững cho xã hội.'}
                  </p>

                  {/* Thống kê đóng góp */}
                  <div className="pt-3 border-t border-[#EFE8D6] dark:border-[#2C2720] grid grid-cols-2 gap-2 text-xs">
                    <div>
                      <span className="text-zinc-500 block text-[11px] font-inter">Đã quyên góp:</span>
                      <span className="font-bold text-[#8C6D37] dark:text-[#C5A059] font-poppins text-sm">
                        {formatCurrency(cause.totalRaised)}
                      </span>
                    </div>
                    <div className="text-right">
                      <span className="text-zinc-500 block text-[11px] font-inter">Lượt ủng hộ:</span>
                      <span className="font-bold text-zinc-900 dark:text-zinc-100 font-poppins text-sm">
                        {cause.totalDonationsCount} lượt
                      </span>
                    </div>
                  </div>
                </CardContent>

                <CardFooter className="pt-2">
                  <Button
                    asChild
                    className="w-full bg-[#1F1D1A] dark:bg-[#F2ECE1] text-[#FAF7F2] dark:text-[#181614] border border-[#C5A059] hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] font-poppins text-xs font-semibold tracking-wider uppercase transition-colors"
                  >
                    <Link to={`${ROUTES.DONATE}?causeId=${cause.id}&causeSlug=${cause.slug}`}>
                      <Gift className="mr-2 h-4 w-4 text-[#C5A059] dark:text-[#8C6D37]" />
                      <span>Quyên góp ngay</span>
                      <ArrowRight className="ml-1.5 h-3.5 w-3.5" />
                    </Link>
                  </Button>
                </CardFooter>
              </Card>
            ))}
          </div>
        )}
      </section>

      {/* 3. Bottom Call-out Banner */}
      <section className="mt-20 max-w-4xl mx-auto px-4 text-center">
        <div className="p-8 border-2 border-double border-[#C5A059] bg-[#F7F2E7]/80 dark:bg-[#161412] rounded-lg space-y-4">
          <div className="text-[#C5A059] font-serif text-lg">✦ ❖ ✦</div>
          <h3 className="font-poppins text-2xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
            Minh Bạch Tuyệt Đối — Đồng Hành Cùng Give-AID
          </h3>
          <p className="font-inter text-sm text-[#615B52] dark:text-[#AAA295] max-w-xl mx-auto">
            Mọi giao dịch quyên góp đều được ghi nhận bằng mã định danh giao dịch duy nhất, cho phép nhà hảo tâm tra cứu và theo dõi trạng thái bất kỳ lúc nào.
          </p>
          <div className="pt-2">
            <Button asChild variant="outline" className="border-[#C5A059] font-poppins text-xs font-semibold uppercase">
              <Link to={ROUTES.DONATE}>Ủng hộ chung cho quỹ thiện nguyện</Link>
            </Button>
          </div>
        </div>
      </section>

    </div>
  )
}
