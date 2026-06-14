using Asp.Versioning;
using Ergon.DTOs.Leave;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leaves")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetAllLeaves([FromQuery] GetAllLeavesRequest request)
        {
            var leaves = await _leaveService.GetAllLeavesAsync(request);
            return Ok(leaves);
        }

        [HttpGet("balances")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetLeaveBalances()
        {
            var balances = await _leaveService.GetLeaveBalancesAsync();
            return Ok(balances);
        }

        [HttpGet("{leaveId}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetLeaveById(Guid leaveId)
        {
            var leave = await _leaveService.GetLeaveByIdAsync(leaveId);
            return Ok(leave);
        }

        [HttpPost]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> ApplyLeave([FromBody] CreateLeaveRequest request)
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var leave = await _leaveService.ApplyLeaveAsync(employeeId, request);
            return Created("", leave);
        }

        [HttpPut("{leaveId}/action")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> ActionLeave(Guid leaveId, [FromBody] LeaveActionRequest request)
        {
            var actionedBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var leave = await _leaveService.ActionLeaveAsync(leaveId, actionedBy, request);
            return Ok(leave);
        }

        [HttpPut("{leaveId}/cancel")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> CancelLeave(Guid leaveId)
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var leave = await _leaveService.CancelLeaveAsync(leaveId, employeeId);
            return Ok(leave);
        }
    }
}
