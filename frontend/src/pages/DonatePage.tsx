import React, { useState, useEffect } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { Cause, DonationResponse } from '@/types/donation'
import { donationService } from '@/services/donationService'
import { formatCurrency, formatDate } from '@/utils/format'
import { ROUTES } from '@/routes/route-paths'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { toast } from 'sonner'
import axios from 'axios'
import {
  Heart,
  Gift,
  CreditCard,
  User,
  CheckCircle2,
  Copy,
  Check,
  ArrowLeft,
  Loader2,
  Sparkles,
} from 'lucide-react'

const PRESET_AMOUNTS = [50000, 100000, 200000, 500000, 1000000, 2000000]

export function DonatePage() {
  const [searchParams] = useSearchParams()
  const { user, isAuthenticated } = useAuth()

  const [causes, setCauses] = useState<Cause[]>([])
  const [isLoadingCauses, setIsLoadingCauses] = useState(true)

  // Form State
  const [selectedCauseId, setSelectedCauseId] = useState<number | ''>('')
  const [donorName, setDonorName] = useState('')
  const [donorEmail, setDonorEmail] = useState('')
  const [donorPhone, setDonorPhone] = useState('')
  const [amount, setAmount] = useState<number>(100000)
  const [customAmountStr, setCustomAmountStr] = useState('100000')
  const [notes, setNotes] = useState('')

  // Mock Card State
  const [cardHolderName, setCardHolderName] = useState('NGUYEN VAN HAO TAM')
  const [cardNumber, setCardNumber] = useState('1234567812345678')
  const [expiryDate, setExpiryDate] = useState('12/28')
  const [cvv, setCvv] = useState('123')

  // UI State
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successDonation, setSuccessDonation] = useState<DonationResponse | null>(null)
  const [copiedCode, setCopiedCode] = useState(false)

  // 1. Tải danh mục Causes và khởi tạo giá trị ban đầu
  useEffect(() => {
    async function initCauses() {
      try {
        const res = await donationService.getAllCauses()
        if (res.success && res.data) {
          setCauses(res.data)

          // Lấy causeId từ URL query param nếu có
          const queryCauseId = searchParams.get('causeId')
          if (queryCauseId) {
            const parsed = parseInt(queryCauseId, 10)
            if (!isNaN(parsed) && res.data.some((c) => c.id === parsed)) {
              setSelectedCauseId(parsed)
            } else if (res.data.length > 0) {
              setSelectedCauseId(res.data[0].id)
            }
          } else if (res.data.length > 0) {
            setSelectedCauseId(res.data[0].id)
          }
        }
      } catch (err) {
        console.error('Không thể tải danh sách mục tiêu:', err)
      } finally {
        setIsLoadingCauses(false)
      }
    }

    initCauses()
  }, [searchParams])

  // 2. Tự động điền thông tin nếu người dùng đã đăng nhập
  useEffect(() => {
    if (isAuthenticated && user) {
      if (user.fullName && !donorName) {
        setDonorName(user.fullName)
      }
      if (user.email && !donorEmail) {
        setDonorEmail(user.email)
      }
    }
  }, [isAuthenticated, user, donorName, donorEmail])

  // Xử lý chọn số tiền định sẵn
  const handleSelectPreset = (value: number) => {
    setAmount(value)
    setCustomAmountStr(value.toString())
  }

  // Xử lý nhập số tiền tùy ý
  const handleCustomAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const raw = e.target.value.replace(/[^0-9]/g, '')
    setCustomAmountStr(raw)
    const parsed = parseInt(raw, 10)
    if (!isNaN(parsed)) {
      setAmount(parsed)
    } else {
      setAmount(0)
    }
  }

  // Nút nhanh điền thông tin thẻ giả lập
  const handleFillMockCard = () => {
    setCardHolderName('NGUYEN VAN HAO TAM')
    setCardNumber('1234567812345678')
    setExpiryDate('12/28')
    setCvv('123')
    toast.info('Đã điền thông tin thẻ mẫu thử nghiệm!')
  }

  // Xử lý sao chép mã giao dịch
  const handleCopyTransactionCode = () => {
    if (successDonation?.transactionCode) {
      navigator.clipboard.writeText(successDonation.transactionCode)
      setCopiedCode(true)
      toast.success('Đã sao chép mã giao dịch vào clipboard!')
      setTimeout(() => setCopiedCode(false), 2500)
    }
  }

  // Xử lý Gửi Quyên Góp
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMessage(null)

    // Validation
    if (!selectedCauseId) {
      setErrorMessage('Vui lòng chọn một mục tiêu quyên góp.')
      return
    }

    if (!donorName.trim()) {
      setErrorMessage('Vui lòng nhập họ và tên của bạn.')
      return
    }

    if (!donorEmail.trim()) {
      setErrorMessage('Vui lòng nhập địa chỉ email để nhận biên lai.')
      return
    }

    if (amount < 10000) {
      setErrorMessage('Số tiền quyên góp tối thiểu là 10.000 VNĐ.')
      return
    }

    if (amount > 100000000) {
      setErrorMessage('Số tiền quyên góp một lần tối đa là 100.000.000 VNĐ.')
      return
    }

    if (!cardHolderName.trim()) {
      setErrorMessage('Vui lòng nhập tên chủ thẻ thanh toán.')
      return
    }

    if (!/^[0-9]{16}$/.test(cardNumber.replace(/\s/g, ''))) {
      setErrorMessage('Số thẻ thanh toán phải có đúng 16 chữ số.')
      return
    }

    if (!/^(0[1-9]|1[0-2])\/?([0-9]{2})$/.test(expiryDate.trim())) {
      setErrorMessage('Hạn thẻ phải theo định dạng MM/YY (ví dụ: 12/28).')
      return
    }

    if (!/^[0-9]{3,4}$/.test(cvv.trim())) {
      setErrorMessage('Mã bảo mật CVV phải có 3 hoặc 4 chữ số.')
      return
    }

    setIsSubmitting(true)

    try {
      const response = await donationService.donate({
        causeId: Number(selectedCauseId),
        donorName: donorName.trim(),
        donorEmail: donorEmail.trim(),
        donorPhone: donorPhone.trim() || undefined,
        amount,
        notes: notes.trim() || undefined,
        cardHolderName: cardHolderName.trim(),
        cardNumber: cardNumber.replace(/\s/g, ''),
        expiryDate: expiryDate.trim(),
        cvv: cvv.trim(),
      })

      if (response.success && response.data) {
        setSuccessDonation(response.data)
        toast.success('Quyên góp thành công! Cảm ơn tấm lòng của bạn.')
      } else {
        setErrorMessage(response.message || 'Giao dịch quyên góp không thành công.')
      }
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        if (err.response?.data?.message) {
          setErrorMessage(err.response.data.message)
        } else if (err.response?.data?.errors) {
          const errors = err.response.data.errors
          const messages = Array.isArray(errors) ? errors : Object.values(errors).flat()
          setErrorMessage(messages.join(' | '))
        } else {
          setErrorMessage('Lỗi xử lý giao dịch từ máy chủ. Vui lòng thử lại.')
        }
      } else {
        setErrorMessage('Đã xảy ra lỗi không xác định. Vui lòng kiểm tra lại.')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="min-h-screen bg-[#FAF7F2] dark:bg-[#121110] text-[#221F1C] dark:text-[#EAE5D9] transition-colors duration-500 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        
        {/* Nút quay lại */}
        <div className="mb-6">
          <Link
            to={ROUTES.CAUSES}
            className="inline-flex items-center text-xs font-poppins font-medium tracking-wider text-[#8C6D37] dark:text-[#C5A059] hover:underline"
          >
            <ArrowLeft className="mr-1.5 h-3.5 w-3.5" />
            <span>Xem tất cả danh mục cứu trợ</span>
          </Link>
        </div>

        {/* 1. Màn hình Thành công (Biên lai quyên góp) */}
        {successDonation ? (
          <Card className="border-4 border-double border-[#C5A059] bg-[#FAF7F2] dark:bg-[#181614] shadow-2xl p-6 sm:p-8">
            <CardHeader className="text-center pb-4">
              <div className="mx-auto w-16 h-16 rounded-full border-2 border-[#C5A059] bg-[#F5EFEB] dark:bg-[#201D1A] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059] mb-4">
                <CheckCircle2 className="h-9 w-9 text-emerald-600" />
              </div>
              <span className="font-poppins text-xs tracking-widest text-[#8C6D37] dark:text-[#C5A059] uppercase block mb-1">
                BIÊN LAI ĐIỆN TỬ • MMXXVI
              </span>
              <CardTitle className="font-poppins text-3xl sm:text-4xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
                Tri Ân Tấm Lòng Vàng
              </CardTitle>
              <CardDescription className="font-inter text-base text-[#615B52] dark:text-[#AAA295] max-w-md mx-auto">
                Cảm ơn bạn đã đồng hành cùng Give-AID. Đóng góp của bạn đã được ghi nhận và đưa vào quỹ điều phối cứu trợ.
              </CardDescription>
            </CardHeader>

            <CardContent className="space-y-6 pt-4 border-t border-[#E6DEC9] dark:border-[#2C2720]">
              
              {/* Hộp Mã giao dịch */}
              <div className="rounded-lg border border-[#D8CDB3] dark:border-[#3A3328] bg-[#F7F2E8]/80 dark:bg-[#1E1B17] p-4 text-center">
                <span className="text-xs font-poppins text-zinc-500 uppercase tracking-wider block mb-1">
                  MÃ GIAO DỊCH TRA CỨU
                </span>
                <div className="flex items-center justify-center gap-2">
                  <span className="font-mono text-xl sm:text-2xl font-bold text-[#8C6D37] dark:text-[#D4AF37] tracking-wider">
                    {successDonation.transactionCode}
                  </span>
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={handleCopyTransactionCode}
                    className="h-8 px-2 text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100"
                  >
                    {copiedCode ? <Check className="h-4 w-4 text-emerald-600" /> : <Copy className="h-4 w-4" />}
                  </Button>
                </div>
              </div>

              {/* Chi tiết biên lai */}
              <div className="space-y-3 text-sm font-inter">
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Mục tiêu quyên góp:</span>
                  <span className="font-bold text-[#1F1D1A] dark:text-[#F5F2EB] font-poppins">{successDonation.causeName}</span>
                </div>
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Số tiền ủng hộ:</span>
                  <span className="font-bold text-xl text-[#8C6D37] dark:text-[#C5A059] font-poppins">
                    {formatCurrency(successDonation.amount)}
                  </span>
                </div>
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Người đóng góp:</span>
                  <span className="font-medium text-[#1F1D1A] dark:text-[#F5F2EB]">{successDonation.donorName}</span>
                </div>
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Email xác nhận:</span>
                  <span className="font-medium text-[#1F1D1A] dark:text-[#F5F2EB]">{successDonation.donorEmail}</span>
                </div>
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Thời gian ghi nhận:</span>
                  <span className="text-zinc-700 dark:text-zinc-300">{formatDate(successDonation.createdAt)}</span>
                </div>
                <div className="flex justify-between py-2 border-b border-[#EFE8D6] dark:border-[#2C2720]">
                  <span className="text-zinc-500">Hình thức thanh toán:</span>
                  <span className="text-zinc-700 dark:text-zinc-300">
                    Thẻ ngân hàng (MockCard •••• {successDonation.cardLastFour})
                  </span>
                </div>
                {successDonation.notes && (
                  <div className="py-2">
                    <span className="text-zinc-500 block mb-1">Lời nhắn:</span>
                    <p className="italic text-zinc-700 dark:text-zinc-300 bg-zinc-100/60 dark:bg-zinc-800/40 p-3 rounded">
                      "{successDonation.notes}"
                    </p>
                  </div>
                )}
              </div>
            </CardContent>

            <CardFooter className="flex flex-col sm:flex-row gap-3 pt-6">
              <Button
                type="button"
                onClick={() => setSuccessDonation(null)}
                className="w-full bg-[#1F1D1A] dark:bg-[#F2ECE1] text-[#FAF7F2] dark:text-[#181614] border border-[#C5A059] font-poppins text-xs uppercase"
              >
                Ủng hộ thêm lượt khác
              </Button>
              <Button asChild variant="outline" className="w-full border-[#C5A059] font-poppins text-xs uppercase">
                <Link to={ROUTES.HOME}>Quay về Trang chủ</Link>
              </Button>
            </CardFooter>
          </Card>
        ) : (
          /* 2. Form Quyên góp */
          <Card className="border-2 border-[#E6DEC9] dark:border-[#2C2720] bg-[#FAF7F2] dark:bg-[#181614] shadow-xl">
            <CardHeader className="text-center border-b border-[#E6DEC9] dark:border-[#2C2720] pb-6">
              <div className="mx-auto w-12 h-12 rounded-full border border-[#C5A059] bg-[#F5EFEB] dark:bg-[#201D1A] flex items-center justify-center text-[#8C6D37] dark:text-[#C5A059] mb-2">
                <Heart className="h-6 w-6 fill-[#C5A059]/20" />
              </div>
              <span className="font-poppins text-xs tracking-widest text-[#8C6D37] dark:text-[#C5A059] uppercase block mb-1">
                GIVE-AID NGO CAMPAIGN
              </span>
              <CardTitle className="font-poppins text-3xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
                Phiếu Đóng Góp Thiện Nguyện
              </CardTitle>
              <CardDescription className="font-inter text-sm text-[#615B52] dark:text-[#AAA295]">
                Điền thông tin và chọn mục tiêu quyên góp để cùng lan tỏa yêu thương.
              </CardDescription>
            </CardHeader>

            <form onSubmit={handleSubmit}>
              <CardContent className="space-y-6 pt-6">
                
                {errorMessage && (
                  <Alert variant="destructive">
                    <AlertDescription className="font-inter">{errorMessage}</AlertDescription>
                  </Alert>
                )}

                {/* Phần 1: Chọn mục tiêu */}
                <div className="space-y-2">
                  <label htmlFor="causeSelect" className="font-poppins font-semibold text-sm flex items-center gap-1.5 text-[#1F1D1A] dark:text-[#F5F2EB]">
                    <Gift className="h-4 w-4 text-[#C5A059]" />
                    <span>Chọn mục tiêu quyên góp <span className="text-red-500">*</span></span>
                  </label>
                  {isLoadingCauses ? (
                    <div className="h-10 rounded border border-zinc-200 animate-pulse bg-zinc-100 dark:bg-zinc-800" />
                  ) : (
                    <select
                      id="causeSelect"
                      value={selectedCauseId}
                      onChange={(e) => setSelectedCauseId(Number(e.target.value))}
                      className="w-full rounded-md border border-[#D8CDB3] dark:border-[#3A3328] bg-white dark:bg-[#1A1816] px-3.5 py-2.5 font-inter text-sm text-[#1F1D1A] dark:text-[#F5F2EB] shadow-sm focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
                    >
                      {causes.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name} — (Đã có {c.totalDonationsCount} lượt ủng hộ)
                        </option>
                      ))}
                    </select>
                  )}
                </div>

                {/* Phần 2: Số tiền đóng góp */}
                <div className="space-y-3 pt-2 border-t border-[#EFE8D6] dark:border-[#2C2720]">
                  <label htmlFor="customAmountInput" className="font-poppins font-semibold text-sm flex items-center gap-1.5 text-[#1F1D1A] dark:text-[#F5F2EB]">
                    <Sparkles className="h-4 w-4 text-[#C5A059]" />
                    <span>Chọn số tiền ủng hộ (VNĐ) <span className="text-red-500">*</span></span>
                  </label>

                  {/* Nút bấm chọn nhanh */}
                  <div className="grid grid-cols-3 sm:grid-cols-6 gap-2">
                    {PRESET_AMOUNTS.map((val) => (
                      <button
                        key={val}
                        type="button"
                        onClick={() => handleSelectPreset(val)}
                        className={`py-2 px-1 text-xs font-poppins font-semibold rounded border transition-all ${
                          amount === val
                            ? 'border-[#C5A059] bg-[#C5A059]/15 text-[#8C6D37] dark:text-[#D4AF37] font-bold shadow-sm'
                            : 'border-[#D8CDB3] dark:border-[#3A3328] bg-white dark:bg-[#1A1816] text-zinc-700 dark:text-zinc-300 hover:border-[#C5A059]'
                        }`}
                      >
                        {formatCurrency(val)}
                      </button>
                    ))}
                  </div>

                  {/* Ô nhập số tiền tùy chọn */}
                  <div className="relative mt-2">
                    <input
                      id="customAmountInput"
                      type="text"
                      placeholder="Hoặc nhập số tiền khác (tối thiểu 10.000 đ)"
                      value={customAmountStr}
                      onChange={handleCustomAmountChange}
                      className="w-full rounded-md border border-[#D8CDB3] dark:border-[#3A3328] bg-white dark:bg-[#1A1816] px-3.5 py-2.5 text-sm font-poppins font-bold text-[#8C6D37] dark:text-[#C5A059] shadow-sm focus:border-[#C5A059] focus:outline-none focus:ring-1 focus:ring-[#C5A059]"
                    />
                    <span className="absolute right-3.5 top-1/2 -translate-y-1/2 text-xs font-semibold text-zinc-400 font-inter">
                      VNĐ
                    </span>
                  </div>
                </div>

                {/* Phần 3: Thông tin người đóng góp */}
                <div className="space-y-4 pt-2 border-t border-[#EFE8D6] dark:border-[#2C2720]">
                  <div className="flex items-center justify-between">
                    <h4 className="font-poppins font-semibold text-sm flex items-center gap-1.5 text-[#1F1D1A] dark:text-[#F5F2EB]">
                      <User className="h-4 w-4 text-[#C5A059]" />
                      <span>Thông tin người quyên góp</span>
                    </h4>
                    {isAuthenticated && (
                      <span className="text-[11px] text-[#8C6D37] dark:text-[#C5A059] font-poppins">
                        ✓ Tự động điền từ tài khoản đã đăng nhập
                      </span>
                    )}
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 font-inter">
                    <FormField id="donorName" label="Họ và tên" required>
                      <Input
                        id="donorName"
                        value={donorName}
                        onChange={(e) => setDonorName(e.target.value)}
                        placeholder="Nguyễn Văn A"
                        disabled={isSubmitting}
                      />
                    </FormField>

                    <FormField id="donorEmail" label="Địa chỉ Email" required>
                      <Input
                        id="donorEmail"
                        type="email"
                        value={donorEmail}
                        onChange={(e) => setDonorEmail(e.target.value)}
                        placeholder="email@example.com"
                        disabled={isSubmitting}
                      />
                    </FormField>
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 font-inter">
                    <FormField id="donorPhone" label="Số điện thoại (tùy chọn)">
                      <Input
                        id="donorPhone"
                        value={donorPhone}
                        onChange={(e) => setDonorPhone(e.target.value)}
                        placeholder="0987654321"
                        disabled={isSubmitting}
                      />
                    </FormField>

                    <FormField id="notes" label="Lời nhắn gửi gắm">
                      <Input
                        id="notes"
                        value={notes}
                        onChange={(e) => setNotes(e.target.value)}
                        placeholder="Chúc các em nhỏ luôn vui khỏe..."
                        disabled={isSubmitting}
                      />
                    </FormField>
                  </div>
                </div>

                {/* Phần 4: Thanh toán thẻ giả lập */}
                <div className="space-y-4 pt-2 border-t border-[#EFE8D6] dark:border-[#2C2720]">
                  <div className="flex items-center justify-between">
                    <h4 className="font-poppins font-semibold text-sm flex items-center gap-1.5 text-[#1F1D1A] dark:text-[#F5F2EB]">
                      <CreditCard className="h-4 w-4 text-[#C5A059]" />
                      <span>Thanh toán thẻ ngân hàng (Môi trường Sandbox)</span>
                    </h4>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={handleFillMockCard}
                      className="h-7 text-xs border-[#C5A059] text-[#8C6D37] dark:text-[#C5A059] font-poppins"
                    >
                      Điền thẻ mẫu
                    </Button>
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 font-inter">
                    <FormField id="cardHolder" label="Tên in trên thẻ" required>
                      <Input
                        id="cardHolder"
                        value={cardHolderName}
                        onChange={(e) => setCardHolderName(e.target.value.toUpperCase())}
                        placeholder="NGUYEN VAN A"
                        disabled={isSubmitting}
                      />
                    </FormField>

                    <FormField id="cardNumber" label="Số thẻ (16 chữ số)" required>
                      <Input
                        id="cardNumber"
                        value={cardNumber}
                        onChange={(e) => setCardNumber(e.target.value.replace(/[^0-9]/g, '').slice(0, 16))}
                        placeholder="1234567812345678"
                        disabled={isSubmitting}
                      />
                    </FormField>
                  </div>

                  <div className="grid grid-cols-2 gap-4 font-inter">
                    <FormField id="expiry" label="Hạn thẻ (MM/YY)" required>
                      <Input
                        id="expiry"
                        value={expiryDate}
                        onChange={(e) => setExpiryDate(e.target.value.slice(0, 5))}
                        placeholder="12/28"
                        disabled={isSubmitting}
                      />
                    </FormField>

                    <FormField id="cvv" label="Mã CVV" required>
                      <Input
                        id="cvv"
                        type="password"
                        value={cvv}
                        onChange={(e) => setCvv(e.target.value.replace(/[^0-9]/g, '').slice(0, 4))}
                        placeholder="123"
                        disabled={isSubmitting}
                      />
                    </FormField>
                  </div>
                </div>

              </CardContent>

              <CardFooter className="flex flex-col space-y-3 pt-6 border-t border-[#E6DEC9] dark:border-[#2C2720]">
                <Button
                  type="submit"
                  disabled={isSubmitting || isLoadingCauses}
                  className="w-full bg-[#1F1D1A] dark:bg-[#F2ECE1] text-[#FAF7F2] dark:text-[#181614] border border-[#C5A059] hover:bg-[#8C6D37] dark:hover:bg-[#C5A059] font-poppins text-sm font-semibold tracking-wider uppercase py-3 transition-colors shadow-lg"
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Đang xử lý giao dịch...
                    </>
                  ) : (
                    `Xác nhận quyên góp ${formatCurrency(amount)}`
                  )}
                </Button>

                <p className="text-[11px] text-center text-zinc-500 font-inter">
                  Giao dịch được bảo mật bởi chuẩn mã hóa TLS và bảo vệ danh tính theo chuẩn OWASP.
                </p>
              </CardFooter>
            </form>
          </Card>
        )}

      </div>
    </div>
  )
}
