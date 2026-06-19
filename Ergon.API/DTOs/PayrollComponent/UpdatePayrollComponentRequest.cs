using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.PayrollComponent
{
    public class UpdatePayrollComponentRequest
    {
        [Required(ErrorMessage = "Payroll component name is required.")]
        public string PayrollComponentName { get; set; } = string.Empty;

        [EnumDataType(typeof(PayrollComponentEnum), ErrorMessage = "Invalid payroll component type.")]
        public PayrollComponentEnum PayrollComponentType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }
    }
}
