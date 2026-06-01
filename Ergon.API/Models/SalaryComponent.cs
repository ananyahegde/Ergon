namespace Ergon.Models
{
    public enum SalaryComponentEnum
    {
        Earning,
        Deduction
    }

    public class SalaryComponent
    {
        public int SalaryComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public SalaryComponentEnum ComponentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public int SalaryStructureId { get; set; }

        // navigation properties
        public SalaryStructure SalaryStructure { get; set; }
    }
}
