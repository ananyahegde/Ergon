using Asp.Versioning;
using Ergon.DTOs.Employee;
using Ergon.Interfaces;
using Ergon.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] GetAllEmployeesRequest request)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(request);
            return Ok(employees);
        }

        [HttpGet("my-team")]
        [Authorize(Policy = "ManagerAndAbove")]
        public async Task<IActionResult> GetMyTeam()
        {
            var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var employees = await _employeeService.GetMyTeamAsync(managerId);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var loggedInEmployeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInEmployeeId != id)
                throw new ForbiddenException("You can only view your own profile.");

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var employee = await _employeeService.CreateEmployeeAsync(request);
            return Created("", employee);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, request);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _employeeService.DeleteEmployeeAsync(id);
            return Ok(employee);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateEmployeeStatus(Guid id, [FromBody] UpdateEmployeeStatusRequest request)
        {
            var employee = await _employeeService.UpdateEmployeeStatusAsync(id, request);
            return Ok(employee);
        }

        [HttpPut("{id}/pfp")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> UpdateEmployeePfp(Guid id, IFormFile pfp)
        {
            var loggedInEmployeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (loggedInEmployeeId != id)
                throw new ForbiddenException("You can only update your own profile picture.");

            var employee = await _employeeService.UpdateEmployeePfpAsync(id, pfp);
            return Ok(employee);
        }

        [HttpGet("{id}/pfp")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetEmployeePfp(Guid id)
        {
            var loggedInEmployeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInEmployeeId != id)
                throw new ForbiddenException("You can only view your own profile picture.");

            var (fileBytes, contentType) = await _employeeService.GetEmployeePfpAsync(id);
            return File(fileBytes, contentType);
        }

        [HttpPut("{id}/profile")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request)
        {
            var loggedInEmployeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (loggedInEmployeeId != id)
                throw new ForbiddenException("You can only update your own profile.");
            var employee = await _employeeService.UpdateProfileAsync(id, request);
            return Ok(employee);
        }

        [HttpGet("stats")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetEmployeeStats()
        {
            var stats = await _employeeService.GetEmployeeStatsAsync();
            return Ok(stats);
        }
    }
}
