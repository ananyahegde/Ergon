namespace Ergon.DTOs.SalaryComponent
{
    public class SalaryComponentResponse
    {
        public int SalaryComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
