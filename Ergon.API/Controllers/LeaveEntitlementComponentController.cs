using Asp.Versioning;
using Ergon.DTOs.LeaveEntitlementComponent;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leave-entitlements/{leaveEntitlementId}/leave-entitlement-components")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class LeaveEntitlementComponentController : ControllerBase
    {
        private readonly ILeaveEntitlementComponentService _leaveEntitlementComponentService;

        public LeaveEntitlementComponentController(ILeaveEntitlementComponentService leaveEntitlementComponentService)
        {
            _leaveEntitlementComponentService = leaveEntitlementComponentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveEntitlementComponentById(int id)
        {
            var leaveEntitlementComponent = await _leaveEntitlementComponentService.GetLeaveEntitlementComponentByIdAsync(id);
            return Ok(leaveEntitlementComponent);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaveEntitlementComponents(int leaveEntitlementId)
        {
            var leaveEntitlementComponents = await _leaveEntitlementComponentService.GetAllLeaveEntitlementComponentsAsync(leaveEntitlementId);
            return Ok(leaveEntitlementComponents);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateLeaveEntitlementComponent(int leaveEntitlementId, [FromBody] CreateLeaveEntitlementComponentRequest request)
        {
            var createdLeaveEntitlementComponent = await _leaveEntitlementComponentService.CreateLeaveEntitlementComponentAsync(leaveEntitlementId, request);
            return Created("", createdLeaveEntitlementComponent);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateLeaveEntitlementComponent(int id, [FromBody] UpdateLeaveEntitlementComponentRequest request)
        {
            var updatedLeaveEntitlementComponent = await _leaveEntitlementComponentService.UpdateLeaveEntitlementComponentAsync(id, request);
            return Ok(updatedLeaveEntitlementComponent);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteLeaveEntitlementComponent(int id)
        {
            var deletedLeaveEntitlementComponent = await _leaveEntitlementComponentService.DeleteLeaveEntitlementComponentAsync(id);
            return Ok(deletedLeaveEntitlementComponent);
        }
    }
}
