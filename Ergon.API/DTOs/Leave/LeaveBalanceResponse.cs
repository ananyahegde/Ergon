namespace Ergon.DTOs.Leave
{
    public class LeaveBalanceResponse
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;
        public int TotalLeaves { get; set; }
        public int UsedLeaves { get; set; }
        public int RemainingLeaves { get; set; }
    }
}
