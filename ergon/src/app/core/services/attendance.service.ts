import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  PagedAttendanceResponse,
  GetAllAttendancesRequest,
  AttendanceTodaySummary,
  MyTodayAttendance,
  AttendanceRecord,
  EmployeeDashboardSummary
} from '../models/attendance.model';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/attendances`;

  getAll(request: GetAllAttendancesRequest) {
    let params = new HttpParams();
    if (request.pageNumber) params = params.set('PageNumber', request.pageNumber);
    if (request.pageSize) params = params.set('PageSize', request.pageSize);
    if (request.employeeId) params = params.set('EmployeeId', request.employeeId);
    if (request.month) params = params.set('Month', request.month);
    if (request.year) params = params.set('Year', request.year);
    if (request.status?.length) request.status.forEach(s => params = params.append('Status', s));
    return this.http.get<PagedAttendanceResponse>(this.baseUrl, { params });
  }

  getToday() {
    return this.http.get<AttendanceTodaySummary>(`${this.baseUrl}/today`);
  }

  getMyToday() {
    return this.http.get<MyTodayAttendance | null>(`${this.baseUrl}/me/today`);
  }

  clockIn() {
    return this.http.post<AttendanceRecord>(`${this.baseUrl}/clock-in`, {});
  }

  clockOut(attendanceId: string) {
    return this.http.put<AttendanceRecord>(`${this.baseUrl}/${attendanceId}/clock-out`, {});
  }

  getEmployeeSummary() {
    return this.http.get<EmployeeDashboardSummary>(`${environment.apiUrl}/dashboard/employee-summary`);
  }
}