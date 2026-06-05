using Microsoft.EntityFrameworkCore;
using Ergon.DTOs.Leave;
using Ergon.Interfaces;
using Ergon.Contexts;
using Ergon.Models;

namespace Ergon.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        protected ErgonContext _context;

        public LeaveRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync()
        {
            var entitlements = await _context.Employees
                .Include(e => e.LeaveEntitlement)
                    .ThenInclude(le => le.LeaveEntitlementComponents)
                        .ThenInclude(lec => lec.LeaveType)
                .SelectMany(e => e.LeaveEntitlement.LeaveEntitlementComponents, (e, lec) => new
                {
                    e.EmployeeId,
                    EmployeeName = e.FirstName + " " + e.LastName,
                    lec.LeaveTypeId,
                    lec.LeaveType.LeaveTypeName,
                    lec.TotalDays
                })
                .ToListAsync();

            var usedLeaves = await _context.Leaves
                .Where(l => l.Status == LeaveStatusEnum.Approved)
                .GroupBy(l => new { l.EmployeeId, l.LeaveTypeId })
                .Select(g => new { g.Key.EmployeeId, g.Key.LeaveTypeId, UsedLeaves = g.Count() })
                .ToListAsync();

            var result = entitlements.Select(e => new LeaveBalanceResponse
            {
                EmployeeName = e.EmployeeName,
                LeaveTypeName = e.LeaveTypeName,
                TotalLeaves = e.TotalDays,
                UsedLeaves = usedLeaves.FirstOrDefault(u => u.EmployeeId == e.EmployeeId && u.LeaveTypeId == e.LeaveTypeId)?.UsedLeaves ?? 0,
                RemainingLeaves = e.TotalDays - (usedLeaves.FirstOrDefault(u => u.EmployeeId == e.EmployeeId && u.LeaveTypeId == e.LeaveTypeId)?.UsedLeaves ?? 0)
            }).ToList();

            return result;
        }
    }
}
