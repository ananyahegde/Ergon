using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveEntitlementComponent
{
    public class UpdateLeaveEntitlementComponentRequest
    {
        [Required]
        [Range(1, 365)]
        public int TotalDays { get; set; }
    }
}
