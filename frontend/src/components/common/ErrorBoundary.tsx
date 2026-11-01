import { Component, ErrorInfo, ReactNode } from 'react'
import { AlertCircle, RefreshCw } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
  error: Error | null
}

export class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: null,
  }

  public static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error }
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('ErrorBoundary bắt được lỗi:', error, errorInfo)
  }

  private handleReset = () => {
    this.setState({ hasError: false, error: null })
    window.location.href = '/'
  }

  public render() {
    if (this.state.hasError) {
      return (
        <div className="flex min-h-screen flex-col items-center justify-center p-4 text-center bg-zinc-50 dark:bg-zinc-950">
          <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-red-100 text-red-600 dark:bg-red-950/50 dark:text-red-400 mb-4">
            <AlertCircle className="h-7 w-7" />
          </div>
          <h1 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">
            Đã xảy ra sự cố ngoài ý muốn
          </h1>
          <p className="mt-2 text-xs text-zinc-500 dark:text-zinc-400 max-w-md">
            Hệ thống gặp sự cố trong quá trình kết xuất giao diện. Bạn có thể tải lại trang hoặc quay lại trang chủ.
          </p>
          <div className="mt-6 flex gap-3">
            <Button onClick={() => window.location.reload()} variant="outline" size="sm">
              <RefreshCw className="mr-2 h-3.5 w-3.5" /> Tải lại trang
            </Button>
            <Button onClick={this.handleReset} size="sm">
              Về trang chủ
            </Button>
          </div>
        </div>
      )
    }

    return this.props.children
  }
}
