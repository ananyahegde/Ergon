using Asp.Versioning;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/review-cycles/{reviewCycleId}/review-cycle-details")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class ReviewCycleDetailsController : ControllerBase
    {
        private readonly IReviewCycleDetailsService _reviewCycleDetailsService;

        public ReviewCycleDetailsController(IReviewCycleDetailsService reviewCycleDetailsService)
        {
            _reviewCycleDetailsService = reviewCycleDetailsService;
        }

        [HttpGet]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetAllReviewCycleDetails(Guid reviewCycleId, [FromQuery] GetAllReviewCycleDetailsRequest request)
        {
            var details = await _reviewCycleDetailsService.GetAllReviewCycleDetailsAsync(reviewCycleId, request);
            return Ok(details);
        }

        [HttpGet("my-team")]
        [Authorize(Policy = "ManagerAndAbove")]
        public async Task<IActionResult> GetMyTeamReviewDetails(Guid reviewCycleId, [FromQuery] GetAllReviewCycleDetailsRequest request)
        {
            var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var details = await _reviewCycleDetailsService.GetMyTeamReviewDetailsAsync(reviewCycleId, managerId, request);
            return Ok(details);
        }

        [HttpGet("{reviewCycleDetailsId}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetReviewCycleDetailsById(Guid reviewCycleDetailsId)
        {
            var details = await _reviewCycleDetailsService.GetReviewCycleDetailsByIdAsync(reviewCycleDetailsId);
            return Ok(details);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateReviewCycleDetails(Guid reviewCycleId, [FromBody] CreateReviewCycleDetailsRequest request)
        {
            var details = await _reviewCycleDetailsService.CreateReviewCycleDetailsAsync(reviewCycleId, request);
            return Created("", details);
        }

        [HttpPut("{reviewCycleDetailsId}/self-score")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> UpdateSelfScore(Guid reviewCycleDetailsId, [FromBody] UpdateSelfScoreRequest request)
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var details = await _reviewCycleDetailsService.UpdateSelfScoreAsync(reviewCycleDetailsId, employeeId, request);
            return Ok(details);
        }

        [HttpPut("{reviewCycleDetailsId}/feedback")]
        [Authorize(Policy = "ManagerAndAbove")]
        public async Task<IActionResult> UpdateFeedback(Guid reviewCycleDetailsId, [FromBody] UpdateFeedbackRequest request)
        {
            var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var details = await _reviewCycleDetailsService.UpdateFeedbackAsync(reviewCycleDetailsId, managerId, request);
            return Ok(details);
        }

        [HttpDelete("{reviewCycleDetailsId}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteReviewCycleDetails(Guid reviewCycleDetailsId)
        {
            var details = await _reviewCycleDetailsService.DeleteReviewCycleDetailsAsync(reviewCycleDetailsId);
            return Ok(details);
        }
    }
}
