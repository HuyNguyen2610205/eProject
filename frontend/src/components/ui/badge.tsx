import * as React from 'react'
import { cn } from '@/utils/cn'

export interface BadgeProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'secondary' | 'destructive' | 'outline' | 'success'
}

export function Badge({ className, variant = 'default', ...props }: BadgeProps) {
  const variants = {
    default: 'border-transparent bg-zinc-900 text-zinc-50 shadow hover:bg-zinc-800 dark:bg-zinc-50 dark:text-zinc-900',
    secondary: 'border-transparent bg-zinc-100 text-zinc-900 hover:bg-zinc-200 dark:bg-zinc-800 dark:text-zinc-50',
    destructive: 'border-transparent bg-red-600 text-white shadow hover:bg-red-700',
    outline: 'border-zinc-300 text-zinc-900 dark:border-zinc-700 dark:text-zinc-50',
    success: 'border-transparent bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-300',
  }

  return (
    <div
      className={cn(
        'inline-flex items-center rounded-md border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-zinc-400 focus:ring-offset-2',
        variants[variant],
        className
      )}
      {...props}
    />
  )
}
