using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycle
{
    public class UpdateReviewCycleRequest
    {
        [Required, MaxLength(200)]
        public string ReviewName { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
