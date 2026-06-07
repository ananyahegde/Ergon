using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveType
{
    public class UpdateLeaveTypeRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LeaveTypeName { get; set; } = string.Empty;
    }
}
