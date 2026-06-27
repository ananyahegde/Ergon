export interface Department {
  departmentId: number;
  departmentName: string;
}

export interface Branch {
  branchId: number;
  branchName: string;
}

export interface Designation {
  designationId: number;
  designationName: string;
}

export interface Shift {
  shiftId: number;
  shiftName: string;
  startTime: string;
  endTime: string;
}

export interface Role {
  roleId: number;
  roleName: string;
}

export interface SalaryStructure {
  salaryStructureId: number;
  salaryStructureName: string;
}

export interface LeaveEntitlement {
  leaveEntitlementId: number;
  leaveEntitlementName: string;
}

export interface SalaryComponent {
  salaryComponentId: number;
  componentName: string;
  componentType: string;
  amount: number;
}

export interface LeaveEntitlementComponent {
  leaveEntitlementComponentId: number;
  leaveTypeName: string;
  totalDays: number;
}

export interface Country {
  countryId: number;
  countryName: string;
}

export interface State {
  stateId: number;
  stateName: string;
}

export interface City {
  cityId: number;
  cityName: string;
}

export interface LeaveType {
  leaveTypeId: number;
  leaveTypeName: string;
}

export interface PublicHoliday {
  publicHolidayId: number;
  publicHolidayName: string;
  publicHolidayDate: string;
}

export interface TaxSlab {
  taxSlabId: number;
  minIncome: number;
  maxIncome: number;
  taxPercentage: number;
}