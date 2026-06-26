import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../../core/services/employee.service';
import { MasterService } from '../../../core/services/master.service';
import { CreateEmployeeRequest, UpdateEmployeeRequest, EmployeeListItem, GENDERS, EMPLOYMENT_TYPES } from '../../../core/models/employee.model';
import { ToastService } from '../../../core/services/toast.service';
import { SingleSelectDropdown } from '../../../shared/components/single-select-dropdown/single-select-dropdown';
import { EmployeeDetailResponse } from '../../../core/models/employee.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [FormsModule, SingleSelectDropdown, DatePipe],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.css'
})
export class EmployeeForm implements OnInit {
  private route = inject(ActivatedRoute);
  router = inject(Router);
  private employeeService = inject(EmployeeService);
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  isEditMode = signal(false);
  employeeId = signal<string | null>(null);
  isLoading = signal(true);
  isSaving = signal(false);
  submitted = signal(false);

  departments = this.masterService.departments;
  branches = this.masterService.branches;
  designations = this.masterService.designations;
  shifts = this.masterService.shifts;
  roles = this.masterService.roles;
  salaryStructures = this.masterService.salaryStructures;
  leaveEntitlements = this.masterService.leaveEntitlements;
  countries = this.masterService.countries;
  states = this.masterService.states;
  cities = this.masterService.cities;

  allEmployees = signal<EmployeeListItem[]>([]);
  reportsToSearch = signal('');
  showReportsToDropdown = signal(false);
  selectedReportsToName = signal('');
  createdEmployee = signal<EmployeeDetailResponse | null>(null);

  filteredEmployees = computed(() => {
    const search = this.reportsToSearch().toLowerCase();
    if (!search) return this.allEmployees().slice(0, 10);
    return this.allEmployees()
      .filter(e => `${e.firstName} ${e.lastName}`.toLowerCase().includes(search))
      .slice(0, 10);
  });

  genders = GENDERS.map(g => ({ label: g, value: g }));
  employmentTypes = EMPLOYMENT_TYPES.map(t => ({ label: t, value: t }));

  private genderMap: Record<string, number> = {
    'Male': 0,
    'Female': 1,
    'Other': 2
  };

  private employmentTypeMap: Record<string, number> = {
    'Intern': 0,
    'FullTime': 1,
    'PartTime': 2,
    'Contract': 3
  };

  form = {
    firstName: '',
    lastName: '',
    workEmail: '',
    personalEmail: '',
    phone: '',
    dateOfBirth: '',
    gender: '',
    addressLine1: '',
    addressLine2: '',
    cityId: null as number | null,
    stateId: null as number | null,
    countryId: null as number | null,
    dateOfJoining: '',
    employmentType: '',
    roleId: null as number | null,
    departmentId: null as number | null,
    branchId: null as number | null,
    designationId: null as number | null,
    shiftId: null as number | null,
    salaryStructureId: null as number | null,
    leaveEntitlementId: null as number | null,
    reportsTo: null as string | null
  };

  errors: Record<string, string> = {};

  private emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  private phoneRegex = /^[+]?[\d\s\-().]{7,15}$/;

  validate(): boolean {
    const e: Record<string, string> = {};
    const f = this.form;

    if (!f.firstName.trim()) e['firstName'] = 'First name is required.';
    else if (f.firstName.trim().length < 2) e['firstName'] = 'Min 2 characters.';
    else if (f.firstName.trim().length > 50) e['firstName'] = 'Max 50 characters.';

    if (!f.lastName.trim()) e['lastName'] = 'Last name is required.';
    else if (f.lastName.trim().length < 2) e['lastName'] = 'Min 2 characters.';
    else if (f.lastName.trim().length > 50) e['lastName'] = 'Max 50 characters.';

    if (!f.personalEmail.trim()) e['personalEmail'] = 'Personal email is required.';
    else if (!this.emailRegex.test(f.personalEmail)) e['personalEmail'] = 'Enter a valid email address.';

    if (!f.phone.trim()) e['phone'] = 'Phone is required.';
    else if (!this.phoneRegex.test(f.phone)) e['phone'] = 'Enter a valid phone number.';

    if (!f.addressLine1.trim()) e['addressLine1'] = 'Address is required.';
    else if (f.addressLine1.trim().length > 200) e['addressLine1'] = 'Max 200 characters.';

    if (!f.countryId) e['countryId'] = 'Country is required.';
    if (!f.stateId) e['stateId'] = 'State is required.';
    if (!f.departmentId) e['departmentId'] = 'Department is required.';
    if (!f.branchId) e['branchId'] = 'Branch is required.';
    if (!f.designationId) e['designationId'] = 'Designation is required.';
    if (!f.shiftId) e['shiftId'] = 'Shift is required.';
    if (!f.salaryStructureId) e['salaryStructureId'] = 'Salary structure is required.';
    if (!f.leaveEntitlementId) e['leaveEntitlementId'] = 'Leave entitlement is required.';

    if (!this.isEditMode()) {
      if (!f.workEmail.trim()) e['workEmail'] = 'Work email is required.';
      else if (!this.emailRegex.test(f.workEmail)) e['workEmail'] = 'Enter a valid email address.';

      if (!f.dateOfBirth) e['dateOfBirth'] = 'Date of birth is required.';
      if (!f.gender) e['gender'] = 'Gender is required.';
      if (!f.dateOfJoining) e['dateOfJoining'] = 'Date of joining is required.';
      if (!f.employmentType) e['employmentType'] = 'Employment type is required.';
      if (!f.roleId) e['roleId'] = 'Role is required.';
    }

    this.errors = e;
    return Object.keys(e).length === 0;
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.employeeId.set(id);
    }

