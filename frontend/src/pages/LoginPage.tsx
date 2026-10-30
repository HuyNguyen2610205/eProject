import React, { useState, useEffect, useRef } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { Input } from '@/components/ui/input'
import { PasswordInput } from '@/components/ui/password-input'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/routes/route-paths'
import axios from 'axios'

interface NavigationState {
  from?: { pathname?: string }
  registrationSuccess?: boolean
  message?: string
  email?: string
}

export function LoginPage() {
  const location = useLocation()
  const navState = location.state as NavigationState | null

  const [email, setEmail] = useState(() => navState?.email || '')
  const [password, setPassword] = useState('')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(() => {
    if (navState?.message) return navState.message
    if (navState?.registrationSuccess) return 'Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay.'
    return null
  })
  const [isSubmitting, setIsSubmitting] = useState(false)

  const passwordInputRef = useRef<HTMLInputElement>(null)
  const { login } = useAuth()
  const navigate = useNavigate()

  const from = navState?.from?.pathname || ROUTES.HOME

  // Khi có email được truyền từ trang đăng ký, tự động focus vào ô mật khẩu
  useEffect(() => {
    if (navState?.email && passwordInputRef.current) {
      passwordInputRef.current.focus()
    }
  }, [navState?.email])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null) // Xóa thông báo thành công cũ để không hiển thị song song với lỗi

    if (!email.trim() || !password.trim()) {
      setErrorMessage('Vui lòng nhập đầy đủ email và mật khẩu.')
      return
    }

    setIsSubmitting(true)

    try {
      await login({ email: email.trim(), password })
      navigate(from, { replace: true })
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        setErrorMessage(err.response.data.message)
      } else if (err instanceof Error) {
        setErrorMessage(err.message)
      } else {
        setErrorMessage('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin kết nối.')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Đăng nhập</CardTitle>
        <CardDescription>Nhập thông tin xác thực để truy cập vào hệ thống</CardDescription>
      </CardHeader>

      <form onSubmit={handleSubmit}>
        <CardContent className="space-y-4">
          {/* Thông báo thành công từ luồng đăng ký hoặc tác vụ khác */}
          {successMessage && !errorMessage && (
            <Alert variant="success" role="status">
              <AlertDescription>{successMessage}</AlertDescription>
            </Alert>
          )}

          {/* Thông báo lỗi đăng nhập */}
          {errorMessage && (
            <Alert variant="destructive" role="alert">
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}

          <FormField id="email" label="Email" required>
            <Input
              id="email"
              type="email"
              placeholder="name@example.com"
              autoComplete="email"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value)
                if (errorMessage) setErrorMessage(null)
              }}
              disabled={isSubmitting}
              required
            />
          </FormField>

          <FormField id="password" label="Mật khẩu" required>
            <PasswordInput
              ref={passwordInputRef}
              id="password"
              placeholder="••••••••"
              autoComplete="current-password"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value)
                if (errorMessage) setErrorMessage(null)
              }}
              disabled={isSubmitting}
              showPasswordLabel="Hiện mật khẩu"
              hidePasswordLabel="Ẩn mật khẩu"
              required
            />
          </FormField>

          <div className="flex justify-end">
            <Link to={ROUTES.FORGOT_PASSWORD} className="text-xs text-zinc-600 hover:underline dark:text-zinc-400">
              Quên mật khẩu?
            </Link>
          </div>
        </CardContent>

        <CardFooter className="flex flex-col space-y-3">
          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting ? 'Đang xác thực...' : 'Đăng nhập'}
          </Button>

          <p className="text-center text-xs text-zinc-500">
            Chưa có tài khoản?{' '}
            <Link to={ROUTES.REGISTER} className="font-semibold text-zinc-900 hover:underline dark:text-zinc-100">
              Đăng ký
            </Link>
          </p>
        </CardFooter>
      </form>
    </Card>
  )
}
