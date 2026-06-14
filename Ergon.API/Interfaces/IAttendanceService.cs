using Ergon.DTOs.Attendance;

namespace Ergon.Interfaces
{
    public interface IAttendanceService
    {
        Task<PagedAttendanceResponse> GetAllAttendancesAsync(GetAllAttendancesRequest request);
        Task<AttendanceResponse> GetAttendanceByIdAsync(Guid attendanceId);
        Task<AttendanceResponse> ClockInAsync(Guid employeeId);
        Task<AttendanceResponse> ClockOutAsync(Guid attendanceId, Guid employeeId);
        Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync();
    }
}
