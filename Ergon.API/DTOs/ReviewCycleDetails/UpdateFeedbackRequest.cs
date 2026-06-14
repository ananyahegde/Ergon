namespace Ergon.DTOs.ReviewCycleDetails
{
    public class UpdateFeedbackRequest
    {
        public decimal FeedbackScore { get; set; }
        public string ManagerComments { get; set; } = string.Empty;
    }
}
