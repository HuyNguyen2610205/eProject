import { lazy, Suspense } from 'react'
import { Routes, Route } from 'react-router-dom'
import { ROUTES } from './route-paths'
import { PublicLayout, AuthLayout, AppLayout } from '@/layouts'
import { ProtectedRoute, GuestRoute, LoadingState } from '@/components/common'

// Lazy loading cho tất cả các trang
const HomePage = lazy(() => import('@/pages/HomePage').then((m) => ({ default: m.HomePage })))
const CausesPage = lazy(() => import('@/pages/CausesPage').then((m) => ({ default: m.CausesPage })))
const DonatePage = lazy(() => import('@/pages/DonatePage').then((m) => ({ default: m.DonatePage })))
const LoginPage = lazy(() => import('@/pages/LoginPage').then((m) => ({ default: m.LoginPage })))
const RegisterPage = lazy(() => import('@/pages/RegisterPage').then((m) => ({ default: m.RegisterPage })))
const ForgotPasswordPage = lazy(() => import('@/pages/ForgotPasswordPage').then((m) => ({ default: m.ForgotPasswordPage })))
const ResetPasswordPage = lazy(() => import('@/pages/ResetPasswordPage').then((m) => ({ default: m.ResetPasswordPage })))
const VerifyEmailPage = lazy(() => import('@/pages/VerifyEmailPage').then((m) => ({ default: m.VerifyEmailPage })))
const ProfilePage = lazy(() => import('@/pages/ProfilePage').then((m) => ({ default: m.ProfilePage })))
const NotFoundPage = lazy(() => import('@/pages/NotFoundPage').then((m) => ({ default: m.NotFoundPage })))
const ErrorPage = lazy(() => import('@/pages/ErrorPage').then((m) => ({ default: m.ErrorPage })))

export function AppRoutes() {
  return (
    <Suspense fallback={<LoadingState fullScreen message="Đang tải giao diện..." />}>
      <Routes>
        {/* 1. Tuyến đường công khai (PublicLayout) */}
        <Route element={<PublicLayout />}>
          <Route path={ROUTES.HOME} element={<HomePage />} />
          <Route path={ROUTES.CAUSES} element={<CausesPage />} />
          <Route path={ROUTES.DONATE} element={<DonatePage />} />
          <Route path={ROUTES.ERROR} element={<ErrorPage />} />
          <Route path={ROUTES.NOT_FOUND} element={<NotFoundPage />} />
        </Route>

        {/* 2. Tuyến đường xác thực (AuthLayout) */}
        <Route element={<AuthLayout />}>
          <Route
            path={ROUTES.LOGIN}
            element={
              <GuestRoute>
                <LoginPage />
              </GuestRoute>
            }
          />
          <Route
            path={ROUTES.REGISTER}
            element={
              <GuestRoute>
                <RegisterPage />
              </GuestRoute>
            }
          />
          <Route path={ROUTES.FORGOT_PASSWORD} element={<ForgotPasswordPage />} />
          <Route path={ROUTES.RESET_PASSWORD} element={<ResetPasswordPage />} />
          <Route path={ROUTES.VERIFY_EMAIL} element={<VerifyEmailPage />} />
        </Route>

        {/* 3. Tuyến đường sau xác thực (AppLayout) */}
        <Route element={<AppLayout />}>
          <Route
            path={ROUTES.PROFILE}
            element={
              <ProtectedRoute>
                <ProfilePage />
              </ProtectedRoute>
            }
          />
        </Route>
      </Routes>
    </Suspense>
  )
}
