namespace Ergon.DTOs.ReviewCycle
{
    public class UpdateReviewCycleRequest
    {
        public string ReviewName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
