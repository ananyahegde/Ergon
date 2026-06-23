using Ergon.DTOs.Dashboard;
using Ergon.Interfaces;

namespace Ergon.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<HRDashboardResponse> GetHRDashboardAsync()
        {
            var now = DateTime.UtcNow;

            var reviewCycle = await _dashboardRepository.GetActiveReviewCycleAsync();
            var payroll = await _dashboardRepository.GetPayrollStatusAsync(now.Month, now.Year);

            return new HRDashboardResponse
            {
                TotalEmployees = await _dashboardRepository.GetTotalActiveEmployeesAsync(),
                NewEmployeesThisMonth = await _dashboardRepository.GetNewEmployeesThisMonthAsync(now.Month, now.Year),
                OnLeaveToday = await _dashboardRepository.GetOnLeaveTodayAsync(),
                PayrollStatus = payroll?.status.ToString() ?? string.Empty,
                PayrollSubmittedAt = payroll?.createdAt,
                ActiveReviewCycleName = reviewCycle?.ReviewName ?? string.Empty,
                ActiveReviewCycleEndDate = reviewCycle?.EndDate.ToString("MMM dd, yyyy") ?? string.Empty,
                TodayAttendanceSnapshot = await _dashboardRepository.GetTodayAttendanceSnapshotAsync(),
                PayrollSummary = await _dashboardRepository.GetPayrollSummaryAsync()
            };
        }

        public async Task<EmployeeDashboardResponse> GetEmployeeDashboardAsync(Guid employeeId)
        {
            var now = DateTime.UtcNow;
            var shift = await _dashboardRepository.GetShiftTimesAsync(employeeId);

            return new EmployeeDashboardResponse
            {
                IsClockedIn = await _dashboardRepository.IsClockedInTodayAsync(employeeId),
                ClockInTime = await _dashboardRepository.GetClockInTimeAsync(employeeId),
                ShiftStart = shift?.start,
                ShiftEnd = shift?.end,
                DaysPresent = await _dashboardRepository.GetDaysPresentAsync(employeeId, now.Month, now.Year),
                DaysAbsent = await _dashboardRepository.GetDaysAbsentAsync(employeeId, now.Month, now.Year),
                DaysLate = await _dashboardRepository.GetDaysLateAsync(employeeId, now.Month, now.Year),
                LeaveBalances = await _dashboardRepository.GetLeaveBalancesAsync(employeeId),
                PendingLeaves = await _dashboardRepository.GetPendingLeavesAsync(employeeId),
                LatestPayslip = await _dashboardRepository.GetLatestPayslipAsync(employeeId)
            };
        }

        public async Task<TeamAttendanceSummary?> GetManagerDashboardAsync(Guid managerId)
        {
            return await _dashboardRepository.GetTeamAttendanceSummaryAsync(managerId);
        }
    }
}
