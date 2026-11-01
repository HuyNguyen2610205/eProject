import * as React from 'react'
import { Eye, EyeOff } from 'lucide-react'
import { Input, InputProps } from './input'
import { cn } from '@/utils/cn'

export interface PasswordInputProps extends Omit<InputProps, 'type'> {
  showPasswordLabel?: string
  hidePasswordLabel?: string
}

export const PasswordInput = React.forwardRef<HTMLInputElement, PasswordInputProps>(
  (
    {
      className,
      disabled,
      showPasswordLabel = 'Hiện mật khẩu',
      hidePasswordLabel = 'Ẩn mật khẩu',
      ...props
    },
    ref
  ) => {
    const [showPassword, setShowPassword] = React.useState(false)

    return (
      <div className="relative w-full">
        <Input
          type={showPassword ? 'text' : 'password'}
          className={cn('pr-10', className)}
          ref={ref}
          disabled={disabled}
          {...props}
        />
        <button
          type="button"
          onClick={() => setShowPassword((prev) => !prev)}
          disabled={disabled}
          aria-label={showPassword ? hidePasswordLabel : showPasswordLabel}
          aria-pressed={showPassword}
          tabIndex={0}
          className="absolute right-0 top-0 h-full px-3 text-zinc-400 hover:text-zinc-700 dark:text-zinc-500 dark:hover:text-zinc-200 focus:outline-none focus-visible:text-zinc-900 dark:focus-visible:text-zinc-100 disabled:cursor-not-allowed disabled:opacity-50 transition-colors flex items-center justify-center cursor-pointer"
        >
          {showPassword ? (
            <EyeOff className="h-4 w-4" aria-hidden="true" />
          ) : (
            <Eye className="h-4 w-4" aria-hidden="true" />
          )}
        </button>
      </div>
    )
  }
)

PasswordInput.displayName = 'PasswordInput'
