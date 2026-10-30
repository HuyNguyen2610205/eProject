import React from 'react'
import { BrowserRouter } from 'react-router-dom'
import { ErrorBoundary } from '@/components/common/ErrorBoundary'
import { AuthProvider } from '@/context/AuthContext'
import { Toaster } from '@/components/ui/sonner'

interface AppProvidersProps {
  children: React.ReactNode
}

export function AppProviders({ children }: AppProvidersProps) {
  return (
    <ErrorBoundary>
      <BrowserRouter>
        <AuthProvider>
          {children}
          <Toaster />
        </AuthProvider>
      </BrowserRouter>
    </ErrorBoundary>
  )
}
