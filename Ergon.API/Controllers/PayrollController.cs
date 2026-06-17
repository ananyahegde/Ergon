using Asp.Versioning;
using Ergon.DTOs.Payroll;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payrolls")]
    [Authorize]
    [EnableRateLimiting("Fixed")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpGet]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> GetAllPayrolls([FromQuery] GetAllPayrollsRequest request)
        {
            var payrolls = await _payrollService.GetAllPayrollsAsync(request);
            return Ok(payrolls);
        }

        [HttpGet("unapproved")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> GetUnapprovedPayrolls()
        {
            var payrolls = await _payrollService.GetUnapprovedPayrollsAsync();
            return Ok(payrolls);
        }

        [HttpGet("{payrollId:guid}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetPayrollById(Guid payrollId)
        {
            var payroll = await _payrollService.GetPayrollByIdAsync(payrollId);
            return Ok(payroll);
        }

        [HttpPost]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> CreatePayroll([FromBody] CreatePayrollRequest request)
        {
            var payroll = await _payrollService.CreatePayrollAsync(request);
            return Created("", payroll);
        }

        [HttpPost("run")]
        [Authorize(Policy = "HRAndAbove")]
        public async Task<IActionResult> RunPayroll([FromQuery] int month, [FromQuery] int year)
        {
            var createdBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _payrollService.RunPayrollAsync(createdBy, month, year);
            return Ok(new { message = $"Payroll generated for {month}/{year}." });
        }

        [HttpPut("approve")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> BulkApprovePayrolls([FromBody] BulkApprovePayrollRequest request)
        {
            var approvedBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _payrollService.BulkApprovePayrollsAsync(approvedBy, request);
            return Ok(new { message = $"Payrolls for {request.Month}/{request.Year} approved." });
        }

        [HttpDelete("{payrollId:guid}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeletePayroll(Guid payrollId)
        {
            var payroll = await _payrollService.DeletePayrollAsync(payrollId);
            return Ok(new { message = "Payroll deleted.", data = payroll });
        }
    }
}
