import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { authService } from '@/services/authService'
import { ROUTES } from '@/routes/route-paths'
import { toast } from 'sonner'
import axios from 'axios'
import { Loader2, ArrowLeft, KeyRound, Copy, Check, ArrowRight } from 'lucide-react'

// Regex chuẩn kiểm tra định dạng email
const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$/

export function ForgotPasswordPage() {
  const [email, setEmail] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successData, setSuccessData] = useState<{
    message: string
    email?: string
    resetToken?: string
  } | null>(null)
  const [copied, setCopied] = useState(false)

  const navigate = useNavigate()

  const handleCopyToken = () => {
    if (successData?.resetToken) {
      navigator.clipboard.writeText(successData.resetToken)
      setCopied(true)
      toast.success('Đã sao chép mã xác thực vào clipboard!')
      setTimeout(() => setCopied(false), 2500)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMessage(null)

    const trimmedEmail = email.trim()

    if (!trimmedEmail) {
      setErrorMessage('Vui lòng nhập địa chỉ email của bạn.')
      return
    }

    if (!EMAIL_REGEX.test(trimmedEmail)) {
      setErrorMessage('Địa chỉ email không đúng định dạng.')
      return
    }

    setIsSubmitting(true)

    try {
      const response = await authService.forgotPassword({ email: trimmedEmail })

      const data = response.data
      setSuccessData({
        message: data?.message || response.message || 'Yêu cầu khôi phục mật khẩu đã được tiếp nhận.',
        email: data?.email || trimmedEmail,
        resetToken: data?.resetToken,
      })

      toast.success('Yêu cầu đặt lại mật khẩu đã được xử lý.')
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        if (err.code === 'ERR_NETWORK') {
          setErrorMessage('Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng hoặc thử lại sau.')
        } else if (err.response?.status && err.response.status >= 500) {
          setErrorMessage('Máy chủ hệ thống đang gặp sự cố. Vui lòng thử lại sau.')
        } else if (err.response?.data?.message) {
          setErrorMessage(err.response.data.message)
        } else {
          setErrorMessage('Không thể gửi yêu cầu đặt lại mật khẩu. Vui lòng thử lại sau.')
        }
      } else {
        setErrorMessage('Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleGoToReset = () => {
    if (successData) {
      const targetUrl = `${ROUTES.RESET_PASSWORD}?email=${encodeURIComponent(
        successData.email || email.trim()
      )}${successData.resetToken ? `&token=${encodeURIComponent(successData.resetToken)}` : ''}`
      navigate(targetUrl)
    }
  }

  return (
    <Card className="w-full shadow-md">
      <CardHeader>
        <CardTitle className="text-xl font-bold tracking-tight">Quên mật khẩu</CardTitle>
        <CardDescription>
          Nhập địa chỉ email liên kết với tài khoản để nhận hướng dẫn khôi phục mật khẩu.
        </CardDescription>
      </CardHeader>

      {successData ? (
        <CardContent className="space-y-4">
          <Alert variant="success">
            <AlertTitle className="font-semibold text-emerald-800 dark:text-emerald-300">
              Yêu cầu đã được tiếp nhận
            </AlertTitle>
            <AlertDescription className="text-emerald-700 dark:text-emerald-400 mt-1">
              {successData.message}
            </AlertDescription>
          </Alert>

          {/* Hỗ trợ Dev Mode: Hiển thị token trực tiếp khi chưa tích hợp dịch vụ gửi Mail SMTP */}
          {successData.resetToken && (
            <div className="rounded-lg border border-amber-200 bg-amber-50/70 p-3.5 text-xs text-amber-900 dark:border-amber-900/50 dark:bg-amber-950/30 dark:text-amber-200 space-y-2.5">
              <div className="flex items-center gap-1.5 font-semibold text-amber-800 dark:text-amber-300">
                <KeyRound className="h-4 w-4" />
                <span>Môi trường thử nghiệm (Dev Token)</span>
              </div>
              <p className="leading-relaxed">
                Hệ thống đã tự động tạo mã xác thực khôi phục mật khẩu cho tài khoản <strong>{successData.email}</strong>:
              </p>
              <div className="flex items-center gap-2">
                <input
                  type="text"
                  readOnly
                  value={successData.resetToken}
                  className="w-full rounded border border-amber-300 bg-white px-2.5 py-1.5 font-mono text-[11px] text-zinc-800 select-all dark:border-amber-800 dark:bg-zinc-900 dark:text-zinc-200"
                />
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleCopyToken}
                  className="h-8 shrink-0 px-2.5 text-xs border-amber-300 dark:border-amber-800"
                >
                  {copied ? (
                    <Check className="h-3.5 w-3.5 text-emerald-600" />
                  ) : (
                    <Copy className="h-3.5 w-3.5" />
                  )}
                </Button>
              </div>

              <Button
                type="button"
                onClick={handleGoToReset}
                className="w-full mt-2 bg-amber-600 hover:bg-amber-700 text-white font-medium"
              >
                <span>Tiến hành Đặt lại mật khẩu ngay</span>
                <ArrowRight className="ml-1.5 h-4 w-4" />
              </Button>
            </div>
          )}

          <div className="pt-2 flex flex-col space-y-2">
            {!successData.resetToken && (
              <Button
                type="button"
                onClick={handleGoToReset}
                className="w-full"
              >
                Nhập mã xác thực để đặt lại mật khẩu
              </Button>
            )}

            <Button
              type="button"
              variant="outline"
              onClick={() => {
                setSuccessData(null)
                setEmail('')
              }}
              className="w-full text-xs"
            >
              Gửi lại với email khác
            </Button>
          </div>

          <div className="text-center pt-2">
            <Link
              to={ROUTES.LOGIN}
              className="inline-flex items-center text-xs text-zinc-600 hover:underline dark:text-zinc-400"
            >
              <ArrowLeft className="mr-1 h-3.5 w-3.5" />
              Quay lại Đăng nhập
            </Link>
          </div>
        </CardContent>
      ) : (
        <form onSubmit={handleSubmit}>
          <CardContent className="space-y-4">
            {errorMessage && (
              <Alert variant="destructive">
                <AlertDescription>{errorMessage}</AlertDescription>
              </Alert>
            )}

            <FormField id="email" label="Email đăng ký" required>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@example.com"
                disabled={isSubmitting}
                autoFocus
                autoComplete="email"
              />
            </FormField>
          </CardContent>

          <CardFooter className="flex flex-col space-y-3">
            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Đang gửi yêu cầu...
                </>
              ) : (
                'Gửi yêu cầu'
              )}
            </Button>

            <div className="flex flex-col space-y-2 pt-1 text-center text-xs">
              <Link
                to={ROUTES.RESET_PASSWORD}
                className="text-zinc-600 hover:underline dark:text-zinc-400"
              >
                Đã có mã xác thực? Đặt lại mật khẩu
              </Link>

              <Link
                to={ROUTES.LOGIN}
                className="inline-flex items-center justify-center text-zinc-600 hover:underline dark:text-zinc-400"
              >
                <ArrowLeft className="mr-1 h-3.5 w-3.5" />
                Quay lại Đăng nhập
              </Link>
            </div>
          </CardFooter>
        </form>
      )}
    </Card>
  )
}
