using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveEntitlementComponent
{
    public class CreateLeaveEntitlementComponentRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public int LeaveTypeId { get; set; }

        [Required]
        [Range(1, 365)]
        public int TotalDays { get; set; }
    }
}
