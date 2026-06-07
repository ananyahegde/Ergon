using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.State
{
    public class UpdateStateRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string StateName { get; set; } = string.Empty;
    }
}
