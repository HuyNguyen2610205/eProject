import React, { useState, useEffect } from 'react'
import { donationService } from '@/services/donationService'
import { contentService } from '@/services/contentService'
import { Cause } from '@/types/donation'
import { Ngo, Programme } from '@/types/content'
import { toast } from 'sonner'
import {
  HeroSection,
  ImpactStatistics,
  CausesSection,
  AboutSection,
  ProgrammesSection,
  WhyChooseUsSection,
  PartnersSection,
  GallerySection,
  CallToActionSection,
  HelpCentreSection,
  FooterSection,
} from '@/components/home'

export function HomePage() {
  // Live State from Backend
  const [causes, setCauses] = useState<Cause[]>([])
  const [ngos, setNgos] = useState<Ngo[]>([])
  const [programmes, setProgrammes] = useState<Programme[]>([])
  const [isLoadingCauses, setIsLoadingCauses] = useState(true)
  const [isLoadingNgos, setIsLoadingNgos] = useState(true)
  const [isLoadingProgrammes, setIsLoadingProgrammes] = useState(true)

  // Contact Form State
  const [contactName, setContactName] = useState('')
  const [contactEmail, setContactEmail] = useState('')
  const [contactSubject, setContactSubject] = useState('')
  const [contactMessage, setContactMessage] = useState('')
  const [isSubmittingContact, setIsSubmittingContact] = useState(false)

  // Newsletter State
  const [newsletterEmail, setNewsletterEmail] = useState('')

  // Tải dữ liệu từ các dịch vụ Backend thông qua API Gateway
  useEffect(() => {
    // 1. Tải danh mục Causes
    donationService
      .getAllCauses()
      .then((res) => {
        if (res.success && res.data) setCauses(res.data)
      })
      .catch((err) => console.error('Lỗi tải Causes:', err))
      .finally(() => setIsLoadingCauses(false))

    // 2. Tải danh sách NGOs
    contentService
      .getNgos()
      .then((res) => {
        if (res.success && res.data) setNgos(res.data)
      })
      .catch((err) => console.error('Lỗi tải NGOs:', err))
      .finally(() => setIsLoadingNgos(false))

    // 3. Tải danh sách Programmes
    contentService
      .getProgrammes({ featuredOnly: true, status: 'Upcoming' })
      .then((res) => {
        if (res.success && res.data) setProgrammes(res.data)
      })
      .catch((err) => console.error('Lỗi tải Programmes:', err))
      .finally(() => setIsLoadingProgrammes(false))
  }, [])

  // Xử lý gửi biểu mẫu liên hệ Help Centre
  const handleContactSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!contactName.trim() || !contactEmail.trim() || !contactSubject.trim() || !contactMessage.trim()) {
      toast.error('Vui lòng điền đầy đủ các thông tin bắt buộc.')
      return
    }

    setIsSubmittingContact(true)
    try {
      const res = await contentService.submitQuery({
        fullName: contactName.trim(),
        email: contactEmail.trim(),
        subject: contactSubject.trim(),
        message: contactMessage.trim(),
      })

      if (res.success) {
        toast.success('Gửi thắc mắc thành công! Đội ngũ Help Centre sẽ phản hồi qua email của bạn trong 24h.')
        setContactName('')
        setContactEmail('')
        setContactSubject('')
        setContactMessage('')
      } else {
        toast.error(res.message || 'Không thể gửi câu hỏi lúc này.')
      }
    } catch {
      toast.error('Không thể kết nối đến máy chủ. Vui lòng thử lại sau.')
    } finally {
      setIsSubmittingContact(false)
    }
  }

  // Xử lý đăng ký nhận tin Newsletter
  const handleNewsletterSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!newsletterEmail.trim()) {
      toast.error('Vui lòng nhập địa chỉ email của bạn.')
      return
    }
    toast.success('Cảm ơn bạn đã đăng ký nhận bản tin hoạt động từ Give-AID!')
    setNewsletterEmail('')
  }

  return (
    <div className="min-h-screen bg-[#FAF7F2] text-[#221F1C] dark:bg-[#121110] dark:text-[#EAE5D9] transition-colors duration-500 font-inter">
      {/* 4.2. Hero Section */}
      <HeroSection />

      {/* 4.3. Impact Statistics */}
      <ImpactStatistics
        causesCount={causes.length || 6}
        ngosCount={ngos.length || 3}
        programmesCount={programmes.length || 3}
      />

      {/* 4.4. Our Causes */}
      <CausesSection causes={causes} isLoading={isLoadingCauses} />

      {/* 4.5. About Give-AID */}
      <AboutSection />

      {/* 4.6. Featured NGO Programmes */}
      <ProgrammesSection programmes={programmes} isLoading={isLoadingProgrammes} />

      {/* 4.7. Why Choose Give-AID? */}
      <WhyChooseUsSection />

      {/* 4.8. Our Partners */}
      <PartnersSection ngos={ngos} isLoading={isLoadingNgos} />

      {/* 4.9. Gallery / Community Activities */}
      <GallerySection />

      {/* 4.10. Call to Action */}
      <CallToActionSection />

      {/* Help Centre / Contact Form */}
      <HelpCentreSection
        contactName={contactName}
        contactEmail={contactEmail}
        contactSubject={contactSubject}
        contactMessage={contactMessage}
        isSubmittingContact={isSubmittingContact}
        onNameChange={setContactName}
        onEmailChange={setContactEmail}
        onSubjectChange={setContactSubject}
        onMessageChange={setContactMessage}
        onSubmit={handleContactSubmit}
      />

      {/* 4.11. Footer */}
      <FooterSection
        newsletterEmail={newsletterEmail}
        onNewsletterEmailChange={setNewsletterEmail}
        onNewsletterSubmit={handleNewsletterSubmit}
      />
    </div>
  )
}
