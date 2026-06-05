namespace Ergon.Models
{
    public class LeaveEntitlement
    {
        public int LeaveEntitlementId { get; set; }
        public string LeaveEntitlementName { get; set; } = string.Empty;

        public ICollection<LeaveEntitlementComponent> LeaveEntitlementComponents { get; set; } = [];
        public ICollection<Employee> Employees { get; set; } = [];
    }
}
