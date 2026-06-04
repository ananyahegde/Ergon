using Ergon.Models;

namespace Ergon.DTOs.Payroll
{
    public class PayrollActionRequest
    {
        public PayrollStatusEnum Action { get; set; } // Approve or Reject
    }
}
