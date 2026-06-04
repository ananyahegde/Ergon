namespace Ergon.DTOs.Attendance
{
    public class CreateAttendanceRequest
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly ClockInTime { get; set; }
    }
}
