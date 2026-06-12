using Asp.Versioning;
using Ergon.DTOs.Shift;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/shifts")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShiftById(int id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            return Ok(shift);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return Ok(shifts);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateShift([FromBody] CreateShiftRequest request)
        {
            var createdShift = await _shiftService.CreateShiftAsync(request);
            return Created("", createdShift);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateShift(int id, [FromBody] UpdateShiftRequest request)
        {
            var updatedShift = await _shiftService.UpdateShiftAsync(id, request);
            return Ok(updatedShift);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var deletedShift = await _shiftService.DeleteShiftAsync(id);
            return Ok(deletedShift);
        }
    }
}
