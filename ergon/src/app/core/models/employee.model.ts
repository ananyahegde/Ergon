export const GENDERS = ['Male', 'Female', 'Other'];
export const EMPLOYMENT_TYPES = ['Intern', 'FullTime', 'PartTime', 'Contract'];

export type EmploymentStatus = 'Active' | 'OnNoticePeriod' | 'Resigned' | 'Terminated' | 'Suspended';
export const EMPLOYMENT_STATUSES = [
  'Active',
  'OnNoticePeriod',
  'Resigned',
  'Terminated',
  'Suspended'
];

export interface EmployeeListItem {
  employeeId: string;
  firstName: string;
  lastName: string;
  workEmail: string;
  departmentName: string;
  designationName: string;
  branchName: string;
  employmentStatus: string;
  pfp: string | null;
}

export interface PagedEmployeeResponse {
  items: EmployeeListItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface GetAllEmployeesRequest {
  pageNumber?: number;
  pageSize?: number;
  sortDirection?: string;
  search?: string;
  departmentIds?: number[];
  designationIds?: number[];
  branchIds?: number[];
  employmentStatuses?: string[];
}

export interface EmployeeDetailResponse {
  employeeId: string;
  firstName: string;
  lastName: string;
  workEmail: string;
  tempPassword: string | null;
  personalEmail: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  pfp: string | null;
  addressLine1: string;
  addressLine2: string;
  cityName: string;
  stateName: string;
  countryName: string;
  postalCode: string;
  dateOfJoining: string;
  employmentType: string;
  employmentStatus: string;
  roleName: string;
  departmentName: string;
  branchName: string;
  designationName: string;
  shiftName: string;
  salaryStructureName: string;
  managerName: string;
  createdAt: string;
  updatedAt: string;
}

export interface EmployeeDocument {
  documentId: string;
  documentName: string;
  documentType: string;
  filePath: string;
  createdAt: string;
  updatedAt: string;
  employeeId: string;
}


export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  workEmail: string;
  personalEmail: string;
  phone: string;
  dateOfBirth: string;
  gender: number;
  addressLine1: string;
  addressLine2?: string;
  cityId?: number;
  stateId: number;
  countryId: number;
  dateOfJoining: string;
  employmentType: number;
  roleId: number;
  departmentId: number;
  branchId: number;
  designationId: number;
  shiftId: number;
  salaryStructureId: number;
  leaveEntitlementId: number;
  reportsTo?: string;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  personalEmail: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  cityId: number;
  stateId: number;
  countryId: number;
  departmentId: number;
  branchId: number;
  designationId: number;
  shiftId: number;
  salaryStructureId: number;
  leaveEntitlementId: number;
  reportsTo?: string;
}

export interface UpdateEmployeeStatusRequest {
  employmentStatus: string;
}


export interface EmployeeStatsResponse {
  totalEmployees: number;
  byStatus: {
    active: number;
    onNoticePeriod: number;
    resigned: number;
    terminated: number;
    suspended: number;
  };
  byType: {
    fullTime: number;
    intern: number;
    partTime: number;
    contract: number;
  };
}