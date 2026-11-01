export interface Ngo {
  id: number
  name: string
  description?: string | null
  logoUrl?: string | null
  website?: string | null
  contactEmail?: string | null
  phone?: string | null
  address?: string | null
  isActive: boolean
  displayOrder: number
  causes?: Array<{ causeId: number; name: string; slug: string }>
  totalProgrammesCount: number
}

export interface Programme {
  id: number
  ngoId: number
  ngoName: string
  ngoLogoUrl?: string | null
  title: string
  description: string
  category: string
  location?: string | null
  startDate: string
  endDate?: string | null
  imageUrl?: string | null
  status: string
  isFeatured: boolean
  registrationsCount: number
}

export interface ContactQueryRequest {
  fullName: string
  email: string
  subject: string
  message: string
}
