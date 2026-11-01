import { Link } from 'react-router-dom'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'

export function VerifyEmailPage() {
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Xác thực Email</CardTitle>
        <CardDescription>Xác minh địa chỉ hộp thư để hoàn tất kích hoạt tài khoản</CardDescription>
      </CardHeader>

      <CardContent className="space-y-4">
        <Alert variant="default">
          <AlertTitle>Kích hoạt tài khoản</AlertTitle>
          <AlertDescription>
            Tuyến đường xác thực sẵn sàng nhận token từ liên kết email khi nghiệp vụ chính thức được triển khai.
          </AlertDescription>
        </Alert>
      </CardContent>

      <CardFooter>
        <Button asChild className="w-full">
          <Link to="/login">Đến trang Đăng nhập</Link>
        </Button>
      </CardFooter>
    </Card>
  )
}
