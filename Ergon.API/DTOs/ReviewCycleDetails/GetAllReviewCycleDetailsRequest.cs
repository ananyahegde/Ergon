namespace Ergon.DTOs.ReviewCycleDetails
{
    public class GetAllReviewCycleDetailsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? EmployeeId { get; set; }
    }
}
