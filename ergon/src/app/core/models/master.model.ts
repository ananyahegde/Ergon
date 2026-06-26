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