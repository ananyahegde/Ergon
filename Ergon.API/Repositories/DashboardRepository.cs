using Ergon.Contexts;
using Ergon.DTOs.Dashboard;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ErgonContext _context;

        public DashboardRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalActiveEmployeesAsync()
        {
            return await _context.Employees
                .CountAsync(e => e.EmploymentStatus == EmploymentStatusEnum.Active);
        }

        public async Task<int> GetNewEmployeesThisMonthAsync(int month, int year)
        {
            return await _context.Employees
                .CountAsync(e => e.DateOfJoining.Month == month
                    && e.DateOfJoining.Year == year
                    && e.EmploymentStatus == EmploymentStatusEnum.Active);
        }

        public async Task<int> GetOnLeaveTodayAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Leaves
                .CountAsync(l => l.Status == LeaveStatusEnum.Approved
                    && l.FromDate <= today
                    && l.ToDate >= today);
        }

        public async Task<(PayrollStatusEnum status, DateTime createdAt)?> GetPayrollStatusAsync(int month, int year)
        {
            var payroll = await _context.Payrolls
                .Where(p => p.Month == month && p.Year == year)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
            return payroll != null ? (payroll.PayrollStatus, payroll.CreatedAt) : null;
        }

        public async Task<ReviewCycle?> GetActiveReviewCycleAsync()
        {
            return await _context.ReviewCycles
                .Where(rc => rc.ReviewCycleStatus == ReviewCycleStatusEnum.Active)
                .OrderByDescending(rc => rc.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<TodayAttendanceSnapshot> GetTodayAttendanceSnapshotAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var totalActive = await _context.Employees
                .CountAsync(e => e.EmploymentStatus == EmploymentStatusEnum.Active);

            var todayAttendances = await _context.Attendances
                .Where(a => a.Date == today)
                .ToListAsync();

            var onLeave = await _context.Leaves
                .CountAsync(l => l.Status == LeaveStatusEnum.Approved
                    && l.FromDate <= today
                    && l.ToDate >= today);

            var present = todayAttendances.Count(a =>
                a.AttendanceStatus == AttendanceStatusEnum.Present ||
                a.AttendanceStatus == AttendanceStatusEnum.HalfDay);

            var absent = todayAttendances.Count(a =>
                a.AttendanceStatus == AttendanceStatusEnum.Absent);

            var clockedInOrMarked = todayAttendances.Count;
            var notClockedIn = totalActive - clockedInOrMarked - onLeave;

            return new TodayAttendanceSnapshot
            {
                Present = present,
                Absent = absent,
                NotClockedIn = Math.Max(0, notClockedIn),
                OnLeave = onLeave
            };
        }

        public async Task<List<MonthlyPayrollSummary>> GetPayrollSummaryAsync()
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var cutoff = sixMonthsAgo.Year * 100 + sixMonthsAgo.Month;

            return await _context.Payrolls
                .Where(p => p.PayrollStatus == PayrollStatusEnum.Approved
                    && (p.Year * 100 + p.Month) >= cutoff)
                .GroupBy(p => new { p.Month, p.Year })
                .Select(g => new MonthlyPayrollSummary
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalSalary = g.Sum(p => p.NetSalary)
                })
                .OrderBy(p => p.Year)
                .ThenBy(p => p.Month)
                .ToListAsync();
        }

        public async Task<bool> IsClockedInTodayAsync(Guid employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId && a.Date == today);
        }

        public async Task<TimeOnly?> GetClockInTimeAsync(Guid employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);
            return attendance?.ClockInTime;
        }

        public async Task<(TimeOnly start, TimeOnly end)?> GetShiftTimesAsync(Guid employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            return employee?.Shift != null
                ? (employee.Shift.StartTime, employee.Shift.EndTime)
                : null;
        }

        public async Task<int> GetDaysPresentAsync(Guid employeeId, int month, int year)
        {
            return await _context.Attendances
                .CountAsync(a => a.EmployeeId == employeeId
                    && a.Date.Month == month
                    && a.Date.Year == year
                    && (a.AttendanceStatus == AttendanceStatusEnum.Present
                        || a.AttendanceStatus == AttendanceStatusEnum.HalfDay));
        }

        public async Task<int> GetDaysAbsentAsync(Guid employeeId, int month, int year)
        {
            return await _context.Attendances
                .CountAsync(a => a.EmployeeId == employeeId
                    && a.Date.Month == month
                    && a.Date.Year == year
                    && a.AttendanceStatus == AttendanceStatusEnum.Absent);
        }

        public async Task<int> GetDaysLateAsync(Guid employeeId, int month, int year)
        {
            return await _context.Attendances
                .CountAsync(a => a.EmployeeId == employeeId
                    && a.Date.Month == month
                    && a.Date.Year == year
                    && a.LateEntry);
        }

        public async Task<List<LeaveBalanceSummary>> GetLeaveBalancesAsync(Guid employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.LeaveEntitlement)
                    .ThenInclude(le => le.LeaveEntitlementComponents)
                        .ThenInclude(lec => lec.LeaveType)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee?.LeaveEntitlement == null) return [];

            var usedLeaves = await _context.Leaves
                .Where(l => l.EmployeeId == employeeId && l.Status == LeaveStatusEnum.Approved)
                .Select(l => new { l.LeaveTypeId, l.FromDate, l.ToDate })
                .ToListAsync();

            var usedByType = usedLeaves
                .GroupBy(l => l.LeaveTypeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(l => l.ToDate.DayNumber - l.FromDate.DayNumber + 1)
                );

            return employee.LeaveEntitlement.LeaveEntitlementComponents.Select(lec => new LeaveBalanceSummary
            {
                LeaveTypeName = lec.LeaveType.LeaveTypeName,
                TotalDays = lec.TotalDays,
                UsedDays = usedByType.GetValueOrDefault(lec.LeaveTypeId, 0),
                RemainingDays = lec.TotalDays - usedByType.GetValueOrDefault(lec.LeaveTypeId, 0)
            }).ToList();
        }

        public async Task<List<PendingLeaveSummary>> GetPendingLeavesAsync(Guid employeeId)
        {
            return await _context.Leaves
                .Include(l => l.LeaveType)
                .Where(l => l.EmployeeId == employeeId && l.Status == LeaveStatusEnum.Open)
                .Select(l => new PendingLeaveSummary
                {
                    LeaveTypeName = l.LeaveType.LeaveTypeName,
                    FromDate = l.FromDate,
                    ToDate = l.ToDate,
                    Status = l.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<LatestPayslipSummary?> GetLatestPayslipAsync(Guid employeeId)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.PayrollComponents)
                .Where(p => p.EmployeeId == employeeId && p.PayrollStatus == PayrollStatusEnum.Approved)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .FirstOrDefaultAsync();

            if (payroll == null) return null;

            var gross = payroll.PayrollComponents
                .Where(pc => pc.PayrollComponentType == PayrollComponentEnum.Earning)
                .Sum(pc => pc.Amount);

            var deductions = payroll.PayrollComponents
                .Where(pc => pc.PayrollComponentType == PayrollComponentEnum.Deduction)
                .Sum(pc => pc.Amount);

            return new LatestPayslipSummary
            {
                Month = payroll.Month,
                Year = payroll.Year,
                NetSalary = payroll.NetSalary,
                GrossSalary = gross,
                TotalDeductions = deductions
            };
        }

        public async Task<TeamAttendanceSummary?> GetTeamAttendanceSummaryAsync(Guid managerId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var teamIds = await _context.Employees
                .Where(e => e.ReportsTo == managerId && e.EmploymentStatus == EmploymentStatusEnum.Active)
                .Select(e => e.EmployeeId)
                .ToListAsync();

            if (!teamIds.Any()) return null;

            var todayAttendances = await _context.Attendances
                .Where(a => teamIds.Contains(a.EmployeeId) && a.Date == today)
                .ToListAsync();

            var onLeave = await _context.Leaves
                .CountAsync(l => teamIds.Contains(l.EmployeeId)
                    && l.Status == LeaveStatusEnum.Approved
                    && l.FromDate <= today
                    && l.ToDate >= today);

            return new TeamAttendanceSummary
            {
                TotalTeamMembers = teamIds.Count,
                PresentToday = todayAttendances.Count(a =>
                    a.AttendanceStatus == AttendanceStatusEnum.Present ||
                    a.AttendanceStatus == AttendanceStatusEnum.HalfDay),
                AbsentToday = todayAttendances.Count(a =>
                    a.AttendanceStatus == AttendanceStatusEnum.Absent),
                OnLeaveToday = onLeave
            };
        }
    }
}
