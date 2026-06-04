using Ergon.Models;

namespace Ergon.DTOs.SalaryComponent
{
    public class CreateSalaryComponentRequest
    {
        public string ComponentName { get; set; } = string.Empty;
        public SalaryComponentEnum ComponentType { get; set; }
        public decimal Amount { get; set; }
    }
}
