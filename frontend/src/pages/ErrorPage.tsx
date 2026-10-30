import { Link } from 'react-router-dom'
import { ErrorState } from '@/components/common/ErrorState'
import { Button } from '@/components/ui/button'

export function ErrorPage() {
  return (
    <div className="py-12">
      <ErrorState
        title="Đã xảy ra lỗi hệ thống"
        message="Không thể xử lý yêu cầu lúc này. Vui lòng thử lại hoặc quay lại trang chủ."
        onRetry={() => window.location.reload()}
        action={
          <Button asChild size="sm">
            <Link to="/">Về trang chủ</Link>
          </Button>
        }
      />
    </div>
  )
}
