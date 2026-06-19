using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.Payroll
{
    public class PayrollActionRequest
    {
        [EnumDataType(typeof(PayrollStatusEnum), ErrorMessage = "Invalid payroll action.")]
        public PayrollStatusEnum Action { get; set; }
    }
}
