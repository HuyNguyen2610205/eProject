export interface Cause {
  id: number
  name: string
  slug: string
  description?: string | null
  iconUrl?: string | null
  imageUrl?: string | null
  isActive: boolean
  displayOrder: number
  totalRaised: number
  totalDonationsCount: number
  createdAt: string
  updatedAt?: string | null
}

export interface DonateRequest {
  causeId: number
  donorName: string
  donorEmail: string
  donorPhone?: string
  amount: number
  notes?: string
  cardHolderName: string
  cardNumber: string
  expiryDate: string
  cvv: string
  simulateFailure?: boolean
}

export interface DonationResponse {
  id: string
  transactionCode: string
  causeId: number
  causeName: string
  causeSlug: string
  userId?: string | null
  donorName: string
  donorEmail: string
  donorPhone?: string | null
  amount: number
  paymentMethod: string
  cardLastFour?: string | null
  status: string
  notes?: string | null
  createdAt: string
  completedAt?: string | null
}
