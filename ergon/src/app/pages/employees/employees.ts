import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../core/services/employee.service';
import { MasterService } from '../../core/services/master.service';
import { EmployeeListItem, GetAllEmployeesRequest, EMPLOYMENT_STATUSES } from '../../core/models/employee.model';
import { Department, Branch, Designation } from '../../core/models/master.model';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './employees.html',
  styleUrl: './employees.css'
})
export class Employees implements OnInit {
  private employeeService = inject(EmployeeService);
  private masterService = inject(MasterService);
  private router = inject(Router);

  employees = signal<EmployeeListItem[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  isLoading = signal(false);

  departments = signal<Department[]>([]);
  branches = signal<Branch[]>([]);
  designations = signal<Designation[]>([]);
  employmentStatuses = EMPLOYMENT_STATUSES;

  search = signal('');
  selectedDepartmentIds = signal<number[]>([]);
  selectedBranchIds = signal<number[]>([]);
  selectedDesignationIds = signal<number[]>([]);
  selectedStatuses = signal<string[]>([]);
  pageNumber = signal(1);
  pageSize = signal(12);

  ngOnInit() {
    this.loadMasterData();
    this.loadEmployees();
  }

  private loadMasterData() {
    this.masterService.getDepartments().subscribe(d => this.departments.set(d));
    this.masterService.getBranches().subscribe(b => this.branches.set(b));
    this.masterService.getDesignations().subscribe(d => this.designations.set(d));
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
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onDepartmentChange(id: number, checked: boolean) {
    const current = this.selectedDepartmentIds();
    this.selectedDepartmentIds.set(checked ? [...current, id] : current.filter(d => d !== id));
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onBranchChange(id: number, checked: boolean) {
    const current = this.selectedBranchIds();
    this.selectedBranchIds.set(checked ? [...current, id] : current.filter(b => b !== id));
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onDesignationChange(id: number, checked: boolean) {
    const current = this.selectedDesignationIds();
    this.selectedDesignationIds.set(checked ? [...current, id] : current.filter(d => d !== id));
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  onStatusChange(status: string, checked: boolean) {
    const current = this.selectedStatuses();
    this.selectedStatuses.set(checked ? [...current, status] : current.filter(s => s !== status));
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
