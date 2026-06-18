using Ergon.Contexts;
using Ergon.DTOs.Leave;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly ErgonContext _context;

        public LeaveRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<(List<Leave> Leaves, int TotalCount)> GetAllAsync(GetAllLeavesRequest request)
        {
            var q = _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.ActionedByEmployee)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(l => l.EmployeeId == request.EmployeeId.Value);

            if (request.LeaveTypeId.HasValue)
                q = q.Where(l => l.LeaveTypeId == request.LeaveTypeId.Value);

            if (request.Month.HasValue)
                q = q.Where(l => l.FromDate.Month == request.Month.Value);

            if (request.Year.HasValue)
                q = q.Where(l => l.FromDate.Year == request.Year.Value);

            if (request.Statuses != null && request.Statuses.Any())
            {
                var statuses = request.Statuses
                    .Select(s => Enum.TryParse<LeaveStatusEnum>(s, out var status) ? status : (LeaveStatusEnum?)null)
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                q = q.Where(l => statuses.Contains(l.Status));
            }

            q = q.OrderByDescending(l => l.AppliedAt);

            var totalCount = await q.CountAsync();

            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var leaves = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (leaves, totalCount);
        }

        public async Task<Leave?> GetByIdAsync(Guid leaveId)
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.ActionedByEmployee)
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly from, DateOnly to, Guid? excludeId = null)
        {
            return await _context.Leaves
                .AnyAsync(l => l.EmployeeId == employeeId
                    && (excludeId == null || l.LeaveId != excludeId)
                    && l.Status != LeaveStatusEnum.Rejected
                    && l.Status != LeaveStatusEnum.Cancelled
                    && !l.IsHalfDay
                    && l.FromDate <= to
                    && l.ToDate >= from);
        }

        public async Task<int> CountHalfDaysOnDateAsync(Guid employeeId, DateOnly date)
        {
            return await _context.Leaves
                .CountAsync(l => l.EmployeeId == employeeId
                    && l.IsHalfDay
                    && l.FromDate == date
                    && l.Status != LeaveStatusEnum.Rejected
                    && l.Status != LeaveStatusEnum.Cancelled);
        }

        public async Task<decimal> GetUsedLeaveDaysAsync(Guid employeeId, int leaveTypeId)
        {
            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId
                    && l.LeaveTypeId == leaveTypeId
                    && l.Status == LeaveStatusEnum.Approved)
                .ToListAsync();

            return leaves.Sum(l => l.IsHalfDay ? 0.5m : 1m);
        }

        public async Task<decimal> GetEntitlementDaysAsync(int leaveEntitlementId, int leaveTypeId)
        {
            var component = await _context.LeaveEntitlementComponents
                .FirstOrDefaultAsync(c => c.LeaveEntitlementId == leaveEntitlementId
                    && c.LeaveTypeId == leaveTypeId);
            return component?.TotalDays ?? 0;
        }

        public async Task<bool> EmployeeExistsAndActiveAsync(Guid employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId
                    && e.EmploymentStatus == EmploymentStatusEnum.Active);
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
                .ToListAsync();

            var result = entitlements.Select(e =>
            {
                var used = usedLeaves
                    .Where(l => l.EmployeeId == e.EmployeeId && l.LeaveTypeId == e.LeaveTypeId)
                    .Sum(l => l.IsHalfDay ? 0.5m : 1m);

                return new LeaveBalanceResponse
                {
                    EmployeeName = e.EmployeeName,
                    LeaveTypeName = e.LeaveTypeName,
                    TotalLeaves = e.TotalDays,
                    UsedLeaves = used,
                    RemainingLeaves = e.TotalDays - used
                };
            }).ToList();

            return result;
        }

        public async Task<Employee?> GetActioningEmployeeAsync(Guid employeeId)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<int?> GetEmployeeLeaveEntitlementIdAsync(Guid employeeId)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            return employee?.LeaveEntitlementId;
        }
    }
}
