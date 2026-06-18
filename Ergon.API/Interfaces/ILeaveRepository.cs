using Ergon.DTOs.Leave;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface ILeaveRepository
    {
        Task<(List<Leave> Leaves, int TotalCount)> GetAllAsync(GetAllLeavesRequest request);
        Task<Leave?> GetByIdAsync(Guid leaveId);
        Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly from, DateOnly to, Guid? excludeId = null);
        Task<int> CountHalfDaysOnDateAsync(Guid employeeId, DateOnly date);
        Task<decimal> GetUsedLeaveDaysAsync(Guid employeeId, int leaveTypeId);
        Task<decimal> GetEntitlementDaysAsync(int leaveEntitlementId, int leaveTypeId);
        Task<bool> EmployeeExistsAndActiveAsync(Guid employeeId);
        Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync();
        Task<Employee?> GetActioningEmployeeAsync(Guid employeeId);
        Task<int?> GetEmployeeLeaveEntitlementIdAsync(Guid employeeId);
    }
}
