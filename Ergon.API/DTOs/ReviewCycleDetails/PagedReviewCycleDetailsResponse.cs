namespace Ergon.DTOs.ReviewCycleDetails
{
    public class PagedReviewCycleDetailsResponse
    {
        public IEnumerable<ReviewCycleDetailsResponse> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
