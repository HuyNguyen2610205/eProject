import * as React from 'react'

export interface ContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  children: React.ReactNode
  className?: string
}

export function Container({ children, className = '', ...props }: ContainerProps) {
  return (
    <div
      className={`mx-auto w-full max-w-5xl px-4 sm:px-6 lg:px-8 ${className}`}
      {...props}
    >
      {children}
    </div>
  )
}
