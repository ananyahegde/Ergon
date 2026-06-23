namespace Ergon.DTOs.Dashboard
{
    public class HRDashboardResponse
    {
        public int TotalEmployees { get; set; }
        public int NewEmployeesThisMonth { get; set; }
        public int OnLeaveToday { get; set; }
        public string PayrollStatus { get; set; } = string.Empty;
        public DateTime? PayrollSubmittedAt { get; set; }
        public string ActiveReviewCycleName { get; set; } = string.Empty;
        public string ActiveReviewCycleEndDate { get; set; } = string.Empty;
        public TodayAttendanceSnapshot? TodayAttendanceSnapshot { get; set; }
        public List<MonthlyPayrollSummary> PayrollSummary { get; set; } = [];
    }
}
