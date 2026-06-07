using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveEntitlement
{
    public class UpdateLeaveEntitlementRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LeaveEntitlementName { get; set; } = string.Empty;
    }
}
