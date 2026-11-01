import React from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { LoadingState } from './LoadingState'
import { ROUTES } from '@/routes/route-paths'

interface GuestRouteProps {
  children: React.ReactNode
}

/**
 * Tuyến đường dành riêng cho khách chưa đăng nhập (Login, Register).
 * Tự động điều hướng về trang chủ hoặc trang trước đó nếu người dùng đã đăng nhập.
 */
export const GuestRoute: React.FC<GuestRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) {
    return <LoadingState message="Đang tải phiên..." fullScreen />
  }

  if (isAuthenticated) {
    const from = (location.state as { from?: { pathname?: string } })?.from?.pathname || ROUTES.HOME
    return <Navigate to={from} replace />
  }

  return <>{children}</>
}
