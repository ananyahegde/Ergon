namespace Ergon.Models
{
    public enum LeaveStatusEnum
    {
        Open,
        Approved,
        Rejected,
        Cancelled
    }

    public class Leave
    {
        public Guid LeaveId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; }
        public LeaveStatusEnum Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public Guid? ActionedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign key
        public Guid EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }

        // navigation
        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
        public Employee? ActionedByEmployee { get; set; }
    }
}
