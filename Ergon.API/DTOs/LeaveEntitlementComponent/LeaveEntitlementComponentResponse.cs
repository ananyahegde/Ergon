namespace Ergon.DTOs.LeaveEntitlementComponent
{
    public class LeaveEntitlementComponentResponse
    {
        public int LeaveEntitlementComponentId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int TotalDays { get; set; }
    }
}
