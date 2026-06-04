namespace Ergon.DTOs.TaxSlab
{
    public class UpdateTaxSlabRequest
    {
        public decimal MinIncome { get; set; }
        public decimal MaxIncome { get; set; }
        public decimal TaxPercentage { get; set; }
    }
}
