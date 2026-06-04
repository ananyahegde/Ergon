namespace Ergon.DTOs.Attendance
{
    public class AttendanceResponse
    {
        public Guid AttendanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly ClockInTime { get; set; }
        public TimeOnly? ClockOutTime { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty;
        public bool LateEntry { get; set; }
        public bool LateExit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
