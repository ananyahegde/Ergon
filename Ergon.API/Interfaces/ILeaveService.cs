using Ergon.DTOs.Leave;

namespace Ergon.Interfaces
{
    public interface ILeaveService
    {
        Task<PagedLeaveResponse> GetAllLeavesAsync(GetAllLeavesRequest request);
        Task<LeaveResponse> GetLeaveByIdAsync(Guid leaveId);
        Task<PagedLeaveResponse> GetMyTeamLeavesAsync(Guid managerId, GetAllLeavesRequest request);
        Task<LeaveResponse> ApplyLeaveAsync(Guid employeeId, CreateLeaveRequest request);
        Task<LeaveResponse> ActionLeaveAsync(Guid leaveId, Guid actionedBy, LeaveActionRequest request);
        Task<LeaveResponse> CancelLeaveAsync(Guid leaveId, Guid employeeId);
        Task<PagedLeaveBalanceResponse> GetLeaveBalancesAsync(GetLeaveBalancesRequest request);
    }
}
