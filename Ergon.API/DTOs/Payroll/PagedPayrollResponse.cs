namespace Ergon.DTOs.Payroll
{
    public class PagedPayrollResponse
    {
        public IEnumerable<PayrollResponse> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
