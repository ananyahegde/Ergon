using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycleDetails
{
    public class UpdateReviewCycleDetailsRequest
    {
        [Range(0, 10, ErrorMessage = "Self score must be between 0 and 10.")]
        public decimal SelfScore { get; set; }

        [Range(0, 10, ErrorMessage = "Feedback score must be between 0 and 10.")]
        public decimal FeedbackScore { get; set; }

        [Required(ErrorMessage = "Manager comments are required.")]
        public string ManagerComments { get; set; } = string.Empty;
    }
}
