import { AlertCircle, RefreshCw } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { cn } from '@/utils/cn'

export interface ErrorStateProps {
  title?: string
  message?: string
  onRetry?: () => void
  action?: React.ReactNode
  className?: string
  fullScreen?: boolean
}

export function ErrorState({
  title = 'Không thể tải nội dung',
  message = 'Đã xảy ra lỗi trong quá trình xử lý yêu cầu. Vui lòng thử lại sau.',
  onRetry,
  action,
  className,
  fullScreen = false,
}: ErrorStateProps) {
  return (
    <div
      role="alert"
      className={cn(
        'flex flex-col items-center justify-center p-6 text-center',
        fullScreen ? 'min-h-[70vh]' : 'min-h-[200px]',
        className
      )}
    >
      <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-100 text-red-600 dark:bg-red-950/50 dark:text-red-400 mb-3">
        <AlertCircle className="h-6 w-6" />
      </div>
      <h3 className="text-sm font-semibold text-zinc-900 dark:text-zinc-100">{title}</h3>
      <p className="mt-1 max-w-sm text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed">
        {message}
      </p>

      <div className="mt-4 flex items-center gap-2">
        {onRetry && (
          <Button size="sm" variant="outline" onClick={onRetry}>
            <RefreshCw className="mr-1.5 h-3.5 w-3.5" /> Thử lại
          </Button>
        )}
        {action}
      </div>
    </div>
  )
}
