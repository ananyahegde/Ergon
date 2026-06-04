namespace Ergon.DTOs.Leave
{
    public class LeaveResponse
    {
        public Guid LeaveId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public string? ActionedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
