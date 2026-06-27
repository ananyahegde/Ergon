using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface ILeaveEntitlementComponentRepository
    {
        Task<IEnumerable<LeaveEntitlementComponent>> GetByLeaveEntitlementIdAsync(int leaveEntitlementId);
    }
}

