using Microsoft.EntityFrameworkCore;
using Ergon.Interfaces;
using Ergon.Contexts;
using Ergon.Models;

namespace Ergon.Repositories
{
    public class LeaveEntitlementComponentRepository : ILeaveEntitlementComponentRepository
    {
        protected ErgonContext _context;

        public LeaveEntitlementComponentRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveEntitlementComponent>> GetByLeaveEntitlementIdAsync(int leaveEntitlementId)
        {
            return await _context.LeaveEntitlementComponents
                .Include(lec => lec.LeaveType)
                .Where(lec => lec.LeaveEntitlementId == leaveEntitlementId)
                .ToListAsync();
        }
    }
}
