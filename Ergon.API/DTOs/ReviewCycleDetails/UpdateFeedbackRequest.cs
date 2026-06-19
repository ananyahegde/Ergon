using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycleDetails
{
    public class UpdateFeedbackRequest
    {
        [Required]
        [Range(1, 10)]
        public decimal FeedbackScore { get; set; }

        [MaxLength(2000)]
        public string ManagerComments { get; set; } = string.Empty;
    }
}
