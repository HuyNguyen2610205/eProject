import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios'
import { tokenStorage } from '@/lib/tokenStorage'
import { ApiResponse } from '@/types/api'
import { AuthResponseData, RefreshTokenRequest } from '@/types/auth'

export const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

let isRefreshing = false
let failedQueue: Array<{
  resolve: (token: string) => void
  reject: (error: unknown) => void
}> = []

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error)
    } else if (token) {
      prom.resolve(token)
    }
  })

  failedQueue = []
}

// Event bus cho phiên làm việc hết hạn
type SessionExpiredHandler = () => void
let onSessionExpiredCallback: SessionExpiredHandler | null = null

export const setOnSessionExpiredCallback = (callback: SessionExpiredHandler) => {
  onSessionExpiredCallback = callback
}

// Request Interceptor: đính kèm access token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = tokenStorage.getAccessToken()
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Response Interceptor: tự động refresh token khi gặp 401
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

    // Nếu không có request gốc hoặc lỗi không phải 401
    if (!originalRequest || !error.response || error.response.status !== 401) {
      return Promise.reject(error)
    }

    // Không lặp vô tận nếu endpoint gây lỗi chính là login hoặc refresh-token
    const url = originalRequest.url || ''
    if (url.includes('/auth/login') || url.includes('/auth/refresh-token') || url.includes('/auth/register')) {
      return Promise.reject(error)
    }

    if (originalRequest._retry) {
      return Promise.reject(error)
    }

    const refreshToken = tokenStorage.getRefreshToken()
    const accessToken = tokenStorage.getAccessToken() || ''

    if (!refreshToken) {
      tokenStorage.clearTokens()
      onSessionExpiredCallback?.()
      return Promise.reject(error)
    }

    if (isRefreshing) {
      return new Promise<string>((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      })
        .then((newToken) => {
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${newToken}`
          }
          return apiClient(originalRequest)
        })
        .catch((err) => Promise.reject(err))
    }

    originalRequest._retry = true
    isRefreshing = true

    try {
      // Backend yêu cầu cả accessToken và refreshToken trong body
      const payload: RefreshTokenRequest = {
        accessToken,
        refreshToken,
      }

      const response = await axios.post<ApiResponse<AuthResponseData>>(
        '/api/auth/refresh-token',
        payload,
        { headers: { 'Content-Type': 'application/json' } }
      )

      if (response.data.success && response.data.data) {
        const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data.data
        tokenStorage.setTokens(newAccessToken, newRefreshToken)

        processQueue(null, newAccessToken)

        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`
        }

        return apiClient(originalRequest)
      } else {
        throw new Error(response.data.message || 'Làm mới token thất bại')
      }
    } catch (refreshError) {
      processQueue(refreshError, null)
      tokenStorage.clearTokens()
      onSessionExpiredCallback?.()
      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  }
)
