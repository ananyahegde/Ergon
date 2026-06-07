using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Department
{
    public class CreateDepartmentRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string DepartmentName { get; set; } = string.Empty;
    }
}
