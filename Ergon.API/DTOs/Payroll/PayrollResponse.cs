namespace Ergon.DTOs.Payroll
{
    public class PayrollResponse
    {
        public Guid PayrollId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal NetSalary { get; set; }
        public string PayrollStatus { get; set; } = string.Empty;
        public string? ApprovedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
