using Ergon.Models;

namespace Ergon.DTOs.PayrollComponent
{
    public class CreatePayrollComponentRequest
    {
        public string PayrollComponentName { get; set; } = string.Empty;
        public PayrollComponentEnum PayrollComponentType { get; set; }
        public decimal Amount { get; set; }
    }
}
