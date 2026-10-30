import * as React from 'react'
import { FolderOpen } from 'lucide-react'
import { cn } from '@/utils/cn'

export interface EmptyStateProps {
  icon?: React.ElementType
  title?: string
  description?: string
  action?: React.ReactNode
  className?: string
}

export function EmptyState({
  icon: Icon = FolderOpen,
  title = 'Chưa có dữ liệu',
  description = 'Hiện tại khu vực này chưa có mục nào để hiển thị.',
  action,
  className,
}: EmptyStateProps) {
  return (
    <div
      className={cn(
        'flex min-h-[220px] flex-col items-center justify-center rounded-xl border border-dashed border-zinc-200 dark:border-zinc-800 p-8 text-center bg-zinc-50/50 dark:bg-zinc-900/50',
        className
      )}
    >
      <div className="flex h-12 w-12 items-center justify-center rounded-full bg-zinc-100 text-zinc-500 dark:bg-zinc-800 dark:text-zinc-400 mb-3">
        <Icon className="h-6 w-6" />
      </div>
      <h3 className="text-sm font-semibold text-zinc-900 dark:text-zinc-100">{title}</h3>
      {description && (
        <p className="mt-1 max-w-sm text-xs text-zinc-500 dark:text-zinc-400 leading-relaxed">
          {description}
        </p>
      )}
      {action && <div className="mt-4">{action}</div>}
    </div>
  )
}
