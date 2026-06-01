namespace Ergon.Models
{
    public class ReviewCycleDetails
    {
        public Guid ReviewCycleDetailsId { get; set; }
        public decimal SelfScore { get; set; }
        public decimal FeedbackScore { get; set; }
        public string ManagerComments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public Guid EmployeeId { get; set; }
        public Guid ReviewCycleId { get; set; }

        public Employee Employee { get; set; }
        public ReviewCycle ReviewCycle { get; set; }
    }
}
