using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycleDetails
{
    public class CreateReviewCycleDetailsRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }
    }
}
