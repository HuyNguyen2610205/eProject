import React, { useState, useEffect } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { Input } from '@/components/ui/input'
import { PasswordInput } from '@/components/ui/password-input'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { authService } from '@/services/authService'
import { ROUTES } from '@/routes/route-paths'
import { toast } from 'sonner'
import axios from 'axios'
import { Loader2, ArrowLeft, CheckCircle2, KeyRound } from 'lucide-react'

// Regex chuẩn kiểm tra định dạng email
const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$/

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [token, setToken] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmNewPassword, setConfirmNewPassword] = useState('')

  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [isSuccess, setIsSuccess] = useState(false)

  // Khởi tạo giá trị email và token từ query parameters (nếu được truyền từ link hoặc trang ForgotPassword)
  useEffect(() => {
    const queryEmail = searchParams.get('email')
    const queryToken = searchParams.get('token')

    if (queryEmail) {
      setEmail(queryEmail)
    }
    if (queryToken) {
      setToken(queryToken)
    }
  }, [searchParams])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMessage(null)

    const trimmedEmail = email.trim()
    const trimmedToken = token.trim()

    // 1. Kiểm tra Email
    if (!trimmedEmail) {
      setErrorMessage('Vui lòng nhập địa chỉ email của bạn.')
      return
    }
    if (!EMAIL_REGEX.test(trimmedEmail)) {
      setErrorMessage('Địa chỉ email không đúng định dạng.')
      return
    }

    // 2. Kiểm tra Mã xác thực
    if (!trimmedToken) {
      setErrorMessage('Vui lòng nhập mã xác thực (Token) được cấp.')
      return
    }

    // 3. Kiểm tra Mật khẩu mới
    if (!newPassword) {
      setErrorMessage('Vui lòng nhập mật khẩu mới.')
      return
    }
    if (newPassword.length < 6) {
      setErrorMessage('Mật khẩu mới phải có ít nhất 6 ký tự.')
      return
    }

    // 4. Kiểm tra Mật khẩu xác nhận
    if (!confirmNewPassword) {
      setErrorMessage('Vui lòng xác nhận mật khẩu mới.')
      return
    }
    if (newPassword !== confirmNewPassword) {
      setErrorMessage('Mật khẩu xác nhận không khớp với mật khẩu mới.')
      return
    }

    setIsSubmitting(true)

    try {
      const response = await authService.resetPassword({
        email: trimmedEmail,
        token: trimmedToken,
        newPassword,
        confirmNewPassword,
      })

      if (response.success) {
        setIsSuccess(true)
        toast.success('Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay.')
      } else {
        setErrorMessage(response.message || 'Đặt lại mật khẩu không thành công.')
      }
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        if (err.code === 'ERR_NETWORK') {
          setErrorMessage('Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng hoặc thử lại sau.')
        } else if (err.response?.status && err.response.status >= 500) {
          setErrorMessage('Máy chủ hệ thống đang gặp sự cố. Vui lòng thử lại sau.')
        } else if (err.response?.data) {
          const resData = err.response.data as { message?: string; errors?: string[] }
          if (resData.errors && resData.errors.length > 0) {
            setErrorMessage(resData.errors.join(' | '))
          } else if (resData.message) {
            setErrorMessage(resData.message)
          } else {
            setErrorMessage('Mã xác thực không hợp lệ hoặc đã hết hạn.')
          }
        } else {
          setErrorMessage('Lỗi đặt lại mật khẩu. Vui lòng kiểm tra lại thông tin.')
        }
      } else {
        setErrorMessage('Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleNavigateToLogin = () => {
    navigate(ROUTES.LOGIN, {
      state: {
        email: email.trim(),
        message: 'Đặt lại mật khẩu thành công! Vui lòng đăng nhập bằng mật khẩu mới của bạn.',
      },
      replace: true,
    })
  }

  return (
    <Card className="w-full shadow-md">
      <CardHeader>
        <div className="flex items-center space-x-2">
          <KeyRound className="h-5 w-5 text-zinc-700 dark:text-zinc-300" />
          <CardTitle className="text-xl font-bold tracking-tight">Đặt lại mật khẩu</CardTitle>
        </div>
        <CardDescription>
          Nhập mã xác thực và thiết lập mật khẩu mới cho tài khoản của bạn.
        </CardDescription>
      </CardHeader>

      {isSuccess ? (
        <CardContent className="space-y-4 pt-2">
          <Alert variant="success">
            <CheckCircle2 className="h-4 w-4" />
            <AlertTitle className="font-semibold text-emerald-800 dark:text-emerald-300">
              Đặt lại mật khẩu thành công!
            </AlertTitle>
            <AlertDescription className="text-emerald-700 dark:text-emerald-400 mt-1">
              Mật khẩu cho tài khoản <strong>{email}</strong> đã được cập nhật thành công. Tất cả phiên đăng nhập cũ đã được thu hồi vì lý do bảo mật.
            </AlertDescription>
          </Alert>

          <Button
            type="button"
            onClick={handleNavigateToLogin}
            className="w-full mt-4"
          >
            Đăng nhập ngay bằng mật khẩu mới
          </Button>

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

            <FormField id="email" label="Email tài khoản" required>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@example.com"
                disabled={isSubmitting}
                autoComplete="email"
              />
            </FormField>

            <FormField id="token" label="Mã xác thực (Reset Token)" required>
              <Input
                id="token"
                type="text"
                value={token}
                onChange={(e) => setToken(e.target.value)}
                placeholder="Dán mã xác thực nhận được tại đây"
                disabled={isSubmitting}
                autoComplete="off"
              />
            </FormField>

            <FormField id="newPassword" label="Mật khẩu mới" required>
              <PasswordInput
                id="newPassword"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                placeholder="Tối thiểu 6 ký tự"
                disabled={isSubmitting}
                autoComplete="new-password"
              />
            </FormField>

            <FormField id="confirmNewPassword" label="Xác nhận mật khẩu mới" required>
              <PasswordInput
                id="confirmNewPassword"
                value={confirmNewPassword}
                onChange={(e) => setConfirmNewPassword(e.target.value)}
                placeholder="Nhập lại mật khẩu mới"
                disabled={isSubmitting}
                autoComplete="new-password"
              />
            </FormField>
          </CardContent>

          <CardFooter className="flex flex-col space-y-3">
            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Đang cập nhật mật khẩu...
                </>
              ) : (
                'Cập nhật mật khẩu'
              )}
            </Button>

            <div className="flex flex-col space-y-2 pt-1 text-center text-xs">
              <Link
                to={ROUTES.FORGOT_PASSWORD}
                className="text-zinc-600 hover:underline dark:text-zinc-400"
              >
                Chưa có mã xác thực? Gửi yêu cầu cấp lại
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
