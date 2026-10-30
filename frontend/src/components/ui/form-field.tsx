import * as React from 'react'
import { Label } from './label'
import { cn } from '@/utils/cn'

export interface FormFieldProps {
  id?: string
  label?: string
  error?: string
  helperText?: string
  required?: boolean
  className?: string
  children: React.ReactNode
}

export function FormField({
  id,
  label,
  error,
  helperText,
  required = false,
  className,
  children,
}: FormFieldProps) {
  const errorId = id && error ? `${id}-error` : undefined
  const helperId = id && helperText ? `${id}-helper` : undefined
  const describedBy = [errorId, helperId].filter(Boolean).join(' ') || undefined

  return (
    <div className={cn('space-y-1.5', className)}>
      {label && (
        <div className="flex items-center justify-between">
          <Label htmlFor={id} className="text-sm font-medium">
            {label}
            {required && <span className="text-red-500 ml-1">*</span>}
          </Label>
        </div>
      )}

      <div
        className="relative"
        data-invalid={!!error}
        aria-describedby={describedBy}
      >
        {children}
      </div>

      {helperText && !error && (
        <p id={helperId} className="text-xs text-zinc-500 dark:text-zinc-400">
          {helperText}
        </p>
      )}

      {error && (
        <p id={errorId} role="alert" className="text-xs text-red-600 dark:text-red-400 font-medium">
          {error}
        </p>
      )}
    </div>
  )
}
