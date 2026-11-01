import { useState, useRef, useEffect } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import {
  User,
  LogOut,
  ChevronDown,
  Menu,
  X,
  HeartHandshake,
  Gift,
  Calendar,
  Info,
  Building2,
  HelpCircle,
  Home,
} from 'lucide-react'
import { Container } from '@/components/ui/container'
import { useAuth } from '@/hooks/useAuth'
import { cn } from '@/utils/cn'
import { ROUTES } from '@/routes/route-paths'

function getInitials(name?: string): string {
  if (!name) return 'U'
  const parts = name.trim().split(/\s+/)
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase()
  return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase()
}

export function AppHeader() {
  const location = useLocation()
  const navigate = useNavigate()
  const { user, isAuthenticated, logout } = useAuth()

  const [dropdownOpen, setDropdownOpen] = useState(false)
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const navItems = [
    { title: 'Home', href: ROUTES.HOME, icon: Home },
    { title: 'Causes', href: ROUTES.CAUSES, icon: HeartHandshake },
    { title: 'Programmes', href: '/#programmes', icon: Calendar },
    { title: 'About Us', href: '/#about', icon: Info },
    { title: 'Our Partners', href: '/#partners', icon: Building2 },
    { title: 'Help Centre', href: '/#contact', icon: HelpCircle },
  ]

  // Đóng dropdown khi click ra ngoài
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setDropdownOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  const handleLogout = async () => {
    setDropdownOpen(false)
    await logout()
    navigate(ROUTES.LOGIN, { replace: true })
  }

  const initials = getInitials(user?.fullName)

  return (
    <header className="border-b border-[#E2D9C8] bg-[#FAF7F2]/95 backdrop-blur dark:border-[#2D2720] dark:bg-[#141210]/95 sticky top-0 z-50 transition-colors">
      <Container className="flex h-20 items-center justify-between">
        
        {/* Bên trái: Logo GIVE-AID & Slogan */}
        <Link to={ROUTES.HOME} className="flex items-center gap-3 group">
          <div className="flex h-11 w-11 items-center justify-center border-2 border-[#C5A059] bg-[#1E3F2F] text-[#C5A059] shadow-sm group-hover:scale-105 transition-transform">
            <span className="text-xl">⚜</span>
          </div>
          <div className="flex flex-col">
            <span className="font-poppins text-xl font-black tracking-wider text-[#1F1D1A] dark:text-[#F5F2EB] flex items-center gap-1.5">
              GIVE-AID
              <span className="h-1.5 w-1.5 rounded-full bg-[#C5A059] inline-block" />
            </span>
            <span className="font-poppins text-[9px] tracking-widest uppercase text-[#8C6D37] dark:text-[#C5A059] hidden sm:inline -mt-0.5 font-semibold">
              ANNO MMXXVI • TOGETHER, WE MAKE A DIFFERENCE
            </span>
          </div>
        </Link>

        {/* Giữa: Desktop Navigation Menu */}
        <nav className="hidden lg:flex items-center gap-1.5 font-poppins text-xs font-semibold">
          {navItems.map((item) => {
            const isExactActive = location.pathname === item.href
            return (
              <a
                key={item.title}
                href={item.href}
                className={cn(
                  'px-3.5 py-2 tracking-wider uppercase transition-colors',
                  isExactActive
                    ? 'text-[#8C6D37] dark:text-[#C5A059] border-b-2 border-[#C5A059] font-bold'
                    : 'text-[#524B42] hover:text-[#8C6D37] hover:bg-[#F0EAE1]/60 dark:text-[#AAA295] dark:hover:text-[#F5F2EB] dark:hover:bg-[#1C1A17]'
                )}
              >
                {item.title}
              </a>
            )
          })}
        </nav>

        {/* Bên phải: Nút Donate Now & Tài khoản */}
        <div className="flex items-center gap-2.5 sm:gap-3">
          
          {/* Nút Donate Now nổi bật */}
          <Link
            to={ROUTES.DONATE}
            className="inline-flex items-center gap-2 px-4 sm:px-5 py-2.5 bg-[#1E3F2F] hover:bg-[#142E22] text-[#FAF7F2] font-poppins text-xs font-bold tracking-wider uppercase border border-[#C5A059] shadow-md hover:shadow-lg transition-all"
          >
            <Gift className="h-3.5 w-3.5 text-[#C5A059]" />
            <span>Donate Now</span>
          </Link>

          {/* User profile hoặc Login button */}
          {isAuthenticated && user ? (
            <div className="relative" ref={dropdownRef}>
              <button
                type="button"
                onClick={() => setDropdownOpen((prev) => !prev)}
                className="flex items-center gap-2 border border-[#E2D9C8] dark:border-[#2D2720] bg-white dark:bg-[#181614] p-1.5 text-xs text-[#221F1C] dark:text-[#EAE5D9] hover:border-[#C5A059] focus:outline-none"
                aria-expanded={dropdownOpen}
              >
                <div className="flex h-7 w-7 items-center justify-center bg-[#F4EFE6] dark:bg-[#201D1A] text-[#8C6D37] dark:text-[#C5A059] font-poppins font-bold text-xs border border-[#C5A059]/40">
                  {initials}
                </div>
                <span className="hidden sm:inline font-poppins text-xs font-semibold max-w-[120px] truncate">
                  {user.fullName || user.email}
                </span>
                <ChevronDown className="h-3.5 w-3.5 text-zinc-400" />
              </button>

              {/* User Dropdown */}
              {dropdownOpen && (
                <div className="absolute right-0 mt-2 w-60 border-2 border-double border-[#C5A059] bg-[#FAF7F2] dark:bg-[#181614] py-2 shadow-2xl z-50 animate-in fade-in zoom-in-95 duration-100 font-poppins">
                  <div className="px-4 py-2.5 border-b border-[#E2D9C8] dark:border-[#2D2720]">
                    <p className="font-bold text-xs text-[#1F1D1A] dark:text-[#F5F2EB] truncate">
                      {user.fullName || 'Tài khoản Thiện Nguyện'}
                    </p>
                    <p className="text-[11px] text-[#6B6256] dark:text-[#A69D8F] font-inter truncate">{user.email}</p>
                  </div>

                  <Link
                    to={ROUTES.PROFILE}
                    onClick={() => setDropdownOpen(false)}
                    className="flex items-center gap-2.5 px-4 py-2.5 text-xs text-[#221F1C] dark:text-[#EAE5D9] hover:bg-[#F0EAE1] dark:hover:bg-[#201D1A]"
                  >
                    <User className="h-3.5 w-3.5 text-[#C5A059]" />
                    <span>Hồ sơ & Mật khẩu</span>
                  </Link>

                  <Link
                    to={ROUTES.CAUSES}
                    onClick={() => setDropdownOpen(false)}
                    className="flex items-center gap-2.5 px-4 py-2.5 text-xs text-[#221F1C] dark:text-[#EAE5D9] hover:bg-[#F0EAE1] dark:hover:bg-[#201D1A]"
                  >
                    <HeartHandshake className="h-3.5 w-3.5 text-[#C5A059]" />
                    <span>Mục tiêu quyên góp</span>
                  </Link>

                  <div className="border-t border-[#E2D9C8] dark:border-[#2D2720] mt-1 pt-1">
                    <button
                      type="button"
                      onClick={handleLogout}
                      className="flex w-full items-center gap-2.5 px-4 py-2 text-xs text-red-700 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-950/20"
                    >
                      <LogOut className="h-3.5 w-3.5" />
                      <span>Đăng xuất</span>
                    </button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="hidden sm:flex items-center gap-2 font-poppins">
              <Link
                to={ROUTES.LOGIN}
                className="px-3.5 py-2 text-xs font-semibold tracking-wider text-[#524B42] hover:text-[#8C6D37] dark:text-[#AAA295] dark:hover:text-[#F5F2EB] transition-colors"
              >
                Login
              </Link>
              <Link
                to={ROUTES.REGISTER}
                className="px-3.5 py-2 text-xs font-semibold tracking-wider border border-[#C5A059] text-[#8C6D37] dark:text-[#C5A059] hover:bg-[#C5A059] hover:text-[#1F1D1A] transition-colors"
              >
                Register
              </Link>
            </div>
          )}

          {/* Hamburger Menu Toggle (Mobile) */}
          <button
            type="button"
            onClick={() => setMobileMenuOpen((prev) => !prev)}
            className="lg:hidden p-2 text-[#524B42] dark:text-[#AAA295] hover:text-[#1F1D1A] focus:outline-none"
            aria-label="Toggle menu"
          >
            {mobileMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
          </button>
        </div>
      </Container>

      {/* Mobile Drawer Menu */}
      {mobileMenuOpen && (
        <div className="lg:hidden border-b border-[#E2D9C8] bg-[#FAF7F2] px-4 py-4 dark:border-[#2D2720] dark:bg-[#141210] animate-in slide-in-from-top-2 duration-150 shadow-xl font-poppins">
          <nav className="flex flex-col space-y-1">
            {navItems.map((item) => (
              <a
                key={item.title}
                href={item.href}
                onClick={() => setMobileMenuOpen(false)}
                className="flex items-center gap-3 px-3 py-2.5 text-xs font-semibold uppercase tracking-wider text-[#221F1C] hover:bg-[#F0EAE1] hover:text-[#8C6D37] dark:text-[#EAE5D9] dark:hover:bg-[#1E1B17]"
              >
                <item.icon className="h-4 w-4 text-[#C5A059]" />
                <span>{item.title}</span>
              </a>
            ))}

            {!isAuthenticated && (
              <div className="pt-3 border-t border-[#E2D9C8] dark:border-[#2D2720] flex flex-col gap-2">
                <Link
                  to={ROUTES.LOGIN}
                  onClick={() => setMobileMenuOpen(false)}
                  className="w-full text-center py-2.5 text-xs font-semibold tracking-wider rounded-sm bg-[#F0EAE1] dark:bg-[#1E1B17] text-[#221F1C] dark:text-[#EAE5D9]"
                >
                  Login
                </Link>
                <Link
                  to={ROUTES.REGISTER}
                  onClick={() => setMobileMenuOpen(false)}
                  className="w-full text-center py-2.5 text-xs font-semibold tracking-wider border border-[#C5A059] text-[#8C6D37] dark:text-[#C5A059]"
                >
                  Register
                </Link>
              </div>
            )}
          </nav>
        </div>
      )}
    </header>
  )
}
