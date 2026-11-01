import {
  HeartHandshake,
  Users,
  Building2,
  Calendar,
} from 'lucide-react'

interface ImpactStatisticsProps {
  causesCount?: number
  ngosCount?: number
  programmesCount?: number
}

export function ImpactStatistics({
  causesCount = 6,
  ngosCount = 3,
  programmesCount = 3,
}: ImpactStatisticsProps) {
  const stats = [
    {
      roman: 'I',
      icon: HeartHandshake,
      number: `${causesCount}`,
      title: 'Causes Supported',
      desc: 'Danh mục bảo trợ trọng điểm',
      isLive: true,
    },
    {
      roman: 'II',
      icon: Users,
      number: '12,500+',
      title: 'Community Volunteers',
      desc: 'Lượt tham gia & hỗ trợ',
      isLive: false,
    },
    {
      roman: 'III',
      icon: Building2,
      number: `${ngosCount}+`,
      title: 'NGO Partners',
      desc: 'Tổ chức thiện nguyện liên kết',
      isLive: true,
    },
    {
      roman: 'IV',
      icon: Calendar,
      number: `${programmesCount}+`,
      title: 'Programmes Organized',
      desc: 'Chiến dịch cộng đồng triển khai',
      isLive: true,
    },
  ]

  return (
    <section className="py-14 bg-[#F4EFE6] dark:bg-[#181614] border-b border-[#E2D9C8] dark:border-[#2D2720]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-6 lg:gap-8">
          {stats.map((s, idx) => {
            const Icon = s.icon
            return (
              <div
                key={idx}
                className="p-6 border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#141210] text-center space-y-2 hover:border-[#C5A059] transition-all relative group shadow-sm"
              >
                {/* Roman Numeral Header */}
                <div className="font-poppins text-xs font-bold text-[#8C6D37] dark:text-[#C5A059] tracking-wider">
                  — {s.roman} —
                </div>

                <div className="w-10 h-10 mx-auto border border-[#C5A059]/40 bg-[#F4EFE6] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] flex items-center justify-center">
                  <Icon className="h-5 w-5" />
                </div>

                <div className="font-poppins text-3xl font-extrabold text-[#1F1D1A] dark:text-[#F5F2EB]">
                  {s.number}
                </div>

                <div className="font-poppins text-xs font-bold text-[#8C6D37] dark:text-[#C5A059] uppercase tracking-wider">
                  {s.title}
                </div>

                <p className="font-inter text-xs text-[#6B6256] dark:text-[#AAA295]">
                  {s.desc}
                </p>

                {!s.isLive && (
                  <span className="inline-block text-[10px] font-inter text-[#8C6D37] dark:text-[#C5A059] italic pt-1">
                    *Số liệu minh họa mẫu
                  </span>
                )}
              </div>
            )
          })}
        </div>
      </div>
    </section>
  )
}
