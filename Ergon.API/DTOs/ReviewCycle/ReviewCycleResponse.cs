using Ergon.Models;

namespace Ergon.DTOs.ReviewCycle
{
    public class ReviewCycleResponse
    {
        public Guid ReviewCycleId { get; set; }
        public string ReviewName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ReviewCycleStatusEnum ReviewCycleStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
