namespace Ergon.DTOs.Leave
{
    public class CreateLeaveRequest
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; }
        public int LeaveTypeId { get; set; }
    }
}
