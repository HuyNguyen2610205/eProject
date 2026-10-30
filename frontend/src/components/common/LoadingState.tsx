import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/utils/cn'

export interface LoadingStateProps {
  message?: string
  fullScreen?: boolean
  className?: string
  size?: 'sm' | 'md' | 'lg' | 'xl'
}

export function LoadingState({
  message = 'Đang tải...',
  fullScreen = false,
  className,
  size = 'md',
}: LoadingStateProps) {
  return (
    <div
      role="status"
      className={cn(
        'flex flex-col items-center justify-center p-6 text-center',
        fullScreen ? 'min-h-[70vh]' : 'min-h-[160px]',
        className
      )}
    >
      <Spinner size={size} className="mb-3" />
      {message && (
        <p className="text-xs font-medium text-zinc-500 dark:text-zinc-400 animate-pulse">
          {message}
        </p>
      )}
    </div>
  )
}
