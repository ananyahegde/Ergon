import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { EmployeeDetailResponse } from '../models/employee.model';
import { UpdateProfileRequest } from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/employees`;

  getProfile(id: string) {
    return this.http.get<EmployeeDetailResponse>(`${this.baseUrl}/${id}`);
  }

  updateProfile(id: string, request: UpdateProfileRequest) {
    return this.http.put<EmployeeDetailResponse>(`${this.baseUrl}/${id}/profile`, request);
  }

  updatePfp(id: string, file: File) {
    const formData = new FormData();
    formData.append('pfp', file);
    return this.http.put<void>(`${this.baseUrl}/${id}/pfp`, formData);
  }

  getPfp(id: string) {
    return this.http.get(`${this.baseUrl}/${id}/pfp`, { responseType: 'blob' });
  }

  changePassword(request: { oldPassword: string; newPassword: string }) {
    return this.http.put<void>(`${environment.apiUrl}/auth/change-password`, request);
  }
}