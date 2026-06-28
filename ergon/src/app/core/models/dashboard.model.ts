export interface AttendanceSnapshot {
  present: number;
  absent: number;
  notClockedIn: number;
  onLeave: number;
}

export interface PayrollSummaryItem {
  month: number;
  year: number;
  totalSalary: number;
}

export interface HrDashboardSummary {
  totalEmployees: number;
  newEmployeesThisMonth: number;
  onLeaveToday: number;
  payrollStatus: string;
  payrollSubmittedAt: string;
  activeReviewCycleName: string;
  activeReviewCycleEndDate: string;
  todayAttendanceSnapshot: AttendanceSnapshot;
  payrollSummary: PayrollSummaryItem[];
}

export interface LeaveBalance {
  leaveTypeName: string;
  totalDays: number;
  usedDays: number;
  remainingDays: number;
}

export interface PendingLeave {
  leaveTypeName: string;
  fromDate: string;
  toDate: string;
  status: string;
}

export interface LatestPayslip {
  month: number;
  year: number;
  netSalary: number;
  grossSalary: number;
  totalDeductions: number;
}

export interface EmployeeDashboardSummary {
  isClockedIn: boolean;
  shiftStart: string;
  shiftEnd: string;
  clockInTime: string | null;
  daysPresent: number;
  daysAbsent: number;
  daysLate: number;
  leaveBalances: LeaveBalance[];
  pendingLeaves: PendingLeave[];
  latestPayslip: LatestPayslip | null;
  teamAttendance: null;
}