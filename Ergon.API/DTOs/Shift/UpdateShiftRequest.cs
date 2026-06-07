using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Shift
{
    public class UpdateShiftRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ShiftName { get; set; } = string.Empty;

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
