using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveType
{
    public class CreateLeaveTypeRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LeaveTypeName { get; set; } = string.Empty;
    }
}
