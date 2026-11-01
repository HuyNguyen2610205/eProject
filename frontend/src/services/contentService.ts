import { apiClient } from './apiClient'
import { ApiResponse } from '@/types/api'
import { Ngo, Programme, ContactQueryRequest } from '@/types/content'

export const contentService = {
  /**
   * Lấy danh sách các tổ chức NGO đối tác
   */
  async getNgos(): Promise<ApiResponse<Ngo[]>> {
    const response = await apiClient.get<ApiResponse<Ngo[]>>('/content/ngos')
    return response.data
  },

  /**
   * Lấy danh sách các chương trình thiện nguyện
   */
  async getProgrammes(params?: {
    featuredOnly?: boolean
    status?: string
    page?: number
    pageSize?: number
  }): Promise<ApiResponse<Programme[]>> {
    const response = await apiClient.get<ApiResponse<Programme[]>>('/content/programmes', {
      params,
    })
    return response.data
  },

  /**
   * Gửi câu hỏi / yêu cầu hỗ trợ đến Help Centre
   */
  async submitQuery(data: ContactQueryRequest): Promise<ApiResponse<unknown>> {
    const response = await apiClient.post<ApiResponse<unknown>>('/content/queries', data)
    return response.data
  },
}
