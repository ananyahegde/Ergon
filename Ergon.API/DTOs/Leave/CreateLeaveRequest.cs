using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Leave
{
    public class CreateLeaveRequest
    {
        [Required]
        public DateOnly FromDate { get; set; }

        [Required]
        public DateOnly ToDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public bool IsHalfDay { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }
    }
}
