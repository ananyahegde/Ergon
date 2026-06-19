using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.ReviewCycleDetails
{
    public class UpdateSelfScoreRequest
    {
        [Required]
        [Range(1, 10)]
        public decimal SelfScore { get; set; }
    }
}
