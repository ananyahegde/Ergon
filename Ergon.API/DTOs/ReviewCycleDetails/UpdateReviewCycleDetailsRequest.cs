namespace Ergon.DTOs.ReviewCycleDetails
{
    public class UpdateReviewCycleDetailsRequest
    {
        public decimal SelfScore { get; set; }
        public decimal FeedbackScore { get; set; }
        public string ManagerComments { get; set; } = string.Empty;
    }
}
