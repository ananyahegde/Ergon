namespace Ergon.DTOs.Dashboard
{
    public class EmployeeDashboardResponse
    {
        public bool IsClockedIn { get; set; }
        public TimeOnly? ShiftStart { get; set; }
        public TimeOnly? ShiftEnd { get; set; }
        public TimeOnly? ClockInTime { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent { get; set; }
        public int DaysLate { get; set; }
        public List<LeaveBalanceSummary> LeaveBalances { get; set; } = [];
        public List<PendingLeaveSummary> PendingLeaves { get; set; } = [];
        public LatestPayslipSummary? LatestPayslip { get; set; }
        public TeamAttendanceSummary? TeamAttendance { get; set; }
    }
}
