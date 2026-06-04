namespace Ergon.DTOs.ReviewCycleDetails
{
    public class CreateReviewCycleDetailsRequest
    {
        public Guid EmployeeId { get; set; }
        public decimal SelfScore { get; set; }
        public decimal FeedbackScore { get; set; }
        public string ManagerComments { get; set; } = string.Empty;
    }
}
