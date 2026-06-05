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
            attendanceTodaySummaryResponse.TotalIncomplete = summary.FirstOrDefault(s => s.Status == AttendanceStatusEnum.Incomplete)?.Count ?? 0;

            return attendanceTodaySummaryResponse;
        }
    }
}
