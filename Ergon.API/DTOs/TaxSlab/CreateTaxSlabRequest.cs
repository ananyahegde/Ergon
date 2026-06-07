using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.TaxSlab
{
    public class CreateTaxSlabRequest
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal MinIncome { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaxIncome { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal TaxPercentage { get; set; }
    }
}
