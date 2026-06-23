using Asp.Versioning;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/dashboard")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("hr-summary")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetHRDashboard()
        {
            var result = await _dashboardService.GetHRDashboardAsync();
            return Ok(result);
        }

        [HttpGet("employee-summary")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _dashboardService.GetEmployeeDashboardAsync(employeeId);
            return Ok(result);
        }

        [HttpGet("manager-summary")]
        [Authorize(Policy = "ManagerAndAbove")]
        public async Task<IActionResult> GetManagerDashboard()
        {
            var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _dashboardService.GetManagerDashboardAsync(managerId);
            return Ok(result);
        }
    }
}
