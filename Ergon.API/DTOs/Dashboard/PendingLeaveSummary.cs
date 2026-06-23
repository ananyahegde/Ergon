namespace Ergon.DTOs.Dashboard
{
    public class PendingLeaveSummary
    {
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
