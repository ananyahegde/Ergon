import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Department, Branch, Designation } from '../models/master.model';

@Injectable({
  providedIn: 'root'
})
export class MasterService {
  private http = inject(HttpClient);

  getDepartments() {
    return this.http.get<Department[]>(`${environment.apiUrl}/departments`);
  }

  getBranches() {
    return this.http.get<Branch[]>(`${environment.apiUrl}/branches`);
  }

  getDesignations() {
    return this.http.get<Designation[]>(`${environment.apiUrl}/designations`);
  }
}
