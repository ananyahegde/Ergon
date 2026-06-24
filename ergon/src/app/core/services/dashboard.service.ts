import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { HrDashboardSummary } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);

  getHrSummary() {
    return this.http.get<HrDashboardSummary>(`${environment.apiUrl}/dashboard/hr-summary`);
  }
}
