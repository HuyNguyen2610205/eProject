import { UserProfile } from './user'

export interface AuthTokens {
  accessToken: string
  refreshToken: string
  accessTokenExpiresAt?: string
  refreshTokenExpiresAt?: string
}

export interface AuthResponseData extends UserProfile, AuthTokens {}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterCredentials {
  email: string
  password: string
  fullName: string
  confirmPassword?: string
}

export interface RefreshTokenRequest {
  accessToken: string
  refreshToken: string
}

export interface LogoutRequest {
  refreshToken: string
}

export interface ForgotPasswordRequest {
  email: string
}

export interface ForgotPasswordResponseData {
  email?: string
  resetToken?: string
  message?: string
}

export interface ResetPasswordRequest {
  email: string
  token: string
  newPassword: string
  confirmNewPassword: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
  confirmNewPassword: string
}


