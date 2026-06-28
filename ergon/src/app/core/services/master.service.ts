import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, tap, switchMap, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Department, 
  Branch, 
  Designation, 
  Shift, 
  Role, 
  SalaryStructure, 
  LeaveEntitlement, 
  Country, 
  State, 
  City, 
  LeaveType, 
  PublicHoliday, 
  TaxSlab,
  SalaryComponent, 
  LeaveEntitlementComponent 
} from '../models/master.model';

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
  leaveTypes = signal<LeaveType[]>([]);
  publicHolidays = signal<PublicHoliday[]>([]);
  taxSlabs = signal<TaxSlab[]>([]);
  salaryComponents = signal<Record<number, SalaryComponent[]>>({});
  leaveEntitlementComponents = signal<Record<number, LeaveEntitlementComponent[]>>({});

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
      cities: this.http.get<City[]>(`${environment.apiUrl}/cities`),
      leaveTypes: this.http.get<LeaveType[]>(`${environment.apiUrl}/leave-types`),
      publicHolidays: this.http.get<PublicHoliday[]>(`${environment.apiUrl}/public-holidays`),
      taxSlabs: this.http.get<TaxSlab[]>(`${environment.apiUrl}/tax-slabs`)
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
        this.leaveTypes.set(data.leaveTypes);
        this.publicHolidays.set(data.publicHolidays);
        this.taxSlabs.set(data.taxSlabs);
      }),
      switchMap(data => {
        const salaryRequests = data.salaryStructures.length
          ? forkJoin(Object.fromEntries(
              data.salaryStructures.map(s => [
                s.salaryStructureId,
                this.http.get<SalaryComponent[]>(`${environment.apiUrl}/salary-structures/${s.salaryStructureId}/salary-components`)
              ])
            ))
          : of({} as Record<string, SalaryComponent[]>);

        const leaveRequests = data.leaveEntitlements.length
          ? forkJoin(Object.fromEntries(
              data.leaveEntitlements.map(l => [
                l.leaveEntitlementId,
                this.http.get<LeaveEntitlementComponent[]>(`${environment.apiUrl}/leave-entitlements/${l.leaveEntitlementId}/leave-entitlement-components`)
              ])
            ))
          : of({} as Record<string, LeaveEntitlementComponent[]>);

        return forkJoin({ salaryComponents: salaryRequests, leaveEntitlementComponents: leaveRequests });
      }),
      tap(data => {
        this.salaryComponents.set(data.salaryComponents as Record<number, SalaryComponent[]>);
        this.leaveEntitlementComponents.set(data.leaveEntitlementComponents as Record<number, LeaveEntitlementComponent[]>);
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
    return this.http.put<Designation>(`${environment.apiUrl}/designations/${id}`, payload).pipe(
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
  createShift(payload: { shiftName: string; startTime: string; endTime: string }) {
    return this.http.post<Shift>(`${environment.apiUrl}/shifts`, payload).pipe(
      tap(created => this.shifts.update(list => [...list, created]))
    );
  }

  updateShift(id: number, payload: { shiftName: string; startTime: string; endTime: string }) {
    return this.http.put<Shift>(`${environment.apiUrl}/shifts/${id}`, payload).pipe(
      tap(updated => this.shifts.update(list => list.map(s => s.shiftId === id ? updated : s)))
    );
  }
  createLeaveType(payload: { leaveTypeName: string }) {
    return this.http.post<LeaveType>(`${environment.apiUrl}/leave-types`, payload).pipe(
      tap(created => this.leaveTypes.update(list => [...list, created]))
    );
  }

  updateLeaveType(id: number, payload: { leaveTypeName: string }) {
    return this.http.put<LeaveType>(`${environment.apiUrl}/leave-types/${id}`, payload).pipe(
      tap(updated => this.leaveTypes.update(list => list.map(l => l.leaveTypeId === id ? updated : l)))
    );
  }
  createPublicHoliday(payload: { publicHolidayName: string; publicHolidayDate: string }) {
    return this.http.post<PublicHoliday>(`${environment.apiUrl}/public-holidays`, payload).pipe(
      tap(created => this.publicHolidays.update(list => [...list, created]))
    );
  }

  updatePublicHoliday(id: number, payload: { publicHolidayName: string; publicHolidayDate: string }) {
    return this.http.put<PublicHoliday>(`${environment.apiUrl}/public-holidays/${id}`, payload).pipe(
      tap(updated => this.publicHolidays.update(list => list.map(p => p.publicHolidayId === id ? updated : p)))
    );
  }

  createTaxSlab(payload: { minIncome: number; maxIncome: number; taxPercentage: number }) {
    return this.http.post<TaxSlab>(`${environment.apiUrl}/tax-slabs`, payload).pipe(
      tap(created => this.taxSlabs.update(list => [...list, created]))
    );
  }

  updateTaxSlab(id: number, payload: { minIncome: number; maxIncome: number; taxPercentage: number }) {
    return this.http.put<TaxSlab>(`${environment.apiUrl}/tax-slabs/${id}`, payload).pipe(
      tap(updated => this.taxSlabs.update(list => list.map(t => t.taxSlabId === id ? updated : t)))
    );
  }

  createSalaryStructure(payload: { salaryStructureName: string }) {
    return this.http.post<SalaryStructure>(`${environment.apiUrl}/salary-structures`, payload).pipe(
      tap(created => this.salaryStructures.update(list => [...list, created]))
    );
  }

  updateSalaryStructure(id: number, payload: { salaryStructureName: string }) {
    return this.http.put<SalaryStructure>(`${environment.apiUrl}/salary-structures/${id}`, payload).pipe(
      tap(updated => this.salaryStructures.update(list => list.map(s => s.salaryStructureId === id ? updated : s)))
    );
  }

  createSalaryComponent(salaryStructureId: number, payload: { componentName: string; componentType: string; amount: number }) {
    return this.http.post<SalaryComponent>(`${environment.apiUrl}/salary-structures/${salaryStructureId}/salary-components`, payload).pipe(
      tap(created => this.salaryComponents.update(map => ({
        ...map,
        [salaryStructureId]: [...(map[salaryStructureId] ?? []), created]
      })))
    );
  }

  updateSalaryComponent(salaryStructureId: number, componentId: number, payload: { componentName: string; componentType: string; amount: number }) {
    return this.http.put<SalaryComponent>(`${environment.apiUrl}/salary-structures/${salaryStructureId}/salary-components/${componentId}`, payload).pipe(
      tap(updated => this.salaryComponents.update(map => ({
        ...map,
        [salaryStructureId]: map[salaryStructureId].map(c => c.salaryComponentId === componentId ? updated : c)
      })))
    );
  }

  createLeaveEntitlement(payload: { leaveEntitlementName: string }) {
    return this.http.post<LeaveEntitlement>(`${environment.apiUrl}/leave-entitlements`, payload).pipe(
      tap(created => this.leaveEntitlements.update(list => [...list, created]))
    );
  }

  updateLeaveEntitlement(id: number, payload: { leaveEntitlementName: string }) {
    return this.http.put<LeaveEntitlement>(`${environment.apiUrl}/leave-entitlements/${id}`, payload).pipe(
      tap(updated => this.leaveEntitlements.update(list => list.map(l => l.leaveEntitlementId === id ? updated : l)))
    );
  }

  createLeaveEntitlementComponent(leaveEntitlementId: number, payload: { leaveTypeId: number; totalDays: number }) {
    return this.http.post<LeaveEntitlementComponent>(`${environment.apiUrl}/leave-entitlements/${leaveEntitlementId}/leave-entitlement-components`, payload).pipe(
      tap(created => this.leaveEntitlementComponents.update(map => ({
        ...map,
        [leaveEntitlementId]: [...(map[leaveEntitlementId] ?? []), created]
      })))
    );
  }

  updateLeaveEntitlementComponent(leaveEntitlementId: number, componentId: number, payload: { totalDays: number }) {
    return this.http.put<LeaveEntitlementComponent>(`${environment.apiUrl}/leave-entitlements/${leaveEntitlementId}/leave-entitlement-components/${componentId}`, payload).pipe(
      tap(updated => this.leaveEntitlementComponents.update(map => ({
        ...map,
        [leaveEntitlementId]: map[leaveEntitlementId].map(c => c.leaveEntitlementComponentId === componentId ? updated : c)
      })))
    );
  }
}