namespace Ergon.DTOs.Dashboard
{
    public class LatestPayslipSummary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal NetSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
    }
}
