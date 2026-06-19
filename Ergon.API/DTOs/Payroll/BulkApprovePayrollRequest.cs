using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Payroll
{
    public class BulkApprovePayrollRequest
    {
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }
    }
}
