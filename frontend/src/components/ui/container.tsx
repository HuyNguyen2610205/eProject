import * as React from 'react'
import { cn } from '@/utils/cn'

export interface ContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full' | 'default'
  as?: React.ElementType
}

const sizeClasses = {
  sm: 'max-w-3xl',
  md: 'max-w-5xl',
  lg: 'max-w-6xl',
  default: 'max-w-7xl',
  xl: 'max-w-screen-2xl',
  full: 'max-w-full',
}

export function Container({
  className,
  size = 'default',
  as: Component = 'div',
  children,
  ...props
}: ContainerProps) {
  return (
    <Component
      className={cn(
        'mx-auto w-full px-4 sm:px-6 lg:px-8',
        sizeClasses[size],
        className
      )}
      {...props}
    >
      {children}
    </Component>
  )
}
