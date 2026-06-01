namespace Ergon.Models
{
    public enum PayrollComponentEnum
    {
        Earning,
        Deduction
    }

    public class PayrollComponent
    {
        public Guid PayrollComponentId { get; set; }
        public string PayrollComponentName { get; set; } = string.Empty;
        public PayrollComponentEnum PayrollComponentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public Guid PayrollId { get; set; }

        public Payroll Payroll { get; set; }
    }
}
