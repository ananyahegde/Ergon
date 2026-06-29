export type ReviewCycleStatus = 0 | 1 | 2;

export const REVIEW_CYCLE_STATUS_LABELS: Record<ReviewCycleStatus, string> = {
  0: 'Draft',
  1: 'Active',
  2: 'Closed'
};

export interface ReviewCycle {
  reviewCycleId: string;
  reviewName: string;
  startDate: string;
  endDate: string;
  reviewCycleStatus: ReviewCycleStatus;
  createdAt: string;
  updatedAt: string;
}

export interface PagedReviewCycleResponse {
  items: ReviewCycle[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ReviewCycleDetails {
  reviewCycleDetailsId: string;
  employeeName: string;
  department: string;
  reviewCycleName: string;
  selfScore: number;
  feedbackScore: number;
  managerComments: string;
  createdAt: string;
  updatedAt: string;
}

export interface PagedReviewCycleDetailsResponse {
  items: ReviewCycleDetails[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface GetAllReviewCyclesRequest {
  pageNumber?: number;
  pageSize?: number;
  status?: string;
}

export interface GetAllReviewCycleDetailsRequest {
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateReviewCycleRequest {
  reviewName: string;
  startDate: string;
  endDate: string;
}

export interface CreateReviewCycleDetailsRequest {
  employeeId: string;
}

export interface UpdateSelfScoreRequest {
  selfScore: number;
}

export interface UpdateFeedbackRequest {
  feedbackScore: number;
  managerComments: string;
}