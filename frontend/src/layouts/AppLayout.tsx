import { Link, Outlet, useLocation } from 'react-router-dom'
import { User, Home } from 'lucide-react'
import { Container } from '@/components/ui/container'
import { AppHeader } from '@/components/common/AppHeader'
import { cn } from '@/utils/cn'
import { ROUTES } from '@/routes/route-paths'

export function AppLayout() {
  const location = useLocation()

  const navItems = [
    { title: 'Trang chủ', href: ROUTES.HOME, icon: Home },
    { title: 'Hồ sơ', href: ROUTES.PROFILE, icon: User },
  ]

  return (
    <div className="flex min-h-screen flex-col bg-[#FAF7F2] text-[#221F1C] dark:bg-[#121110] dark:text-[#EAE5D9] transition-colors duration-300 font-inter">
      {/* Top Header đồng bộ dùng chung */}
      <AppHeader />

      {/* Main Content Area */}
      <main className="flex-1 py-8 pb-16 md:pb-8">
        <Container>
          <Outlet />
        </Container>
      </main>

      {/* Mobile Bottom Navigation */}
      <nav className="md:hidden fixed bottom-0 left-0 z-40 w-full border-t border-[#E2D9C8] bg-[#FAF7F2]/95 backdrop-blur dark:border-[#2D2720] dark:bg-[#141210]/95 py-1 px-2 shadow-sm font-poppins">
        <div className="grid grid-cols-2 gap-1">
          {navItems.map((item) => {
            const Icon = item.icon
            const isActive = location.pathname === item.href
            return (
              <Link
                key={item.href}
                to={item.href}
                className={cn(
                  'flex flex-col items-center justify-center py-1.5 text-[10px] font-semibold transition-colors rounded',
                  isActive
                    ? 'text-[#8C6D37] dark:text-[#C5A059] font-bold'
                    : 'text-[#6B6256] dark:text-[#A69D8F]'
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
      <footer className="hidden md:block border-t border-[#E2D9C8] bg-[#F4EFE6] py-4 text-center text-xs font-poppins text-[#8C6D37] dark:border-[#2D2720] dark:bg-[#181614] dark:text-[#C5A059]">
        <Container>
          <span>⚜ &copy; MMXXVI GIVE-AID NGO • NỀN TẢNG BẢO TRỢ & KẾT NỐI DI SẢN ⚜</span>
        </Container>
      </footer>
    </div>
  )
}
