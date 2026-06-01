namespace Ergon.Models
{
    public enum AttendanceStatusEnum
    {
        Present,
        Absent,
        OnLeave,
        HalfDay,
        Incomplete
    }


    public class Attendance
    {
        public Guid AttendanceId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly ClockInTime { get; set; }
        public TimeOnly ClockOutTime { get; set; }
        public AttendanceStatusEnum AttendanceStatus { get; set; }
        public bool LateEntry { get; set; }
        public bool LateExit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public Guid EmployeeId { get; set; }

        // navigation properties
        public Employee Employee { get; set; }
    }
}
