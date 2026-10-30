import { Link, Outlet, useLocation } from 'react-router-dom'
import { User, Home } from 'lucide-react'
import { Container } from '@/components/ui/container'
import { AppHeader } from '@/components/common/AppHeader'
import { cn } from '@/utils/cn'
import { ROUTES } from '@/routes/route-paths'

const currentYear = new Date().getFullYear()

export function AppLayout() {
  const location = useLocation()

  const navItems = [
    { title: 'Trang chủ', href: ROUTES.HOME, icon: Home },
    { title: 'Hồ sơ', href: ROUTES.PROFILE, icon: User },
  ]

  return (
    <div className="flex min-h-screen flex-col bg-zinc-50 text-zinc-900 dark:bg-zinc-950 dark:text-zinc-50">
      {/* Top Header đồng bộ dùng chung */}
      <AppHeader />

      {/* Main Content Area */}
      <main className="flex-1 py-8 pb-16 md:pb-8">
        <Container>
          <Outlet />
        </Container>
      </main>

      {/* Mobile Bottom Navigation */}
      <nav className="md:hidden fixed bottom-0 left-0 z-40 w-full border-t border-zinc-200 bg-white/95 backdrop-blur dark:border-zinc-800 dark:bg-zinc-900/95 py-1 px-2 shadow-sm">
        <div className="grid grid-cols-2 gap-1">
          {navItems.map((item) => {
            const Icon = item.icon
            const isActive = location.pathname === item.href
            return (
              <Link
                key={item.href}
                to={item.href}
                className={cn(
                  'flex flex-col items-center justify-center py-1.5 text-[10px] font-medium transition-colors rounded',
                  isActive
                    ? 'text-zinc-900 dark:text-zinc-50 font-bold'
                    : 'text-zinc-500 dark:text-zinc-400'
                )}
              >
                <Icon className="h-4 w-4 mb-0.5" />
                <span>{item.title}</span>
              </Link>
            )
          })}
        </div>
      </nav>

      {/* Footer */}
      <footer className="hidden md:block border-t border-zinc-200 bg-white py-4 text-center text-xs text-zinc-500 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-400">
        <Container>
          <span>&copy; {currentYear} eProject Client Portal • Đã kết nối hệ thống Identity</span>
        </Container>
      </footer>
    </div>
  )
}