    this.employeeService.getAll({ pageNumber: 1, pageSize: 1000 }).subscribe({
      next: (res) => {
        this.allEmployees.set(res.items);
        if (this.isEditMode()) {
          this.loadEmployee();
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  private loadEmployee() {
    this.employeeService.getById(this.employeeId()!).subscribe({
      next: (emp) => {
        this.form.firstName = emp.firstName;
        this.form.lastName = emp.lastName;
        this.form.personalEmail = emp.personalEmail;
        this.form.phone = emp.phone;
        this.form.addressLine1 = emp.addressLine1;
        this.form.addressLine2 = emp.addressLine2 ?? '';

        this.form.departmentId = this.masterService.departments().find(d => d.departmentName === emp.departmentName)?.departmentId ?? null;
        this.form.branchId = this.masterService.branches().find(b => b.branchName === emp.branchName)?.branchId ?? null;
        this.form.designationId = this.masterService.designations().find(d => d.designationName === emp.designationName)?.designationId ?? null;
        this.form.shiftId = this.masterService.shifts().find(s => s.shiftName === emp.shiftName)?.shiftId ?? null;
        this.form.salaryStructureId = this.masterService.salaryStructures().find(s => s.salaryStructureName === emp.salaryStructureName)?.salaryStructureId ?? null;
        this.form.countryId = this.masterService.countries().find(c => c.countryName === emp.countryName)?.countryId ?? null;
        this.form.stateId = this.masterService.states().find(s => s.stateName === emp.stateName)?.stateId ?? null;
        this.form.cityId = this.masterService.cities().find(c => c.cityName === emp.cityName)?.cityId ?? null;

        if (emp.managerName) {
          this.selectedReportsToName.set(emp.managerName);
          const manager = this.allEmployees().find(e => `${e.firstName} ${e.lastName}` === emp.managerName);
          if (manager) this.form.reportsTo = manager.employeeId;
        }

        this.isLoading.set(false);
      },
      error: () => this.router.navigate(['/employees'])
    });
  }

  selectReportsTo(emp: EmployeeListItem) {
    this.form.reportsTo = emp.employeeId;
    this.selectedReportsToName.set(`${emp.firstName} ${emp.lastName}`);
    this.reportsToSearch.set('');
    this.showReportsToDropdown.set(false);
  }

  clearReportsTo() {
    this.form.reportsTo = null;
    this.selectedReportsToName.set('');
    this.reportsToSearch.set('');
  }

  getBackendError(err: any): string {
    if (err?.error?.errors) {
      const messages = Object.values(err.error.errors).flat() as string[];
      return messages[0] ?? 'Something went wrong.';
    }
    return err?.error?.message ?? err?.error?.title ?? 'Something went wrong.';
  }

  submit() {
    this.submitted.set(true);
    if (!this.validate()) return;

    this.isSaving.set(true);

    if (this.isEditMode()) {
      const request: UpdateEmployeeRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        personalEmail: this.form.personalEmail,
        phone: this.form.phone,
        addressLine1: this.form.addressLine1,
        addressLine2: this.form.addressLine2 || undefined,
        cityId: this.form.cityId!,
        stateId: this.form.stateId!,
        countryId: this.form.countryId!,
        departmentId: this.form.departmentId!,
        branchId: this.form.branchId!,
        designationId: this.form.designationId!,
        shiftId: this.form.shiftId!,
        salaryStructureId: this.form.salaryStructureId!,
        leaveEntitlementId: this.form.leaveEntitlementId!,
        reportsTo: this.form.reportsTo ?? undefined
      };
      this.employeeService.update(this.employeeId()!, request).subscribe({
        next: () => {
          this.toastService.success('Employee updated successfully.');
          this.router.navigate(['/employees', this.employeeId()]);
        },
        error: (err) => {
          this.toastService.error(this.getBackendError(err));
          this.isSaving.set(false);
        }
      });
    } else {
      const request: CreateEmployeeRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        workEmail: this.form.workEmail,
        personalEmail: this.form.personalEmail,
        phone: this.form.phone,
        dateOfBirth: this.form.dateOfBirth,
        gender: this.genderMap[this.form.gender],
        addressLine1: this.form.addressLine1,
        addressLine2: this.form.addressLine2 || undefined,
        cityId: this.form.cityId ?? undefined,
        stateId: this.form.stateId!,
        countryId: this.form.countryId!,
        dateOfJoining: this.form.dateOfJoining,
        employmentType: this.employmentTypeMap[this.form.employmentType],
        roleId: this.form.roleId!,
        departmentId: this.form.departmentId!,
        branchId: this.form.branchId!,
        designationId: this.form.designationId!,
        shiftId: this.form.shiftId!,
        salaryStructureId: this.form.salaryStructureId!,
        leaveEntitlementId: this.form.leaveEntitlementId!,
        reportsTo: this.form.reportsTo ?? undefined
      };
      this.employeeService.create(request).subscribe({
        next: (emp) => {
          this.createdEmployee.set(emp);
          this.isSaving.set(false);
        },
        error: (err) => {
          this.toastService.error(this.getBackendError(err));
          this.isSaving.set(false);
        }
      });
    }
  }

  cancel() {
    if (this.isEditMode()) {
      this.router.navigate(['/employees', this.employeeId()]);
    } else {
      this.router.navigate(['/employees']);
    }
  }
}