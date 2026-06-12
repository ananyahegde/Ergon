using Asp.Versioning;
using Ergon.DTOs.LeaveEntitlement;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leave-entitlements")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class LeaveEntitlementController : ControllerBase
    {
        private readonly ILeaveEntitlementService _leaveEntitlementService;

        public LeaveEntitlementController(ILeaveEntitlementService leaveEntitlementService)
        {
            _leaveEntitlementService = leaveEntitlementService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveEntitlementById(int id)
        {
            var leaveEntitlement = await _leaveEntitlementService.GetLeaveEntitlementByIdAsync(id);
            return Ok(leaveEntitlement);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaveEntitlements()
        {
            var leaveEntitlements = await _leaveEntitlementService.GetAllLeaveEntitlementsAsync();
            return Ok(leaveEntitlements);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateLeaveEntitlement([FromBody] CreateLeaveEntitlementRequest request)
        {
            var createdLeaveEntitlement = await _leaveEntitlementService.CreateLeaveEntitlementAsync(request);
            return Created("", createdLeaveEntitlement);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateLeaveEntitlement(int id, [FromBody] UpdateLeaveEntitlementRequest request)
        {
            var updatedLeaveEntitlement = await _leaveEntitlementService.UpdateLeaveEntitlementAsync(id, request);
            return Ok(updatedLeaveEntitlement);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteLeaveEntitlement(int id)
        {
            var deletedLeaveEntitlement = await _leaveEntitlementService.DeleteLeaveEntitlementAsync(id);
            return Ok(deletedLeaveEntitlement);
        }
    }
}
