import { useState } from 'react'
import {
  Heart,
  GraduationCap,
  Trees,
  CalendarDays,
  Users,
} from 'lucide-react'

interface GalleryItem {
  id: string
  title: string
  category: 'all' | 'community' | 'education' | 'environment' | 'events' | 'volunteer'
  categoryLabel: string
  location: string
  description: string
  color: string
}

export function GallerySection() {
  const [activeTab, setActiveTab] = useState<string>('all')

  const categories = [
    { key: 'all', label: 'Tất cả hoạt động', icon: Heart },
    { key: 'community', label: 'Community Support', icon: Heart },
    { key: 'education', label: 'Education Activities', icon: GraduationCap },
    { key: 'environment', label: 'Environmental Activities', icon: Trees },
    { key: 'events', label: 'Charity Events', icon: CalendarDays },
    { key: 'volunteer', label: 'Volunteer Programmes', icon: Users },
  ]

  const galleryItems: GalleryItem[] = [
    {
      id: 'g1',
      title: 'Trao tặng 500 suất quà & áo ấm vùng cao',
      category: 'community',
      categoryLabel: 'Community Support',
      location: 'Huyện Mèo Vạc, Hà Giang',
      description: 'Chuyến xe thiện nguyện mang hơi ấm mùa đông và nhu yếu phẩm đến các hộ nghèo vùng núi đá.',
      color: 'from-[#1E3F2F] to-[#11241B]',
    },
    {
      id: 'g2',
      title: 'Khánh thành tủ sách Hy Vọng & lớp học mới',
      category: 'education',
      categoryLabel: 'Education Activities',
      location: 'Huyện Nam Trà My, Quảng Nam',
      description: 'Trang bị hơn 1.200 đầu sách thiếu nhi và bàn ghế học tập cho các em học sinh tiểu học.',
      color: 'from-[#2A3A4A] to-[#16212D]',
    },
    {
      id: 'g3',
      title: 'Chiến dịch trồng 2.000 cây xanh giữ rừng đầu nguồn',
      category: 'environment',
      categoryLabel: 'Environmental Activities',
      location: 'Vườn quốc gia Cúc Phương, Ninh Bình',
      description: 'Hoạt động gắn kết tình nguyện viên và người dân địa phương phủ xanh đồi trọc, bảo vệ nguồn nước.',
      color: 'from-[#243A2A] to-[#122016]',
    },
    {
      id: 'g4',
      title: 'Giải chạy Marathon "Nụ Cười Trẻ Thơ 2026"',
      category: 'events',
      categoryLabel: 'Charity Events',
      location: 'Hồ Hoàn Kiếm, Hà Nội',
      description: 'Sự kiện thường niên thu hút hơn 3.000 vận động viên gây quỹ phẫu thuật nụ cười cho trẻ em dị tật hàm mặt.',
      color: 'from-[#4D3A1F] to-[#2B2011]',
    },
    {
      id: 'g5',
      title: 'Khám bệnh và phát thuốc nhân đạo miễn phí',
      category: 'volunteer',
      categoryLabel: 'Volunteer Programmes',
      location: 'Huyện Cần Giờ, TP. Hồ Chí Minh',
      description: 'Đoàn y bác sĩ tình nguyện đã khám và tư vấn sức khỏe cho 600 người già neo đơn và gia đình chính sách.',
      color: 'from-[#1B3A36] to-[#0D211F]',
    },
    {
      id: 'g6',
      title: 'Bữa cơm dinh dưỡng cuối tuần cho mái ấm',
      category: 'community',
      categoryLabel: 'Community Support',
      location: 'Mái ấm Ánh Sáng, Quận 8, TP.HCM',
      description: 'Tự tay nấu và phục vụ các bữa ăn nóng sốt, đầy đủ dinh dưỡng cho các em nhỏ có hoàn cảnh đặc biệt.',
      color: 'from-[#421D1D] to-[#270E0E]',
    },
  ]

  const filteredItems =
    activeTab === 'all'
      ? galleryItems
      : galleryItems.filter((item) => item.category === activeTab)

  return (
    <section id="gallery" className="py-20 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="text-center max-w-3xl mx-auto space-y-3 mb-10">
        <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
          <span>⚜</span>
          <span>KÝ SỰ & BẢO TỒN HOẠT ĐỘNG THỰC ĐỊA</span>
          <span>⚜</span>
        </div>

        <h2 className="font-poppins text-3xl sm:text-4xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
          Community Activities & Gallery
        </h2>

        <div className="flex items-center justify-center gap-4 my-2 text-[#C5A059]">
          <div className="h-[1px] w-12 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
          <div className="h-[1px] w-12 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>

        <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295]">
          Ghi lại những khoảnh khắc ấm áp và hành trình sẻ chia ý nghĩa của cộng đồng Give-AID cùng các tổ chức đối tác trên khắp dải đất Việt Nam.
        </p>
      </div>

      {/* Tabs lọc hoạt động */}
      <div className="flex flex-wrap items-center justify-center gap-2 mb-12 font-poppins">
        {categories.map((cat) => (
          <button
            key={cat.key}
            type="button"
            onClick={() => setActiveTab(cat.key)}
            className={`px-4 py-2 text-xs font-semibold tracking-wider uppercase transition-all ${
              activeTab === cat.key
                ? 'bg-[#1E3F2F] text-[#FAF7F2] border-2 border-[#C5A059] shadow-md font-bold'
                : 'bg-[#FAF7F2] dark:bg-[#181614] text-[#524B42] dark:text-[#AAA295] border border-[#DCD1BA] dark:border-[#383126] hover:border-[#C5A059]'
            }`}
          >
            {cat.label}
          </button>
        ))}
      </div>

      {/* Grid hình ảnh hoạt động */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-7">
        {filteredItems.map((item) => (
          <div
            key={item.id}
            className="group border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] hover:border-[#C5A059] shadow-sm hover:shadow-xl transition-all flex flex-col justify-between"
          >
            {/* Visual Frame */}
            <div
              className={`aspect-video w-full bg-gradient-to-br ${item.color} p-6 flex flex-col justify-between text-[#FAF7F2] relative overflow-hidden border-b border-[#C5A059]/40`}
            >
              <div className="flex items-center justify-between z-10 font-poppins">
                <span className="text-[10px] uppercase font-bold tracking-wider px-2.5 py-1 border border-[#C5A059] bg-[#142E22]/80">
                  {item.categoryLabel}
                </span>
                <span className="text-[11px] text-[#C5A059] font-medium">✦ FIELD CHRONICLE ✦</span>
              </div>
              <div className="z-10 font-inter">
                <p className="text-xs text-[#E5DDCF] font-semibold">📍 {item.location}</p>
              </div>

              {/* Decorative classical watermarks */}
              <div className="absolute -right-6 -bottom-6 w-28 h-28 border border-[#C5A059]/20 rounded-full pointer-events-none group-hover:scale-110 transition-transform duration-500" />
            </div>

            {/* Chi tiết thông tin */}
            <div className="p-6 space-y-2">
              <h3 className="font-poppins text-lg font-bold text-[#1F1D1A] dark:text-[#F5F2EB] group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors line-clamp-1">
                {item.title}
              </h3>
              <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] line-clamp-2 leading-relaxed font-normal">
                {item.description}
              </p>
            </div>
          </div>
        ))}
      </div>
    </section>
  )
}
