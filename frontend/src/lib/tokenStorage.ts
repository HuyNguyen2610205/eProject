const ACCESS_TOKEN_KEY = 'eproject_access_token'
const REFRESH_TOKEN_KEY = 'eproject_refresh_token'

export const tokenStorage = {
  getAccessToken(): string | null {
    try {
      return localStorage.getItem(ACCESS_TOKEN_KEY)
    } catch {
      return null
    }
  },

  getRefreshToken(): string | null {
    try {
      return localStorage.getItem(REFRESH_TOKEN_KEY)
    } catch {
      return null
    }
  },

  setTokens(accessToken: string, refreshToken: string): void {
    try {
      localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
    } catch (error) {
      console.error('Không thể lưu trữ token vào localStorage:', error)
    }
  },

  setAccessToken(accessToken: string): void {
    try {
      localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
    } catch (error) {
      console.error('Không thể lưu trữ access token:', error)
    }
  },

  clearTokens(): void {
    try {
      localStorage.removeItem(ACCESS_TOKEN_KEY)
      localStorage.removeItem(REFRESH_TOKEN_KEY)
    } catch (error) {
      console.error('Không thể xóa token khỏi localStorage:', error)
    }
  },
}
