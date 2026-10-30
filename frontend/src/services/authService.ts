import { apiClient } from './apiClient'
import { ApiResponse } from '@/types/api'
import {
  AuthResponseData,
  LoginCredentials,
  RegisterCredentials,
  RefreshTokenRequest,
  LogoutRequest,
  ForgotPasswordRequest,
  ForgotPasswordResponseData,
  ResetPasswordRequest,
  ChangePasswordRequest,
} from '@/types/auth'
import { UserProfile } from '@/types/user'

export const authService = {
  /**
   * Đăng nhập với email và mật khẩu
   */
  async login(credentials: LoginCredentials): Promise<ApiResponse<AuthResponseData>> {
    const response = await apiClient.post<ApiResponse<AuthResponseData>>(
      '/auth/login',
      credentials
    )
    return response.data
  },

  /**
   * Đăng ký tài khoản người dùng mới (hỗ trợ 200 OK, 201 Created và 204 No Content)
   */
  async register(credentials: RegisterCredentials): Promise<ApiResponse<AuthResponseData | null>> {
    const response = await apiClient.post<ApiResponse<AuthResponseData>>(
      '/auth/register',
      credentials
    )
    if (response.status === 204 || !response.data) {
      return {
        success: true,
        message: 'Đăng ký tài khoản thành công.',
        data: null,
        errors: null,
      }
    }
    return response.data
  },

  /**
   * Làm mới Access Token thông qua cặp Access Token cũ và Refresh Token
   */
  async refreshToken(tokens: RefreshTokenRequest): Promise<ApiResponse<AuthResponseData>> {
    const response = await apiClient.post<ApiResponse<AuthResponseData>>(
      '/auth/refresh-token',
      tokens
    )
    return response.data
  },

  /**
   * Đăng xuất và thu hồi Refresh Token trên backend
   */
  async logout(refreshToken: string): Promise<ApiResponse<boolean>> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      '/auth/logout',
      { refreshToken } as LogoutRequest
    )
    return response.data
  },

  /**
   * Lấy thông tin hồ sơ của người dùng hiện tại đang đăng nhập
   */
  async getProfile(): Promise<ApiResponse<UserProfile>> {
    const response = await apiClient.get<ApiResponse<UserProfile>>('/auth/profile')
    return response.data
  },

  /**
   * Yêu cầu đặt lại mật khẩu qua email
   */
  async forgotPassword(data: ForgotPasswordRequest): Promise<ApiResponse<ForgotPasswordResponseData>> {
    const response = await apiClient.post<ApiResponse<ForgotPasswordResponseData>>(
      '/auth/forgot-password',
      data
    )
    return response.data
  },

  /**
   * Đặt lại mật khẩu mới với mã xác thực (Token)
   */
  async resetPassword(data: ResetPasswordRequest): Promise<ApiResponse<boolean>> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      '/auth/reset-password',
      data
    )
    return response.data
  },

  /**
   * Đổi mật khẩu cho người dùng hiện hành (yêu cầu Authorization Bearer Token)
   */
  async changePassword(data: ChangePasswordRequest): Promise<ApiResponse<boolean>> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      '/auth/change-password',
      data
    )
    return response.data
  },
}
