namespace Ergon.Models
{
    public class BankAccount
    {
        public Guid BankAccountId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IfscCode { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public Guid EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
