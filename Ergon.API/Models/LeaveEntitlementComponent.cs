namespace Ergon.Models
{
    public class LeaveEntitlementComponent
    {
        public int LeaveEntitlementComponentId { get; set; }
        public decimal TotalDays { get; set; }

        public int LeaveEntitlementId { get; set; }
        public int LeaveTypeId { get; set; }

        public LeaveEntitlement LeaveEntitlement { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
    }
}
