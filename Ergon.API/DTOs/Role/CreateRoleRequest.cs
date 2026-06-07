using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Role
{
    public class CreateRoleRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;
    }
}
