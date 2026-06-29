import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  PagedReviewCycleResponse,
  ReviewCycle,
  PagedReviewCycleDetailsResponse,
  ReviewCycleDetails,
  GetAllReviewCyclesRequest,
  GetAllReviewCycleDetailsRequest,
  CreateReviewCycleRequest,
  CreateReviewCycleDetailsRequest,
  UpdateSelfScoreRequest,
  UpdateFeedbackRequest
} from '../models/performance.model';

@Injectable({ providedIn: 'root' })
export class PerformanceService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/review-cycles`;

  getAll(request: GetAllReviewCyclesRequest) {
    let params = new HttpParams();
    if (request.pageNumber) params = params.set('PageNumber', request.pageNumber);
    if (request.pageSize) params = params.set('PageSize', request.pageSize);
    if (request.status) params = params.set('Status', request.status);
    return this.http.get<PagedReviewCycleResponse>(this.baseUrl, { params });
  }

  getById(id: string) {
    return this.http.get<ReviewCycle>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateReviewCycleRequest) {
    return this.http.post<ReviewCycle>(this.baseUrl, payload);
  }

  startCycle(id: string) {
    return this.http.patch<ReviewCycle>(`${this.baseUrl}/${id}/start`, {});
  }

  closeCycle(id: string) {
    return this.http.put<{ message: string; data: ReviewCycle }>(`${this.baseUrl}/${id}/close`, {});
  }

  getDetails(reviewCycleId: string, request: GetAllReviewCycleDetailsRequest) {
    let params = new HttpParams();
    if (request.pageNumber) params = params.set('PageNumber', request.pageNumber);
    if (request.pageSize) params = params.set('PageSize', request.pageSize);
    return this.http.get<PagedReviewCycleDetailsResponse>(`${this.baseUrl}/${reviewCycleId}/review-cycle-details`, { params });
  }

  getMyTeamDetails(reviewCycleId: string, request: GetAllReviewCycleDetailsRequest) {
    let params = new HttpParams();
    if (request.pageNumber) params = params.set('PageNumber', request.pageNumber);
    if (request.pageSize) params = params.set('PageSize', request.pageSize);
    return this.http.get<PagedReviewCycleDetailsResponse>(`${this.baseUrl}/${reviewCycleId}/review-cycle-details/my-team`, { params });
  }

  createDetail(reviewCycleId: string, payload: CreateReviewCycleDetailsRequest) {
    return this.http.post<ReviewCycleDetails>(`${this.baseUrl}/${reviewCycleId}/review-cycle-details`, payload);
  }

  submitSelfScore(reviewCycleId: string, detailId: string, payload: UpdateSelfScoreRequest) {
    return this.http.put<ReviewCycleDetails>(`${this.baseUrl}/${reviewCycleId}/review-cycle-details/${detailId}/self-score`, payload);
  }

  submitFeedback(reviewCycleId: string, detailId: string, payload: UpdateFeedbackRequest) {
    return this.http.put<ReviewCycleDetails>(`${this.baseUrl}/${reviewCycleId}/review-cycle-details/${detailId}/feedback`, payload);
  }
}