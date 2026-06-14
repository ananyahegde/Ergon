using Ergon.Models;

namespace Ergon.DTOs.Leave
{
    public class LeaveActionRequest
    {
        public LeaveStatusEnum Action { get; set; } // Approved or Rejected
    }
}
