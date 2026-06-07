using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.LeaveEntitlement
{
    public class CreateLeaveEntitlementRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LeaveEntitlementName { get; set; } = string.Empty;
    }
}
