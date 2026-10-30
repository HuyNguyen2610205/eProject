import { Outlet } from 'react-router-dom'
import { AppHeader } from '@/components/common/AppHeader'

export function PublicLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-zinc-50 text-zinc-900 dark:bg-zinc-950 dark:text-zinc-50">
      {/* Header đồng bộ chuẩn với toàn bộ hệ thống */}
      <AppHeader />

      {/* Main Content Area */}
      <main className="flex-1 w-full flex flex-col">
        <Outlet />
      </main>
    </div>
  )
}
