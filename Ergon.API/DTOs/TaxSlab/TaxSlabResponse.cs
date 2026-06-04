namespace Ergon.DTOs.TaxSlab
{
    public class TaxSlabResponse
    {
        public int TaxSlabId { get; set; }
        public decimal MinIncome { get; set; }
        public decimal MaxIncome { get; set; }
        public decimal TaxPercentage { get; set; }
    }
}
