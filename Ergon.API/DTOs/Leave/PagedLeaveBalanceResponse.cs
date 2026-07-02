namespace Ergon.DTOs.Leave
{
    public class PagedLeaveBalanceResponse
    {
        public IEnumerable<LeaveBalanceResponse> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
