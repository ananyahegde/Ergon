using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycleDetails
{
    public class GetAllReviewCycleDetailsRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1.")]
        public int PageSize { get; set; } = 10;

        public Guid? EmployeeId { get; set; }
    }
}
