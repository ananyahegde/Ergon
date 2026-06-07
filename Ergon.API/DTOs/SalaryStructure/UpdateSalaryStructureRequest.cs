using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.SalaryStructure
{
    public class UpdateSalaryStructureRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string SalaryStructureName { get; set; } = string.Empty;
    }
}
