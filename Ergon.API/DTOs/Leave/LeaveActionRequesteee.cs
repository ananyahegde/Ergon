using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.Leave
{
    public class LeaveActionRequest
    {
        [Required]
        public LeaveStatusEnum Action { get; set; }
    }
}
