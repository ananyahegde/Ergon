// `TaxSlab` Master Table.

namespace Ergon.Models
{
    public class TaxSlab
    {
        public int TaxSlabId { get; set; }
        public decimal MinIncome { get; set; }
        public decimal MaxIncome { get; set; }
        public decimal TaxPercentage { get; set; } // fraction
    }
}
