import React from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { LoadingState } from './LoadingState'
import { ROUTES } from '@/routes/route-paths'

interface ProtectedRouteProps {
  children: React.ReactNode
}

/**
 * Bảo vệ tuyến đường chỉ cho phép người dùng đã xác thực truy cập.
 * Tự động điều hướng về /login và bảo lưu URL dự định truy cập nếu chưa đăng nhập.
 */
export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) {
    return <LoadingState message="Đang xác thực phiên làm việc..." fullScreen />
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} state={{ from: location }} replace />
  }

  return <>{children}</>
}
