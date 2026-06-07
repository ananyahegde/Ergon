using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.PublicHoliday
{
    public class CreatePublicHolidayRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string PublicHolidayName { get; set; } = string.Empty;

        [Required]
        public DateOnly PublicHolidayDate { get; set; }
    }
}
