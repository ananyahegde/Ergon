using Asp.Versioning;
using Ergon.DTOs.PublicHoliday;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public-holidays")]
    [EnableRateLimiting("Fixed")]
    public class PublicHolidayController : ControllerBase
    {
        private readonly IPublicHolidayService _publicHolidayService;

        public PublicHolidayController(IPublicHolidayService publicHolidayService)
        {
            _publicHolidayService = publicHolidayService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublicHolidayById(int id)
        {
            var publicHoliday = await _publicHolidayService.GetPublicHolidayByIdAsync(id);
            return Ok(publicHoliday);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPublicHolidays()
        {
            var publicHolidays = await _publicHolidayService.GetAllPublicHolidaysAsync();
            return Ok(publicHolidays);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublicHoliday([FromBody] CreatePublicHolidayRequest request)
        {
            var createdPublicHoliday = await _publicHolidayService.CreatePublicHolidayAsync(request);
            return Created("", createdPublicHoliday);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublicHoliday(int id, [FromBody] UpdatePublicHolidayRequest request)
        {
            var updatedPublicHoliday = await _publicHolidayService.UpdatePublicHolidayAsync(id, request);
            return Ok(updatedPublicHoliday);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicHoliday(int id)
        {
            var deletedPublicHoliday = await _publicHolidayService.DeletePublicHolidayAsync(id);
            return Ok(deletedPublicHoliday);
        }
    }
}
