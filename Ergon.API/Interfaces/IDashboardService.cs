using Ergon.DTOs.Dashboard;

namespace Ergon.Interfaces
{
    public interface IDashboardService
    {
        Task<HRDashboardResponse> GetHRDashboardAsync();
        Task<EmployeeDashboardResponse> GetEmployeeDashboardAsync(Guid employeeId);
        Task<TeamAttendanceSummary?> GetManagerDashboardAsync(Guid managerId);
    }
}
