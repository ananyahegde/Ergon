import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  GetAllEmployeesRequest, 
  PagedEmployeeResponse, 
  EmployeeDetailResponse, 
  EmployeeDocument,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
  UpdateEmployeeStatusRequest,
  EmployeeListItem } from '../models/employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/employees`;

  getAll(request: GetAllEmployeesRequest) {
    let params = new HttpParams();

    if (request.pageNumber) params = params.set('PageNumber', request.pageNumber);
    if (request.pageSize) params = params.set('PageSize', request.pageSize);
    if (request.sortDirection) params = params.set('SortDirection', request.sortDirection);
    if (request.search) params = params.set('Search', request.search);
    if (request.departmentIds?.length) request.departmentIds.forEach(id => params = params.append('DepartmentIds', id));
    if (request.designationIds?.length) request.designationIds.forEach(id => params = params.append('DesignationIds', id));
    if (request.branchIds?.length) request.branchIds.forEach(id => params = params.append('BranchIds', id));
    if (request.employmentStatuses?.length) request.employmentStatuses.forEach(s => params = params.append('EmploymentStatuses', s));

    return this.http.get<PagedEmployeeResponse>(this.baseUrl, { params });
  }

  getById(id: string) {
    return this.http.get<EmployeeDetailResponse>(`${this.baseUrl}/${id}`);
  }

  getPfp(employeeId: string) {
    return this.http.get(`${this.baseUrl}/${employeeId}/pfp`, { responseType: 'blob' });
  }


  getDocuments(id: string) {
    return this.http.get<EmployeeDocument[]>(`${this.baseUrl}/${id}/documents`);
  }

  downloadDocument(employeeId: string, documentId: string) {
    return this.http.get(`${this.baseUrl}/${employeeId}/documents/${documentId}/download`, {
      responseType: 'blob'
    });
  }

  create(request: CreateEmployeeRequest) {
    return this.http.post<EmployeeDetailResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateEmployeeRequest) {
    return this.http.put<EmployeeDetailResponse>(`${this.baseUrl}/${id}`, request);
  }

    updateStatus(id: string, status: number) {
      return this.http.put<UpdateEmployeeStatusRequest>(`${this.baseUrl}/${id}/status`, { employmentStatus: status });
  }

  getMyTeam() {
    return this.http.get<EmployeeListItem[]>(`${this.baseUrl}/my-team`);
  }
}
