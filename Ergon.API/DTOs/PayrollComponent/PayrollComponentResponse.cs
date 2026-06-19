namespace Ergon.DTOs.PayrollComponent
{
    public class PayrollComponentResponse
    {
        public Guid PayrollComponentId { get; set; }
        public string PayrollComponentName { get; set; } = string.Empty;
        public string PayrollComponentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
