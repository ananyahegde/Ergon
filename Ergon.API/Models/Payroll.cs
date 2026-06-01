namespace Ergon.Models
{
    public enum PayrollStatusEnum
    {
        ApprovalPending,
        Approved
    }

    public class Payroll
    {
        public Guid PayrollId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal NetSalary { get; set; }
        public PayrollStatusEnum PayrollStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys 
        public Guid EmployeeId { get; set; }
        public Guid? ApprovedBy { get; set; }

        public Employee Employee { get; set; }
        public Employee? ApprovedByEmployee { get; set; }

        public ICollection<PayrollComponent> PayrollComponents { get; set; }
    }
}
