namespace Ergon.Models
{
    public enum ReviewCycleStatusEnum
    {
        Active,
        Closed
    }

    public class ReviewCycle
    {
        public Guid ReviewCycleId { get; set; }
        public string ReviewName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ReviewCycleStatusEnum ReviewCycleStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ReviewCycleDetails> ReviewCycleDetails { get; set; }
    }
}
