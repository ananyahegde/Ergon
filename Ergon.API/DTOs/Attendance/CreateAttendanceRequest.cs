using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Attendance
{
    public class CreateAttendanceRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly ClockInTime { get; set; }
    }
}
