/**
 * Định dạng ngày giờ theo chuẩn hiển thị Tiếng Việt
 */
export function formatDate(dateInput: string | Date | null | undefined): string {
  if (!dateInput) return '—'
  const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput
  if (isNaN(date.getTime())) return '—'

  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}

/**
 * Định dạng số tiền VND
 */
export function formatCurrency(amount: number | null | undefined): string {
  if (amount == null) return '0 ₫'
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  }).format(amount)
}

/**
 * Rút gọn chuỗi nếu vượt quá độ dài tối đa
 */
export function truncate(text: string, maxLength = 60): string {
  if (!text || text.length <= maxLength) return text
  return text.slice(0, maxLength) + '...'
}
