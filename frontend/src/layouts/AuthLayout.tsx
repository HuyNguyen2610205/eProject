import { Link, Outlet } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-zinc-50 px-4 py-12 text-zinc-900 dark:bg-zinc-950 dark:text-zinc-50">
      {/* Brand Header */}
      <div className="mb-6 text-center">
        <Link to="/" className="text-xl font-bold tracking-tight hover:opacity-80 transition-opacity">
          eProject
        </Link>
        <p className="mt-1 text-xs text-zinc-500 dark:text-zinc-400">
          Cổng định danh & xác thực người dùng
        </p>
      </div>

      {/* Main Centered Form Card */}
      <div className="w-full max-w-md">
        <Outlet />
      </div>

      {/* Return Link */}
      <div className="mt-8 text-center text-xs text-zinc-500">
        <Link to="/" className="inline-flex items-center gap-1.5 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors">
          <ArrowLeft className="h-3.5 w-3.5" /> Quay lại Trang chủ
        </Link>
      </div>
    </div>
  )
}
