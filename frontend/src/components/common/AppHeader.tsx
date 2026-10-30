import { useState, useRef, useEffect } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { User, Home, Bell, LogOut, ChevronDown } from 'lucide-react'
import { Container } from '@/components/ui/container'
import { Badge } from '@/components/ui/badge'
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
  const dropdownRef = useRef<HTMLDivElement>(null)

  const navItems = [
    { title: 'Trang chủ', href: ROUTES.HOME, icon: Home },
    ...(isAuthenticated ? [{ title: 'Hồ sơ', href: ROUTES.PROFILE, icon: User }] : []),
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
    <header className="border-b border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-900 sticky top-0 z-40">
      <Container className="flex h-14 items-center justify-between">
        <div className="flex items-center gap-6">
          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-1 text-xs font-medium">
            {navItems.map((item) => {
              const Icon = item.icon
              const isActive = location.pathname === item.href
              return (
                <Link
                  key={item.href}
                  to={item.href}
                  className={cn(
                    'flex items-center gap-1.5 rounded-md px-3 py-1.5 transition-colors',
                    isActive
                      ? 'bg-zinc-100 text-zinc-900 dark:bg-zinc-800 dark:text-zinc-50 font-semibold'
                      : 'text-zinc-600 hover:bg-zinc-50 dark:text-zinc-400 dark:hover:bg-zinc-800/50'
                  )}
                >
                  <Icon className="h-3.5 w-3.5" />
                  <span>{item.title}</span>
                </Link>
              )
            })}
          </nav>
        </div>

        <div className="flex items-center gap-3">
          {isAuthenticated ? (
            <>
              <button
                type="button"
                className="relative p-1.5 text-zinc-500 hover:text-zinc-900 dark:text-zinc-400 dark:hover:text-zinc-100 cursor-pointer"
                title="Thông báo"
              >
                <Bell className="h-4 w-4" />
                <span className="absolute top-1 right-1 h-1.5 w-1.5 rounded-full bg-zinc-900 dark:bg-zinc-100" />
              </button>

              {/* User Dropdown */}
              <div className="relative" ref={dropdownRef}>
                <button
                  type="button"
                  onClick={() => setDropdownOpen((prev) => !prev)}
                  className="flex items-center gap-2 rounded-full p-0.5 hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors cursor-pointer"
                  aria-expanded={dropdownOpen}
                  aria-haspopup="true"
                  data-testid="user-menu-button"
                >
                  <div className="h-8 w-8 rounded-full bg-zinc-900 text-zinc-50 dark:bg-zinc-100 dark:text-zinc-900 flex items-center justify-center text-xs font-semibold shadow-xs">
                    {initials}
                  </div>
                  <div className="hidden sm:flex flex-col text-left text-xs leading-tight pr-1">
                    <span
                      className="font-medium text-zinc-900 dark:text-zinc-100 max-w-[120px] truncate"
                      data-testid="header-user-name"
                    >
                      {user?.fullName || 'Tài khoản'}
                    </span>
                    <span className="text-[10px] text-zinc-400 max-w-[120px] truncate">
                      {user?.email || ''}
                    </span>
                  </div>
                  <ChevronDown className="h-3.5 w-3.5 text-zinc-400 hidden sm:block" />
                </button>

                {dropdownOpen && (
                  <div className="absolute right-0 mt-2 w-56 rounded-md border border-zinc-200 bg-white py-1.5 shadow-lg dark:border-zinc-800 dark:bg-zinc-900 z-50 animate-in fade-in zoom-in-95 duration-100">
                    <div className="px-3 py-2 border-b border-zinc-100 dark:border-zinc-800">
                      <p className="text-xs font-semibold text-zinc-900 dark:text-zinc-100 truncate">
                        {user?.fullName}
                      </p>
                      <p className="text-[11px] text-zinc-500 truncate">{user?.email}</p>
                      <div className="mt-1 flex flex-wrap gap-1">
                        {user?.roles?.map((r) => (
                          <Badge
                            key={r}
                            variant={r === 'Admin' ? 'default' : 'secondary'}
                            className="text-[9px] px-1 py-0"
                          >
                            {r}
                          </Badge>
                        ))}
                      </div>
                    </div>

                    <Link
                      to={ROUTES.PROFILE}
                      onClick={() => setDropdownOpen(false)}
                      className="flex items-center gap-2 px-3 py-2 text-xs text-zinc-700 hover:bg-zinc-100 dark:text-zinc-300 dark:hover:bg-zinc-800"
                    >
                      <User className="h-3.5 w-3.5" />
                      <span>Hồ sơ tài khoản</span>
                    </Link>

                    <Link
                      to={ROUTES.HOME}
                      onClick={() => setDropdownOpen(false)}
                      className="flex items-center gap-2 px-3 py-2 text-xs text-zinc-700 hover:bg-zinc-100 dark:text-zinc-300 dark:hover:bg-zinc-800"
                    >
                      <Home className="h-3.5 w-3.5" />
                      <span>Trang chủ</span>
                    </Link>

                    <div className="border-t border-zinc-100 dark:border-zinc-800 my-1" />

                    <button
                      type="button"
                      onClick={handleLogout}
                      data-testid="logout-button"
                      className="flex w-full items-center gap-2 px-3 py-2 text-xs text-red-600 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-950/30 cursor-pointer"
                    >
                      <LogOut className="h-3.5 w-3.5" />
                      <span>Đăng xuất</span>
                    </button>
                  </div>
                )}
              </div>
            </>
          ) : (
            <div className="flex items-center gap-3 text-xs font-medium">
              <Link
                to={ROUTES.REGISTER}
                className="text-zinc-600 hover:text-zinc-900 dark:text-zinc-400 dark:hover:text-zinc-100 transition-colors"
              >
                Đăng ký
              </Link>
              <Link
                to={ROUTES.LOGIN}
                className="rounded-md bg-zinc-900 px-3 py-1.5 text-zinc-50 hover:bg-zinc-800 dark:bg-zinc-50 dark:text-zinc-900 dark:hover:bg-zinc-200 transition-colors"
              >
                Đăng nhập
              </Link>
            </div>
          )}
        </div>
      </Container>
    </header>
  )
}
