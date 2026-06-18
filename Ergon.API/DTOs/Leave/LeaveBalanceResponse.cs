namespace Ergon.DTOs.Leave
{
    public class LeaveBalanceResponse
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;
        public decimal TotalLeaves { get; set; }
        public decimal UsedLeaves { get; set; }
        public decimal RemainingLeaves { get; set; }
    }
}
