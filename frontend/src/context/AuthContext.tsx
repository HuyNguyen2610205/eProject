import React, { useCallback, useEffect, useState } from 'react'
import { authService } from '@/services/authService'
import { setOnSessionExpiredCallback } from '@/services/apiClient'
import { tokenStorage } from '@/lib/tokenStorage'
import { UserProfile } from '@/types/user'
import { LoginCredentials, RegisterCredentials } from '@/types/auth'
import { AuthContext } from './auth-context-def'

export type { AuthContextType } from './auth-context-def'

interface AuthProviderProps {
  children: React.ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<UserProfile | null>(null)
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false)
  const [isLoading, setIsLoading] = useState<boolean>(true)

  // Khôi phục hồ sơ người dùng từ backend
  const refreshProfile = useCallback(async () => {
    try {
      const response = await authService.getProfile()
      if (response.success && response.data) {
        const profile = response.data
        setUser({
          userId: profile.userId,
          id: profile.userId,
          email: profile.email,
          fullName: profile.fullName,
          roles: profile.roles || [],
          permissions: profile.permissions || [],
        })
        setIsAuthenticated(true)
      } else {
        throw new Error(response.message || 'Không thể lấy thông tin người dùng')
      }
    } catch {
      // Khi không lấy được profile hợp lệ, dọn sạch session để đảm bảo tính nhất quán
      tokenStorage.clearTokens()
      setUser(null)
      setIsAuthenticated(false)
    }
  }, [])

  // Đăng nhập
  const login = useCallback(
    async (credentials: LoginCredentials) => {
      // 1. Dọn dẹp session cũ để tránh xung đột dữ liệu tài khoản trước
      tokenStorage.clearTokens()
      setUser(null)
      setIsAuthenticated(false)

      // 2. Gửi yêu cầu đăng nhập lên backend
      const response = await authService.login(credentials)
      if (!response.success || !response.data) {
        throw new Error(response.message || 'Đăng nhập không thành công')
      }

      const { accessToken, refreshToken, userId, email, fullName, roles, permissions } = response.data

      // 3. Lưu trữ token phiên mới
      tokenStorage.setTokens(accessToken, refreshToken)

      // 4. Cập nhật state người dùng từ dữ liệu thực tế do backend trả về
      setUser({
        userId,
        id: userId,
        email,
        fullName,
        roles: roles || [],
        permissions: permissions || [],
      })
      setIsAuthenticated(true)
    },
    []
  )

  // Đăng ký tài khoản mới (không tự động lưu token hay đăng nhập ngầm)
  const register = useCallback(
    async (credentials: RegisterCredentials) => {
      tokenStorage.clearTokens()
      setUser(null)
      setIsAuthenticated(false)

      const response = await authService.register(credentials)
      if (response && response.success === false) {
        throw new Error(response.message || 'Đăng ký không thành công')
      }
    },
    []
  )

  // Đăng xuất
  const logout = useCallback(async () => {
    const refreshToken = tokenStorage.getRefreshToken()
    try {
      if (refreshToken) {
        await authService.logout(refreshToken)
      }
    } catch (error) {
      console.warn('Lỗi thu hồi token phía backend khi đăng xuất:', error)
    } finally {
      // Luôn dọn sạch triệt để storage và state ở frontend
      tokenStorage.clearTokens()
      setUser(null)
      setIsAuthenticated(false)
    }
  }, [])

  // Khởi tạo và lắng nghe phiên làm việc khi load trang
  useEffect(() => {
    const initAuth = async () => {
      const refreshToken = tokenStorage.getRefreshToken()
      const accessToken = tokenStorage.getAccessToken()

      if (!refreshToken && !accessToken) {
        setUser(null)
        setIsAuthenticated(false)
        setIsLoading(false)
        return
      }

      await refreshProfile()
      setIsLoading(false)
    }

    // Đăng ký callback khi refresh token thất bại
    setOnSessionExpiredCallback(() => {
      setUser(null)
      setIsAuthenticated(false)
    })

    initAuth()
  }, [refreshProfile])

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        login,
        register,
        logout,
        refreshProfile,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}
