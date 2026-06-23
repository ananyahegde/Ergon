using Ergon.DTOs.Dashboard;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalActiveEmployeesAsync();
        Task<int> GetNewEmployeesThisMonthAsync(int month, int year);
        Task<int> GetOnLeaveTodayAsync();
        Task<(PayrollStatusEnum status, DateTime createdAt)?> GetPayrollStatusAsync(int month, int year);
        Task<ReviewCycle?> GetActiveReviewCycleAsync();
        Task<TodayAttendanceSnapshot> GetTodayAttendanceSnapshotAsync();
        Task<List<MonthlyPayrollSummary>> GetPayrollSummaryAsync();

        Task<bool> IsClockedInTodayAsync(Guid employeeId);
        Task<TimeOnly?> GetClockInTimeAsync(Guid employeeId);
        Task<(TimeOnly start, TimeOnly end)?> GetShiftTimesAsync(Guid employeeId);
        Task<int> GetDaysPresentAsync(Guid employeeId, int month, int year);
        Task<int> GetDaysAbsentAsync(Guid employeeId, int month, int year);
        Task<int> GetDaysLateAsync(Guid employeeId, int month, int year);
        Task<List<LeaveBalanceSummary>> GetLeaveBalancesAsync(Guid employeeId);
        Task<List<PendingLeaveSummary>> GetPendingLeavesAsync(Guid employeeId);
        Task<LatestPayslipSummary?> GetLatestPayslipAsync(Guid employeeId);

        Task<TeamAttendanceSummary?> GetTeamAttendanceSummaryAsync(Guid managerId);
    }
}
