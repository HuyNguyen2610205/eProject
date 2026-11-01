import { apiClient } from './apiClient'
import { ApiResponse } from '@/types/api'
import { Cause, DonateRequest, DonationResponse } from '@/types/donation'

export const donationService = {
  /**
   * Lấy danh sách tất cả các danh mục quyên góp (Causes)
   */
  async getAllCauses(includeInactive = false): Promise<ApiResponse<Cause[]>> {
    const response = await apiClient.get<ApiResponse<Cause[]>>('/donations/causes', {
      params: { includeInactive },
    })
    return response.data
  },

  /**
   * Lấy chi tiết một danh mục quyên góp theo Id hoặc Slug
   */
  async getCauseByIdOrSlug(idOrSlug: string | number): Promise<ApiResponse<Cause>> {
    const response = await apiClient.get<ApiResponse<Cause>>(`/donations/causes/${idOrSlug}`)
    return response.data
  },

  /**
   * Tiếp nhận quyên góp trực tuyến
   */
  async donate(data: DonateRequest): Promise<ApiResponse<DonationResponse>> {
    const response = await apiClient.post<ApiResponse<DonationResponse>>('/donations', data)
    return response.data
  },

  /**
   * Tra cứu thông tin giao dịch quyên góp bằng mã giao dịch
   */
  async trackDonation(transactionCode: string): Promise<ApiResponse<DonationResponse>> {
    const response = await apiClient.get<ApiResponse<DonationResponse>>(
      `/donations/track/${transactionCode}`
    )
    return response.data
  },

  /**
   * Lấy lịch sử quyên góp của người dùng hiện hành (yêu cầu đăng nhập)
   */
  async getMyDonationHistory(): Promise<ApiResponse<DonationResponse[]>> {
    const response = await apiClient.get<ApiResponse<DonationResponse[]>>('/donations/my-history')
    return response.data
  },
}
