using Ergon.DTOs.LeaveEntitlementComponent;

namespace Ergon.Interfaces
{
    public interface ILeaveEntitlementComponentService
    {
        Task<LeaveEntitlementComponentResponse> GetLeaveEntitlementComponentByIdAsync(int id);
        Task<IEnumerable<LeaveEntitlementComponentResponse>> GetAllLeaveEntitlementComponentsAsync(int leaveEntitlementId);
        Task<LeaveEntitlementComponentResponse> CreateLeaveEntitlementComponentAsync(int leaveEntitlementId, CreateLeaveEntitlementComponentRequest request);
        Task<LeaveEntitlementComponentResponse> UpdateLeaveEntitlementComponentAsync(int id, UpdateLeaveEntitlementComponentRequest request);
        Task<LeaveEntitlementComponentResponse> DeleteLeaveEntitlementComponentAsync(int id);
    }
}
