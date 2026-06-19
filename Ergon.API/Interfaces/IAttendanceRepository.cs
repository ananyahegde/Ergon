using Ergon.DTOs.Attendance;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync();
        Task<(List<Attendance> Items, int TotalCount)> GetPagedAttendancesAsync(GetAllAttendancesRequest request);
        Task<Attendance?> GetAttendanceByIdAsync(Guid attendanceId);
        Task<Employee?> GetEmployeeWithShiftAsync(Guid employeeId);
        Task<Attendance?> GetAttendanceForDateAsync(Guid employeeId, DateOnly date);
        Task<Attendance?> GetAttendanceWithShiftAsync(Guid attendanceId);
        Task AddAttendanceAsync(Attendance attendance);
        Task SaveChangesAsync();
    }
}
