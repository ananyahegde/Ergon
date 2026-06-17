namespace Ergon.DTOs.Payroll
{
    public class ComponentBreakdown
    {
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PayrollResponse
    {
        public Guid PayrollId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AnnualSalary { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal MonthlyTDS { get; set; }
        public decimal NetSalary { get; set; }
        public string PayrollStatus { get; set; } = string.Empty;
        public string? ApprovedByName { get; set; }
        public List<ComponentBreakdown> Breakdown { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
