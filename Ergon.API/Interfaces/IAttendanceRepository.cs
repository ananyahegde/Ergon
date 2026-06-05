using Ergon.DTOs.Attendance;

namespace Ergon.Interfaces
{
    public interface IAttendanceRepository
    {
        public Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync();
    }
}
