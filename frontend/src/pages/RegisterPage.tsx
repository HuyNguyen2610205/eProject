import React, { useState, useRef } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { FormField } from '@/components/ui/form-field'
import { Input } from '@/components/ui/input'
import { PasswordInput } from '@/components/ui/password-input'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useAuth } from '@/hooks/useAuth'
import { ROUTES } from '@/routes/route-paths'
import { toast } from 'sonner'
import axios from 'axios'

// Regex chuẩn kiểm tra định dạng email
const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$/

interface FieldErrors {
  fullName?: string
  email?: string
  password?: string
  confirmPassword?: string
  general?: string
}

type FormStatus = 'idle' | 'invalid' | 'loading' | 'success' | 'error'

export function RegisterPage() {
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({})
  const [formStatus, setFormStatus] = useState<FormStatus>('idle')

  // Refs phục vụ tự động focus vào trường lỗi đầu tiên
  const fullNameInputRef = useRef<HTMLInputElement>(null)
  const emailInputRef = useRef<HTMLInputElement>(null)
  const passwordInputRef = useRef<HTMLInputElement>(null)
  const confirmPasswordInputRef = useRef<HTMLInputElement>(null)

  const { register } = useAuth()
  const navigate = useNavigate()

  // Hàm kiểm tra tính hợp lệ của toàn bộ form
  const validateForm = (
    currentFullName: string,
    currentEmail: string,
    currentPass: string,
    currentConfirmPass: string
  ): { isValid: boolean; errors: FieldErrors } => {
    const errors: FieldErrors = {}
    const trimmedName = currentFullName.trim()
    const trimmedEmail = currentEmail.trim()

    // 1. Kiểm tra Họ và tên
    if (!trimmedName) {
      errors.fullName = 'Họ và tên không được để trống.'
    } else if (trimmedName.length < 2) {
      errors.fullName = 'Họ và tên phải có ít nhất 2 ký tự.'
    } else if (trimmedName.length > 100) {
      errors.fullName = 'Họ và tên không được vượt quá 100 ký tự.'
    }

    // 2. Kiểm tra Email
    if (!trimmedEmail) {
      errors.email = 'Email không được để trống.'
    } else if (!EMAIL_REGEX.test(trimmedEmail)) {
      errors.email = 'Email không đúng định dạng.'
    }

    // 3. Kiểm tra Mật khẩu
    if (!currentPass) {
      errors.password = 'Mật khẩu không được để trống.'
    } else if (currentPass.length < 6) {
      errors.password = 'Mật khẩu phải có ít nhất 6 ký tự.'
    } else if (!/\d/.test(currentPass)) {
      errors.password = 'Mật khẩu phải chứa ít nhất một chữ số (0-9).'
    } else if (!/[a-z]/.test(currentPass)) {
      errors.password = 'Mật khẩu phải chứa ít nhất một chữ cái thường (a-z).'
    }

    // 4. Kiểm tra Mật khẩu xác nhận
    if (!currentConfirmPass) {
      errors.confirmPassword = 'Vui lòng nhập mật khẩu xác nhận.'
    } else if (currentPass && currentPass !== currentConfirmPass) {
      errors.confirmPassword = 'Mật khẩu xác nhận không khớp với mật khẩu.'
    }

    return {
      isValid: Object.keys(errors).length === 0,
      errors,
    }
  }

  // Focus vào trường lỗi đầu tiên
  const focusFirstError = (errors: FieldErrors) => {
    if (errors.fullName) {
      fullNameInputRef.current?.focus()
    } else if (errors.email) {
      emailInputRef.current?.focus()
    } else if (errors.password) {
      passwordInputRef.current?.focus()
    } else if (errors.confirmPassword) {
      confirmPasswordInputRef.current?.focus()
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    // Xóa trạng thái lỗi cũ khi bắt đầu submit lần mới
    setFieldErrors({})

    const trimmedFullName = fullName.trim()
    const trimmedEmail = email.trim()

    const { isValid, errors } = validateForm(trimmedFullName, trimmedEmail, password, confirmPassword)

    if (!isValid) {
      setFormStatus('invalid')
      setFieldErrors(errors)
      focusFirstError(errors)
      return
    }

    setFormStatus('loading')

    try {
      await register({
        fullName: trimmedFullName,
        email: trimmedEmail,
        password,
        confirmPassword,
      })

      setFormStatus('success')

      const successMsg = 'Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay.'
      toast.success(successMsg, { duration: 5000 })

      // Điều hướng về trang Đăng nhập và truyền thông điệp thành công cùng email đã đăng ký
      navigate(ROUTES.LOGIN, {
        state: {
          registrationSuccess: true,
          message: successMsg,
          email: trimmedEmail,
        },
        replace: true,
      })
    } catch (err: unknown) {
      setFormStatus('error')
      const newErrors: FieldErrors = {}

      if (axios.isAxiosError(err)) {
        if (err.code === 'ERR_NETWORK') {
          newErrors.general = 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng hoặc thử lại sau.'
        } else if (err.response?.status && err.response.status >= 500) {
          newErrors.general = 'Máy chủ hệ thống đang gặp sự cố. Vui lòng thử lại sau.'
        } else if (err.response?.data) {
          const resData = err.response.data as { message?: string; errors?: string[] }
          const errorList = resData.errors || []

          if (errorList.length > 0) {
            // Ánh xạ lỗi từ Backend vào từng ô tương ứng
            errorList.forEach((errorText) => {
              const lower = errorText.toLowerCase()
              if (lower.includes('email')) {
                newErrors.email = errorText
              } else if (lower.includes('mật khẩu xác nhận') || lower.includes('confirmpassword')) {
                newErrors.confirmPassword = errorText
              } else if (lower.includes('mật khẩu') || lower.includes('password')) {
                newErrors.password = errorText
              } else if (lower.includes('họ và tên') || lower.includes('fullname')) {
                newErrors.fullName = errorText
              } else {
                newErrors.general = errorText
              }
            })
          } else if (resData.message) {
            const lower = resData.message.toLowerCase()
            if (lower.includes('email')) {
              newErrors.email = resData.message
            } else {
              newErrors.general = resData.message
            }
          } else {
            newErrors.general = 'Dữ liệu đăng ký không hợp lệ.'
          }
        } else {
          newErrors.general = err.message || 'Lỗi gửi yêu cầu đăng ký.'
        }
      } else if (err instanceof Error) {
        newErrors.general = err.message
      } else {
        newErrors.general = 'Đã xảy ra lỗi không xác định.'
      }

      setFieldErrors(newErrors)
      focusFirstError(newErrors)
    }
  }

  const isSubmitting = formStatus === 'loading'

  return (
    <Card className="w-full border-2 border-double border-[#C5A059] bg-[#FAF7F2] dark:bg-[#181614] shadow-2xl">
      <CardHeader className="text-center">
        <span className="font-poppins text-xs text-[#8C6D37] dark:text-[#C5A059] tracking-widest uppercase block mb-1">
          ⚜ ANNO MMXXVI ⚜
        </span>
        <CardTitle className="font-poppins text-2xl font-bold text-[#1F1D1A] dark:text-[#F5F2EB]">
          Đăng Ký Tài Khoản
        </CardTitle>
        <CardDescription className="font-inter text-base text-[#615B52] dark:text-[#AAA295]">
          Gia nhập cộng đồng hảo tâm và đồng hành cùng Give-AID
        </CardDescription>
      </CardHeader>

      <form onSubmit={handleSubmit} noValidate>
        <CardContent className="space-y-4">
          {/* Thông báo lỗi chung (kết nối mạng hoặc lỗi server) */}
          {fieldErrors.general && (
            <Alert variant="destructive" role="alert">
              <AlertTitle className="font-poppins text-xs font-semibold">Đăng ký chưa thành công</AlertTitle>
              <AlertDescription className="font-inter text-sm">{fieldErrors.general}</AlertDescription>
            </Alert>
          )}

          <FormField
            id="fullName"
            label="Họ và tên"
            error={fieldErrors.fullName}
            required
          >
            <Input
              ref={fullNameInputRef}
              id="fullName"
              placeholder="Nguyễn Văn A"
              autoComplete="name"
              value={fullName}
              onChange={(e) => {
                setFullName(e.target.value)
                if (fieldErrors.fullName) {
                  setFieldErrors((prev) => ({ ...prev, fullName: undefined }))
                }
              }}
              disabled={isSubmitting}
              className="border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] focus:border-[#C5A059]"
              required
            />
          </FormField>

          <FormField
            id="email"
            label="Địa chỉ Email"
            error={fieldErrors.email}
            required
          >
            <Input
              ref={emailInputRef}
              id="email"
              type="email"
              placeholder="name@example.com"
              autoComplete="email"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value)
                if (fieldErrors.email) {
                  setFieldErrors((prev) => ({ ...prev, email: undefined }))
                }
              }}
              disabled={isSubmitting}
              className="border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] focus:border-[#C5A059]"
              required
            />
          </FormField>

          <FormField
            id="password"
            label="Mật khẩu"
            error={fieldErrors.password}
            helperText="Tối thiểu 6 ký tự, gồm ít nhất 1 chữ thường và 1 chữ số"
            required
          >
            <PasswordInput
              ref={passwordInputRef}
              id="password"
              placeholder="••••••••"
              autoComplete="new-password"
              value={password}
              onChange={(e) => {
                const newPass = e.target.value
                setPassword(newPass)
                if (fieldErrors.password) {
                  setFieldErrors((prev) => ({ ...prev, password: undefined }))
                }
                if (confirmPassword && fieldErrors.confirmPassword && newPass === confirmPassword) {
                  setFieldErrors((prev) => ({ ...prev, confirmPassword: undefined }))
                }
              }}
              disabled={isSubmitting}
              showPasswordLabel="Hiện mật khẩu"
              hidePasswordLabel="Ẩn mật khẩu"
              className="border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] focus:border-[#C5A059]"
              required
            />
          </FormField>

          <FormField
            id="confirmPassword"
            label="Xác nhận mật khẩu"
            error={fieldErrors.confirmPassword}
            required
          >
            <PasswordInput
              ref={confirmPasswordInputRef}
              id="confirmPassword"
              placeholder="Nhập lại mật khẩu"
              autoComplete="new-password"
              value={confirmPassword}
              onChange={(e) => {
                setConfirmPassword(e.target.value)
                if (fieldErrors.confirmPassword) {
                  setFieldErrors((prev) => ({ ...prev, confirmPassword: undefined }))
                }
              }}
              disabled={isSubmitting}
              showPasswordLabel="Hiện mật khẩu xác nhận"
              hidePasswordLabel="Ẩn mật khẩu xác nhận"
              className="border-[#DCD1BA] dark:border-[#383126] bg-white dark:bg-[#121110] focus:border-[#C5A059]"
              required
            />
          </FormField>
        </CardContent>

        <CardFooter className="flex flex-col space-y-4 font-poppins">
          <Button
            type="submit"
            className="w-full py-3.5 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-bold text-xs uppercase tracking-widest border border-[#C5A059] shadow-md cursor-pointer"
            disabled={isSubmitting}
          >
            {isSubmitting ? 'Đang khởi tạo tài khoản...' : 'Đăng ký tài khoản'}
          </Button>

          <p className="text-center text-xs text-[#6B6256] dark:text-[#A69D8F] font-inter">
            Đã có tài khoản?{' '}
            <Link to={ROUTES.LOGIN} className="font-bold text-[#8C6D37] dark:text-[#C5A059] hover:underline font-poppins">
              Đăng nhập
            </Link>
          </p>
        </CardFooter>
      </form>
    </Card>
  )
}
