namespace Ergon.DTOs.Leave
{
    public class GetLeaveBalancesRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? EmployeeId { get; set; }
    }
}
