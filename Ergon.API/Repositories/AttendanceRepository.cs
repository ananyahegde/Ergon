// AttendanceRepository.cs
using Microsoft.EntityFrameworkCore;
using Ergon.Interfaces;
using Ergon.Contexts;
using Ergon.DTOs.Attendance;
using Ergon.Models;

namespace Ergon.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        protected ErgonContext _context;

        public AttendanceRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync()
        {
            AttendanceTodaySummaryResponse attendanceTodaySummaryResponse = new AttendanceTodaySummaryResponse();
            var summary = await _context.Attendances
              .Where(a => a.Date == DateOnly.FromDateTime(DateTime.Today))
              .GroupBy(a => a.AttendanceStatus)
              .Select(g => new { Status = g.Key, Count = g.Count() })
              .ToListAsync();

            attendanceTodaySummaryResponse.TotalPresent = summary.FirstOrDefault(s => s.Status == AttendanceStatusEnum.Present)?.Count ?? 0;
            attendanceTodaySummaryResponse.TotalAbsent = summary.FirstOrDefault(s => s.Status == AttendanceStatusEnum.Absent)?.Count ?? 0;
            attendanceTodaySummaryResponse.TotalOnLeave = summary.FirstOrDefault(s => s.Status == AttendanceStatusEnum.OnLeave)?.Count ?? 0;
            attendanceTodaySummaryResponse.TotalHalfDay = summary.FirstOrDefault(s => s.Status == AttendanceStatusEnum.HalfDay)?.Count ?? 0;

            var totalActive = await _context.Employees
                .CountAsync(e => e.EmploymentStatus == EmploymentStatusEnum.Active);
            var totalWithRecord = summary.Sum(s => s.Count);
            attendanceTodaySummaryResponse.TotalIncomplete = totalActive - totalWithRecord;

            return attendanceTodaySummaryResponse;
        }

        public async Task<(List<Attendance> Items, int TotalCount)> GetPagedAttendancesAsync(GetAllAttendancesRequest request)
        {
            var q = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(a => a.EmployeeId == request.EmployeeId.Value);

            if (request.Month.HasValue)
                q = q.Where(a => a.Date.Month == request.Month.Value);

            if (request.Year.HasValue)
                q = q.Where(a => a.Date.Year == request.Year.Value);

            if (request.Status != null && request.Status.Any())
            {
                var statuses = request.Status
                    .Select(s => Enum.TryParse<AttendanceStatusEnum>(s, out var status) ? status : (AttendanceStatusEnum?)null)
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                q = q.Where(a => statuses.Contains(a.AttendanceStatus));
            }

            q = q.OrderByDescending(a => a.Date);

            var totalCount = await q.CountAsync();

            var items = await q
                .Skip((Math.Max(1, request.PageNumber) - 1) * Math.Max(1, request.PageSize))
                .Take(Math.Max(1, request.PageSize))
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(Guid attendanceId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
        }

        public async Task<Employee?> GetEmployeeWithShiftAsync(Guid employeeId)
        {
            return await _context.Employees
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Attendance?> GetAttendanceForDateAsync(Guid employeeId, DateOnly date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);
        }

        public async Task<Attendance?> GetAttendanceWithShiftAsync(Guid attendanceId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Shift)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
        }

        public async Task AddAttendanceAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
