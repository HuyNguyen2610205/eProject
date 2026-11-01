import { Outlet } from 'react-router-dom'
import { AppHeader } from '@/components/common/AppHeader'

export function PublicLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-[#FAF7F2] text-[#221F1C] dark:bg-[#121110] dark:text-[#EAE5D9] transition-colors duration-300 font-inter">
      {/* Header đồng bộ phong cách cổ điển di sản */}
      <AppHeader />

      {/* Main Content Area */}
      <main className="flex-1 w-full flex flex-col">
        <Outlet />
      </main>
    </div>
  )
}
