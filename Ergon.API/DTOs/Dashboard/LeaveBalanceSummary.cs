namespace Ergon.DTOs.Dashboard
{
    public class LeaveBalanceSummary
    {
        public string LeaveTypeName { get; set; } = string.Empty;
        public decimal TotalDays { get; set; }
        public decimal UsedDays { get; set; }
        public decimal RemainingDays { get; set; }
    }
}
