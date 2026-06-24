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
