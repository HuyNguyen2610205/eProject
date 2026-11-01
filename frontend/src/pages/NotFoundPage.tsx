import { Link } from 'react-router-dom'
import { Button } from '@/components/Button'

export function NotFoundPage() {
  return (
    <div className="mx-auto max-w-md text-center">
      <p className="text-4xl font-bold text-zinc-400">404</p>
      <h1 className="mt-2 text-xl font-semibold text-zinc-900 dark:text-zinc-100">
        Không tìm thấy trang
      </h1>
      <p className="mt-2 text-xs text-zinc-500 dark:text-zinc-400">
        Đường dẫn bạn yêu cầu không tồn tại trong hệ thống Client Web.
      </p>
      <div className="mt-6">
        <Link to="/">
          <Button variant="secondary">Quay lại trang chính</Button>
        </Link>
      </div>
    </div>
  )
}
