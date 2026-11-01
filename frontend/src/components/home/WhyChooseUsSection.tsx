import {
  Heart,
  Compass,
  Users2,
  Share2,
} from 'lucide-react'

export function WhyChooseUsSection() {
  const features = [
    {
      numeral: 'I',
      icon: Heart,
      title: 'Support Meaningful Causes',
      desc: 'Bảo trợ xã hội thiết thực theo 6 danh mục trọng điểm: Trẻ em, Giáo dục, Người khuyết tật, Phụ nữ, Thanh niên và Người cao tuổi.',
    },
    {
      numeral: 'II',
      icon: Compass,
      title: 'Discover NGO Programmes',
      desc: 'Khám phá và đồng hành cùng các chiến dịch cứu trợ thực địa, chương trình học bổng do các tổ chức thiện nguyện uy tín chủ trì.',
    },
    {
      numeral: 'III',
      icon: Users2,
      title: 'Connect with Communities',
      desc: 'Gắn kết nhà hảo tâm, tình nguyện viên và các cộng đồng địa phương cùng chia sẻ một mục tiêu chung vì một tương lai ấm no, tươi sáng.',
    },
    {
      numeral: 'IV',
      icon: Share2,
      title: 'Encourage Collective Giving',
      desc: 'Mỗi đóng góp dù nhỏ bé đều được trân trọng và góp phần tạo nên nguồn sức mạnh to lớn khi toàn xã hội cùng chung tay hành động.',
    },
  ]

  return (
    <section className="py-20 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="text-center max-w-3xl mx-auto space-y-3 mb-14">
        <div className="inline-flex items-center gap-2 px-3 py-1 border border-[#C5A059] bg-[#F7F2E8] dark:bg-[#1E1B17] text-[#8C6D37] dark:text-[#C5A059] text-xs font-poppins font-semibold tracking-wider uppercase">
          <span>⚜</span>
          <span>NGUYÊN TẮC PHỤNG SỰ CỘNG ĐỒNG</span>
          <span>⚜</span>
        </div>

        <h2 className="font-poppins text-3xl sm:text-4xl font-bold tracking-tight text-[#1F1D1A] dark:text-[#F5F2EB]">
          Why Choose Give-AID?
        </h2>

        <div className="flex items-center justify-center gap-4 my-2 text-[#C5A059]">
          <div className="h-[1px] w-12 bg-gradient-to-r from-transparent to-[#C5A059]" />
          <span className="text-xs font-serif select-none">❧ ❖ ☙</span>
          <div className="h-[1px] w-12 bg-gradient-to-l from-transparent to-[#C5A059]" />
        </div>

        <p className="font-inter text-base text-[#524B42] dark:text-[#AAA295]">
          Chúng tôi nỗ lực gìn giữ các giá trị nhân văn cốt lõi, kiến tạo nền tảng thiện nguyện an toàn, chính trực và thuận tiện nhất.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
        {features.map((f, idx) => {
          const Icon = f.icon
          return (
            <div
              key={idx}
              className="p-7 border border-[#DCD1BA] dark:border-[#383126] bg-[#FAF7F2] dark:bg-[#181614] hover:border-[#C5A059] transition-all flex flex-col justify-between group shadow-sm"
            >
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <div className="w-12 h-12 border border-[#C5A059] bg-[#F4EFE6] dark:bg-[#201D1A] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059]">
                    <Icon className="h-6 w-6" />
                  </div>
                  <span className="font-poppins text-xs font-bold text-[#8C6D37] dark:text-[#C5A059] tracking-wider">
                    — {f.numeral} —
                  </span>
                </div>

                <h3 className="font-poppins text-sm font-bold text-[#1F1D1A] dark:text-[#F5F2EB] uppercase tracking-wider group-hover:text-[#8C6D37] dark:group-hover:text-[#C5A059] transition-colors">
                  {f.title}
                </h3>

                <p className="font-inter text-sm text-[#524B42] dark:text-[#AAA295] leading-relaxed font-normal">
                  {f.desc}
                </p>
              </div>
            </div>
          )
        })}
      </div>
    </section>
  )
}
