import * as React from 'react'
import { cn } from '@/utils/cn'

export interface PageHeaderProps extends React.HTMLAttributes<HTMLDivElement> {
  title: string
  description?: string
  breadcrumbs?: React.ReactNode
  actions?: React.ReactNode
}

export function PageHeader({
  title,
  description,
  breadcrumbs,
  actions,
  className,
  ...props
}: PageHeaderProps) {
  return (
    <div
      className={cn(
        'flex flex-col gap-4 pb-6 md:flex-row md:items-center md:justify-between',
        className
      )}
      {...props}
    >
      <div className="space-y-1">
        {breadcrumbs && <div className="text-xs text-zinc-500 mb-1">{breadcrumbs}</div>}
        <h1 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 sm:text-3xl">
          {title}
        </h1>
        {description && (
          <p className="text-sm text-zinc-600 dark:text-zinc-400 leading-relaxed">
            {description}
          </p>
        )}
      </div>

      {actions && (
        <div className="flex flex-wrap items-center gap-2.5 pt-2 md:pt-0 shrink-0">
          {actions}
        </div>
      )}
    </div>
  )
}
