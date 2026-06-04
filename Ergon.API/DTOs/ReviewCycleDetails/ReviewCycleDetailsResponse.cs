namespace Ergon.DTOs.ReviewCycleDetails
{
    public class ReviewCycleDetailsResponse
    {
        public Guid ReviewCycleDetailsId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ReviewCycleName { get; set; } = string.Empty;
        public decimal SelfScore { get; set; }
        public decimal FeedbackScore { get; set; }
        public string ManagerComments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
