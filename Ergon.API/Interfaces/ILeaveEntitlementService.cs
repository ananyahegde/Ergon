using Ergon.DTOs.LeaveEntitlement;

namespace Ergon.Interfaces
{
    public interface ILeaveEntitlementService
    {
        Task<LeaveEntitlementResponse> GetLeaveEntitlementByIdAsync(int id);
        Task<IEnumerable<LeaveEntitlementResponse>> GetAllLeaveEntitlementsAsync();
        Task<LeaveEntitlementResponse> CreateLeaveEntitlementAsync(CreateLeaveEntitlementRequest request);
        Task<LeaveEntitlementResponse> UpdateLeaveEntitlementAsync(int id, UpdateLeaveEntitlementRequest request);
        Task<LeaveEntitlementResponse> DeleteLeaveEntitlementAsync(int id);
    }
}
