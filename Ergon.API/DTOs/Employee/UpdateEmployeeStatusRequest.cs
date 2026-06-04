using Ergon.Models;

namespace Ergon.DTOs.Employee
{
    public class UpdateEmployeeStatusRequest
    {
        public EmploymentStatusEnum EmploymentStatus { get; set; }
    }
}
