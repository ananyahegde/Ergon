namespace Ergon.DTOs.Leave
{
    public class GetAllLeavesRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public List<string>? Statuses { get; set; }
    }
}
