import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { PageHeader } from '@/components/ui/page-header'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { PasswordInput } from '@/components/ui/password-input'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useAuth } from '@/hooks/useAuth'
import { authService } from '@/services/authService'
import { RefreshCw, LogOut, User, KeyRound, Loader2, CheckCircle2 } from 'lucide-react'
import { ROUTES } from '@/routes/route-paths'
import { toast } from 'sonner'
import axios from 'axios'

export function ProfilePage() {
  const { user, refreshProfile, logout } = useAuth()
  const [isRefreshing, setIsRefreshing] = useState(false)
  const [isLoggingOut, setIsLoggingOut] = useState(false)
  const navigate = useNavigate()

  // State cho form Đổi mật khẩu
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmNewPassword, setConfirmNewPassword] = useState('')
  const [isSubmittingPassword, setIsSubmittingPassword] = useState(false)
  const [passwordError, setPasswordError] = useState<string | null>(null)
  const [passwordSuccess, setPasswordSuccess] = useState<string | null>(null)

  const handleRefresh = async () => {
    setIsRefreshing(true)
    try {
      await refreshProfile()
    } finally {
      setIsRefreshing(false)
    }
  }

  const handleLogout = async () => {
    setIsLoggingOut(true)
    try {
      await logout()
      navigate(ROUTES.LOGIN, { replace: true })
    } finally {
      setIsLoggingOut(false)
    }
  }

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault()
    setPasswordError(null)
    setPasswordSuccess(null)

    // 1. Kiểm tra Mật khẩu hiện tại
    if (!currentPassword) {
      setPasswordError('Vui lòng nhập mật khẩu hiện tại.')
      return
    }

    // 2. Kiểm tra Mật khẩu mới
    if (!newPassword) {
      setPasswordError('Vui lòng nhập mật khẩu mới.')
      return
    }
    if (newPassword.length < 6) {
      setPasswordError('Mật khẩu mới phải có ít nhất 6 ký tự.')
      return
    }
    if (newPassword === currentPassword) {
      setPasswordError('Mật khẩu mới không được trùng với mật khẩu hiện tại.')
      return
    }

    // 3. Kiểm tra Xác nhận mật khẩu mới
    if (!confirmNewPassword) {
      setPasswordError('Vui lòng xác nhận mật khẩu mới.')
      return
    }
    if (newPassword !== confirmNewPassword) {
      setPasswordError('Mật khẩu xác nhận không khớp với mật khẩu mới.')
      return
    }

    setIsSubmittingPassword(true)

    try {
      const response = await authService.changePassword({
        currentPassword,
        newPassword,
        confirmNewPassword,
      })

      if (response.success) {
        const msg = response.message || 'Đổi mật khẩu thành công!'
        setPasswordSuccess(msg)
        toast.success(msg)

        // Reset các ô nhập mật khẩu sau khi đổi thành công
        setCurrentPassword('')
        setNewPassword('')
        setConfirmNewPassword('')
      } else {
        setPasswordError(response.message || 'Đổi mật khẩu không thành công.')
      }
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        if (err.code === 'ERR_NETWORK') {
          setPasswordError('Không thể kết nối đến máy chủ. Vui lòng thử lại sau.')
        } else if (err.response?.status && err.response.status >= 500) {
          setPasswordError('Máy chủ hệ thống đang gặp sự cố. Vui lòng thử lại sau.')
        } else if (err.response?.data) {
          const resData = err.response.data as { message?: string; errors?: string[] }
          if (resData.errors && resData.errors.length > 0) {
            setPasswordError(resData.errors.join(' | '))
          } else if (resData.message) {
            setPasswordError(resData.message)
          } else {
            setPasswordError('Mật khẩu hiện tại không chính xác hoặc dữ liệu không hợp lệ.')
          }
        } else {
          setPasswordError('Đã xảy ra lỗi khi gửi yêu cầu đổi mật khẩu.')
        }
      } else {
        setPasswordError('Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.')
      }
    } finally {
      setIsSubmittingPassword(false)
    }
  }

  if (!user) {
    return (
      <div className="max-w-3xl mx-auto py-8 text-center">
        <p className="text-sm text-zinc-500">Đang tải thông tin hồ sơ tài khoản...</p>
      </div>
    )
  }

  return (
    <div className="space-y-6 max-w-3xl mx-auto">
      <PageHeader
        title="Hồ sơ tài khoản"
        actions={
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handleRefresh}
              disabled={isRefreshing}
            >
              <RefreshCw className={`h-3.5 w-3.5 mr-1.5 ${isRefreshing ? 'animate-spin' : ''}`} />
              Làm mới
            </Button>
            <Button
              variant="destructive"
              size="sm"
              onClick={handleLogout}
              disabled={isLoggingOut}
            >
              <LogOut className="h-3.5 w-3.5 mr-1.5" />
              Đăng xuất
            </Button>
          </div>
        }
      />

      {/* Thông tin cá nhân */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="text-base flex items-center gap-2">
                <User className="h-4 w-4 text-zinc-500" />
                Thông tin cá nhân
              </CardTitle>
            </div>
            <div className="flex gap-1.5">
              {user.roles && user.roles.length > 0 ? (
                user.roles.map((role) => (
                  <Badge key={role} variant={role === 'Admin' ? 'default' : 'secondary'}>
                    {role}
                  </Badge>
                ))
              ) : (
                <Badge variant="secondary">User</Badge>
              )}
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-3 text-xs">
          <div className="flex justify-between py-2 border-b border-zinc-100 dark:border-zinc-800">
            <span className="text-zinc-500">Họ và tên:</span>
            <span className="font-semibold text-zinc-900 dark:text-zinc-100" data-testid="profile-fullname">
              {user.fullName || '—'}
            </span>
          </div>
          <div className="flex justify-between py-2">
            <span className="text-zinc-500">Địa chỉ Email:</span>
            <span className="font-medium text-zinc-900 dark:text-zinc-100" data-testid="profile-email">
              {user.email || '—'}
            </span>
          </div>
        </CardContent>
      </Card>

      {/* Đổi mật khẩu */}
      <Card>
        <CardHeader>
          <CardTitle className="text-base flex items-center gap-2">
            <KeyRound className="h-4 w-4 text-zinc-500" />
            Đổi mật khẩu
          </CardTitle>
          <CardDescription>
            Để bảo vệ an toàn cho tài khoản, hãy cập nhật mật khẩu định kỳ với tối thiểu 6 ký tự.
          </CardDescription>
        </CardHeader>

        <form onSubmit={handleChangePassword}>
          <CardContent className="space-y-4">
            {passwordSuccess && (
              <Alert variant="success">
                <CheckCircle2 className="h-4 w-4" />
                <AlertTitle className="font-semibold text-emerald-800 dark:text-emerald-300">
                  Thành công
                </AlertTitle>
                <AlertDescription className="text-emerald-700 dark:text-emerald-400">
                  {passwordSuccess}
                </AlertDescription>
              </Alert>
            )}

            {passwordError && (
              <Alert variant="destructive">
                <AlertDescription>{passwordError}</AlertDescription>
              </Alert>
            )}

            <FormField id="currentPassword" label="Mật khẩu hiện tại" required>
              <PasswordInput
                id="currentPassword"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                placeholder="Nhập mật khẩu đang sử dụng"
                disabled={isSubmittingPassword}
                autoComplete="current-password"
              />
            </FormField>

            <FormField id="newPassword" label="Mật khẩu mới" required>
              <PasswordInput
                id="newPassword"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                placeholder="Tối thiểu 6 ký tự"
                disabled={isSubmittingPassword}
                autoComplete="new-password"
              />
            </FormField>

            <FormField id="confirmNewPassword" label="Xác nhận mật khẩu mới" required>
              <PasswordInput
                id="confirmNewPassword"
                value={confirmNewPassword}
                onChange={(e) => setConfirmNewPassword(e.target.value)}
                placeholder="Nhập lại mật khẩu mới"
                disabled={isSubmittingPassword}
                autoComplete="new-password"
              />
            </FormField>
          </CardContent>

          <CardFooter className="flex justify-end pt-2">
            <Button type="submit" disabled={isSubmittingPassword}>
              {isSubmittingPassword ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Đang cập nhật...
                </>
              ) : (
                'Cập nhật mật khẩu'
              )}
            </Button>
          </CardFooter>
        </form>
      </Card>

      <div className="text-center pt-2">
        <Button asChild variant="ghost" size="sm">
          <Link to={ROUTES.HOME}>← Quay lại Trang chủ</Link>
        </Button>
      </div>
    </div>
  )
}
