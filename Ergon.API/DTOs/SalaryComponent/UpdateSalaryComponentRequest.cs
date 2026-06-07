using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.SalaryComponent
{
    public class UpdateSalaryComponentRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ComponentName { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(SalaryComponentEnum))]
        public SalaryComponentEnum ComponentType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
