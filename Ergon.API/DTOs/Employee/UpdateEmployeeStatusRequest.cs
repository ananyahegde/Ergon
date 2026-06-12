using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.Employee
{
    public class UpdateEmployeeStatusRequest
    {
        [Required]
        public EmploymentStatusEnum EmploymentStatus { get; set; }
    }
}
