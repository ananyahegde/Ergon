using Asp.Versioning;
using Ergon.DTOs.Attendance;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/attendances")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetAllAttendances([FromQuery] GetAllAttendancesRequest request)
        {
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (role == "Employee" || role == "Manager")
                request.EmployeeId = employeeId;

            var attendances = await _attendanceService.GetAllAttendancesAsync(request);
            return Ok(attendances);
        }

        [HttpGet("today")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetTodaySummary()
        {
            var summary = await _attendanceService.GetTodaySummaryAsync();
            return Ok(summary);
        }

        [HttpPost("clock-in")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> ClockIn()
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var attendance = await _attendanceService.ClockInAsync(employeeId);
            return Created("", attendance);
        }

        [HttpGet("{attendanceId:guid}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetAttendanceById(Guid attendanceId)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(attendanceId);
            return Ok(attendance);
        }


        [HttpPut("{attendanceId}/clock-out")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> ClockOut(Guid attendanceId)
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var attendance = await _attendanceService.ClockOutAsync(attendanceId, employeeId);
            return Ok(attendance);
        }

        [HttpGet("me/today")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetMyTodayAttendance()
        {
            var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var attendance = await _attendanceService.GetMyTodayAttendanceAsync(employeeId);
            if (attendance == null) return Ok(null);
            return Ok(attendance);
        }
    }
}
