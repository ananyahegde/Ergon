using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Designation
{
    public class UpdateDesignationRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string DesignationName { get; set; } = string.Empty;
    }
}
