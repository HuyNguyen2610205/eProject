import { Outlet } from 'react-router-dom'
import { Container } from '@/components/Container'

export function RootLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-zinc-50 text-zinc-900 dark:bg-zinc-950 dark:text-zinc-50">
      {/* Header tối giản */}
      <header className="border-b border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-900">
        <Container className="flex h-14 items-center justify-between">
          <span className="text-sm font-semibold tracking-tight">eProject</span>
          <span className="text-xs text-zinc-500 dark:text-zinc-400">Client Web Skeleton</span>
        </Container>
      </header>

      {/* Vùng nội dung chính */}
      <main className="flex flex-1 items-center justify-center py-12">
        <Container>
          <Outlet />
        </Container>
      </main>

      {/* Footer tối giản */}
      <footer className="border-t border-zinc-200 bg-white py-4 text-center text-xs text-zinc-500 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-400">
        <Container>
          <span>eProject • Client Web Skeleton (Chưa có đề tài chính thức)</span>
        </Container>
      </footer>
    </div>
  )
}
