import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../core/services/employee.service';
import { MasterService } from '../../core/services/master.service';
import { EmployeeListItem, GetAllEmployeesRequest, EMPLOYMENT_STATUSES, EmployeeStatsResponse } from '../../core/models/employee.model';
import { MultiSelectDropdown } from '../../shared/components/multi-select-dropdown/multi-select-dropdown';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [FormsModule, MultiSelectDropdown, RouterLink],
  templateUrl: './employees.html',
  styleUrl: './employees.css'
})
export class Employees implements OnInit {
  private employeeService = inject(EmployeeService);
  private masterService = inject(MasterService);
  private router = inject(Router);

  employees = signal<EmployeeListItem[]>([]);
  stats = signal<EmployeeStatsResponse | null>(null);
  private searchSubject = new Subject<string>();
  totalCount = signal(0);
  totalPages = signal(0);
  isLoading = signal(false);

  departments = this.masterService.departments;
  branches = this.masterService.branches;
  designations = this.masterService.designations;
  employmentStatuses = EMPLOYMENT_STATUSES;

  search = signal('');
  selectedDepartmentIds = signal<number[]>([]);
  selectedBranchIds = signal<number[]>([]);
  selectedDesignationIds = signal<number[]>([]);
  selectedStatuses = signal<string[]>([]);
  pageNumber = signal(1);
  pageSize = signal(12);

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.pageNumber.set(1);
      this.loadEmployees();
    });
    this.loadEmployees();
    this.loadStats();
  }

  loadStats() {
    this.employeeService.getStats().subscribe({
      next: data => this.stats.set(data),
      error: () => {}
    });
  }
  
  loadEmployees() {
    this.isLoading.set(true);
    const request: GetAllEmployeesRequest = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      search: this.search() || undefined,
      departmentIds: this.selectedDepartmentIds().length ? this.selectedDepartmentIds() : undefined,
      branchIds: this.selectedBranchIds().length ? this.selectedBranchIds() : undefined,
      designationIds: this.selectedDesignationIds().length ? this.selectedDesignationIds() : undefined,
      employmentStatuses: this.selectedStatuses().length ? this.selectedStatuses() : undefined
    };

    this.employeeService.getAll(request).subscribe({
      next: res => {
        this.employees.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch(value: string) {
    this.search.set(value);
    this.searchSubject.next(value);
  }

  statusOptions = EMPLOYMENT_STATUSES.map(s => ({ label: s, value: s }));

  hasActiveFilters = computed(() =>
    this.selectedDepartmentIds().length > 0 ||
    this.selectedBranchIds().length > 0 ||
    this.selectedDesignationIds().length > 0 ||
    this.selectedStatuses().length > 0 ||
    this.search().length > 0
  );

  onDepartmentChange(ids: number[]) {
    this.selectedDepartmentIds.set(ids);
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onBranchChange(ids: number[]) {
    this.selectedBranchIds.set(ids);
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onDesignationChange(ids: number[]) {
    this.selectedDesignationIds.set(ids);
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onStatusChange(statuses: string[]) {
    this.selectedStatuses.set(statuses);
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  clearAllFilters() {
    this.search.set('');
    this.selectedDepartmentIds.set([]);
    this.selectedBranchIds.set([]);
    this.selectedDesignationIds.set([]);
    this.selectedStatuses.set([]);
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadEmployees();
  }

  navigateToDetail(id: string) {
    this.router.navigate(['/employees', id]);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Active': return 'status-active';
      case 'Resigned': return 'status-resigned';
      case 'Suspended': return 'status-suspended';
      case 'Terminated': return 'status-terminated';
      case 'OnNoticePeriod': return 'status-notice';
      default: return '';
    }
  }

  getInitials(firstName: string, lastName: string): string {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  }

  pages = computed(() => {
    const total = this.totalPages();
    return Array.from({ length: total }, (_, i) => i + 1);
  });
}
