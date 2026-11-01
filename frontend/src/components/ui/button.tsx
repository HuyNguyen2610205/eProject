import * as React from 'react'
import { Loader2 } from 'lucide-react'
import { cn } from '@/utils/cn'
import {
  buttonVariants,
  type ButtonVariant,
  type ButtonSize,
} from './button-variants'

export type { ButtonVariant, ButtonSize }

export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant
  size?: ButtonSize
  isLoading?: boolean
  asChild?: boolean
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      className,
      variant = 'default',
      size = 'default',
      isLoading = false,
      asChild = false,
      children,
      disabled,
      ...props
    },
    ref
  ) => {
    const classes = buttonVariants({ variant, size, className })

    if (asChild && React.isValidElement(children)) {
      const childProps = children.props as { className?: string }
      return React.cloneElement(
        children as React.ReactElement<{ className?: string }>,
        {
          className: cn(classes, childProps.className),
          ...props,
        }
      )
    }

    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={classes}
        {...props}
      >
        {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
        {children}
      </button>
    )
  }
)

Button.displayName = 'Button'
