namespace Ergon.Models
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;

        public ICollection<Leave> Leaves { get; set; }
    }
}
