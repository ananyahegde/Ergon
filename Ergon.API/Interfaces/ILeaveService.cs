using Ergon.DTOs.Leave;

namespace Ergon.Interfaces
{
    public interface ILeaveService
    {
        Task<PagedLeaveResponse> GetAllLeavesAsync(GetAllLeavesRequest request);
        Task<LeaveResponse> GetLeaveByIdAsync(Guid leaveId);
        Task<LeaveResponse> ApplyLeaveAsync(Guid employeeId, CreateLeaveRequest request);
        Task<LeaveResponse> ActionLeaveAsync(Guid leaveId, Guid actionedBy, LeaveActionRequest request);
        Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync();
        Task<LeaveResponse> CancelLeaveAsync(Guid leaveId, Guid employeeId);
    }
}
