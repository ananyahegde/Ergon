using Asp.Versioning;
using Ergon.DTOs.ReviewCycle;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/review-cycles")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class ReviewCycleController : ControllerBase
    {
        private readonly IReviewCycleService _reviewCycleService;

        public ReviewCycleController(IReviewCycleService reviewCycleService)
        {
            _reviewCycleService = reviewCycleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewCycles([FromQuery] GetAllReviewCyclesRequest request)
        {
            var reviewCycles = await _reviewCycleService.GetAllReviewCyclesAsync(request);
            return Ok(reviewCycles);
        }

        [HttpGet("{reviewCycleId}")]
        public async Task<IActionResult> GetReviewCycleById(Guid reviewCycleId)
        {
            var reviewCycle = await _reviewCycleService.GetReviewCycleByIdAsync(reviewCycleId);
            return Ok(reviewCycle);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateReviewCycle([FromBody] CreateReviewCycleRequest request)
        {
            var reviewCycle = await _reviewCycleService.CreateReviewCycleAsync(request);
            return Created("", reviewCycle);
        }

        [HttpPut("{reviewCycleId}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateReviewCycle(Guid reviewCycleId, [FromBody] UpdateReviewCycleRequest request)
        {
            var reviewCycle = await _reviewCycleService.UpdateReviewCycleAsync(reviewCycleId, request);
            return Ok(reviewCycle);
        }

        [HttpDelete("{reviewCycleId}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteReviewCycle(Guid reviewCycleId)
        {
            var reviewCycle = await _reviewCycleService.DeleteReviewCycleAsync(reviewCycleId);
            return Ok(new { message = "Review cycle deleted.", data = reviewCycle });
        }

        [HttpPut("{reviewCycleId}/close")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CloseReviewCycle(Guid reviewCycleId)
        {
            var reviewCycle = await _reviewCycleService.CloseReviewCycleAsync(reviewCycleId);
            return Ok(new { message = "Review cycle closed.", data = reviewCycle });
        }
    }
}
