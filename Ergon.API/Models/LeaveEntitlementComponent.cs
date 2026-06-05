namespace Ergon.Models
{
    public class LeaveEntitlementComponent
    {
        public int LeaveEntitlementComponentId { get; set; }
        public int TotalDays { get; set; }

        // foreign keys
        public int LeaveEntitlementId { get; set; }
        public int LeaveTypeId { get; set; }

        // navigation
        public LeaveEntitlement LeaveEntitlement { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
    }
}
