using Asp.Versioning;
using Ergon.DTOs.LeaveType;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leave-types")]
    [EnableRateLimiting("Fixed")]
    [Authorize(Policy = "AllRoles")]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService;

        public LeaveTypeController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveTypeById(int id)
        {
            var leaveType = await _leaveTypeService.GetLeaveTypeByIdAsync(id);
            return Ok(leaveType);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            var leaveTypes = await _leaveTypeService.GetAllLeaveTypesAsync();
            return Ok(leaveTypes);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateLeaveType([FromBody] CreateLeaveTypeRequest request)
        {
            var createdLeaveType = await _leaveTypeService.CreateLeaveTypeAsync(request);
            return Created("", createdLeaveType);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateLeaveType(int id, [FromBody] UpdateLeaveTypeRequest request)
        {
            var updatedLeaveType = await _leaveTypeService.UpdateLeaveTypeAsync(id, request);
            return Ok(updatedLeaveType);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var deletedLeaveType = await _leaveTypeService.DeleteLeaveTypeAsync(id);
            return Ok(deletedLeaveType);
        }
    }
}
