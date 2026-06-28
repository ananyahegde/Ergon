export interface AttendanceRecord {
  attendanceId: string;
  employeeId: string;
  employeeName: string;
  date: string;
  clockInTime: string | null;
  clockOutTime: string | null;
  attendanceStatus: string;
  lateEntry: boolean;
  lateExit: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface PagedAttendanceResponse {
  items: AttendanceRecord[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface GetAllAttendancesRequest {
  pageNumber?: number;
  pageSize?: number;
  employeeId?: string;
  month?: number;
  year?: number;
  status?: string[];
}

export interface AttendanceTodaySummary {
  totalPresent: number;
  totalAbsent: number;
  totalOnLeave: number;
  totalHalfDay: number;
  totalIncomplete: number;
}

export interface MyTodayAttendance {
  attendanceId: string;
  clockInTime: string | null;
  clockOutTime: string | null;
  attendanceStatus: string;
  date: string;
}

export interface TodayAttendanceSnapshot {
  present: number;
  absent: number;
  notClockedIn: number;
  onLeave: number;
}

export interface EmployeeDashboardSummary {
  shiftStart: string;
  shiftEnd: string;
  clockInTime: string | null;
  daysPresent: number;
  daysAbsent: number;
  daysLate: number;
}

export const ATTENDANCE_STATUSES = ['Present', 'Absent', 'OnLeave', 'HalfDay', 'Incomplete'];