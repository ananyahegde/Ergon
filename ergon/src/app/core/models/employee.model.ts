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

export type EmploymentStatus = 'Active' | 'OnNoticePeriod' | 'Resigned' | 'Terminated' | 'Suspended';

export const EMPLOYMENT_STATUSES: EmploymentStatus[] = [
  'Active',
  'OnNoticePeriod',
  'Resigned',
  'Terminated',
  'Suspended'
];
