import * as React from 'react'
import { AlertCircle, CheckCircle2, Info } from 'lucide-react'
import { cn } from '@/utils/cn'

export interface AlertProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'destructive' | 'success'
}

export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(
  ({ className, variant = 'default', children, ...props }, ref) => {
    const variants = {
      default: 'border-zinc-200 bg-zinc-50 text-zinc-900 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-100',
      destructive: 'border-red-200 bg-red-50 text-red-900 dark:border-red-900 dark:bg-red-950/40 dark:text-red-200 [&>svg]:text-red-600',
      success: 'border-emerald-200 bg-emerald-50 text-emerald-900 dark:border-emerald-900 dark:bg-emerald-950/40 dark:text-emerald-200 [&>svg]:text-emerald-600',
    }

    const Icon = variant === 'destructive' ? AlertCircle : variant === 'success' ? CheckCircle2 : Info

    return (
      <div
        ref={ref}
        role="alert"
        className={cn(
          'relative w-full rounded-lg border px-4 py-3 text-sm [&>svg+div]:translate-y-[-2px] [&>svg]:absolute [&>svg]:left-4 [&>svg]:top-3.5 [&>svg~*]:pl-7',
          variants[variant],
          className
        )}
        {...props}
      >
        <Icon className="h-4 w-4" />
        {children}
      </div>
    )
  }
)

Alert.displayName = 'Alert'

export const AlertTitle = React.forwardRef<HTMLParagraphElement, React.HTMLAttributes<HTMLHeadingElement>>(
  ({ className, ...props }, ref) => (
    <h5 ref={ref} className={cn('mb-1 font-semibold leading-none tracking-tight text-sm', className)} {...props} />
  )
)

AlertTitle.displayName = 'AlertTitle'

export const AlertDescription = React.forwardRef<HTMLParagraphElement, React.HTMLAttributes<HTMLParagraphElement>>(
  ({ className, ...props }, ref) => (
    <div ref={ref} className={cn('text-xs [&_p]:leading-relaxed text-zinc-600 dark:text-zinc-400', className)} {...props} />
  )
)

AlertDescription.displayName = 'AlertDescription'
