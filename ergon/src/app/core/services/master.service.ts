import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Department, Branch, Designation, Shift, Role, SalaryStructure, LeaveEntitlement, Country, State, City } from '../models/master.model';

@Injectable({
  providedIn: 'root'
})
export class MasterService {
  private http = inject(HttpClient);

  departments = signal<Department[]>([]);
  branches = signal<Branch[]>([]);
  designations = signal<Designation[]>([]);
  shifts = signal<Shift[]>([]);
  roles = signal<Role[]>([]);
  salaryStructures = signal<SalaryStructure[]>([]);
  leaveEntitlements = signal<LeaveEntitlement[]>([]);
  countries = signal<Country[]>([]);
  states = signal<State[]>([]);
  cities = signal<City[]>([]);

  loadAll() {
    return forkJoin({
      departments: this.http.get<Department[]>(`${environment.apiUrl}/departments`),
      branches: this.http.get<Branch[]>(`${environment.apiUrl}/branches`),
      designations: this.http.get<Designation[]>(`${environment.apiUrl}/designations`),
      shifts: this.http.get<Shift[]>(`${environment.apiUrl}/shifts`),
      roles: this.http.get<Role[]>(`${environment.apiUrl}/roles`),
      salaryStructures: this.http.get<SalaryStructure[]>(`${environment.apiUrl}/salary-structures`),
      leaveEntitlements: this.http.get<LeaveEntitlement[]>(`${environment.apiUrl}/leave-entitlements`),
      countries: this.http.get<Country[]>(`${environment.apiUrl}/countries`),
      states: this.http.get<State[]>(`${environment.apiUrl}/states`),
      cities: this.http.get<City[]>(`${environment.apiUrl}/cities`)
    }).pipe(
      tap(data => {
        this.departments.set(data.departments);
        this.branches.set(data.branches);
        this.designations.set(data.designations);
        this.shifts.set(data.shifts);
        this.roles.set(data.roles);
        this.salaryStructures.set(data.salaryStructures);
        this.leaveEntitlements.set(data.leaveEntitlements);
        this.countries.set(data.countries);
        this.states.set(data.states);
        this.cities.set(data.cities);
      })
    );
  }

  createDepartment(payload: { departmentName: string }) {
    return this.http.post<Department>(`${environment.apiUrl}/departments`, payload).pipe(
      tap(created => this.departments.update(list => [...list, created]))
    );
  }

  updateDepartment(id: number, payload: { departmentName: string }) {
    return this.http.put<Department>(`${environment.apiUrl}/departments/${id}`, payload).pipe(
      tap(updated => this.departments.update(list => list.map(d => d.departmentId === id ? updated : d)))
    );
  }

  createBranch(payload: { branchName: string }) {
    return this.http.post<Branch>(`${environment.apiUrl}/branches`, payload).pipe(
      tap(created => this.branches.update(list => [...list, created]))
    );
  }

  updateBranch(id: number, payload: { branchName: string }) {
    return this.http.put<Branch>(`${environment.apiUrl}/branches/${id}`, payload).pipe(
      tap(updated => this.branches.update(list => list.map(b => b.branchId === id ? updated : b)))
    );
  }
  createDesignation(payload: { designationName: string }) {
    return this.http.post<Designation>(`${environment.apiUrl}/designations`, payload).pipe(
      tap(created => this.designations.update(list => [...list, created]))
    );
  }

  updateDesignation(id: number, payload: { designationName: string }) {
    return this.http.put<Designation>(`${environment.apiUrl}/branches/${id}`, payload).pipe(
      tap(updated => this.designations.update(list => list.map(d => d.designationId === id ? updated : d)))
    );
  }
  createRole(payload: { roleName: string }) {
    return this.http.post<Role>(`${environment.apiUrl}/roles`, payload).pipe(
      tap(created => this.roles.update(list => [...list, created]))
    );
  }

  updateRole(id: number, payload: { roleName: string }) {
    return this.http.put<Role>(`${environment.apiUrl}/roles/${id}`, payload).pipe(
      tap(updated => this.roles.update(list => list.map(r => r.roleId === id ? updated : r)))
    );
  }

  createCountry(payload: { countryName: string }) {
    return this.http.post<Country>(`${environment.apiUrl}/countries`, payload).pipe(
      tap(created => this.countries.update(list => [...list, created]))
    );
  }

  updateCountry(id: number, payload: { countryName: string }) {
    return this.http.put<Country>(`${environment.apiUrl}/countries/${id}`, payload).pipe(
      tap(updated => this.countries.update(list => list.map(c => c.countryId === id ? updated : c)))
    );
  }

  createState(payload: { stateName: string }) {
    return this.http.post<State>(`${environment.apiUrl}/states`, payload).pipe(
      tap(created => this.states.update(list => [...list, created]))
    );
  }

  updateState(id: number, payload: { stateName: string }) {
    return this.http.put<State>(`${environment.apiUrl}/states/${id}`, payload).pipe(
      tap(updated => this.states.update(list => list.map(s => s.stateId === id ? updated : s)))
    );
  }

  createCity(payload: { cityName: string }) {
    return this.http.post<City>(`${environment.apiUrl}/cities`, payload).pipe(
      tap(created => this.cities.update(list => [...list, created]))
    );
  }

  updateCity(id: number, payload: { cityName: string }) {
    return this.http.put<City>(`${environment.apiUrl}/cities/${id}`, payload).pipe(
      tap(updated => this.cities.update(list => list.map(c => c.cityId === id ? updated : c)))
    );
  }
}