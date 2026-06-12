using Asp.Versioning;
using Ergon.DTOs.SalaryStructure;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/salary-structures")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class SalaryStructureController : ControllerBase
    {
        private readonly ISalaryStructureService _salaryStructureService;

        public SalaryStructureController(ISalaryStructureService salaryStructureService)
        {
            _salaryStructureService = salaryStructureService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalaryStructureById(int id)
        {
            var salaryStructure = await _salaryStructureService.GetSalaryStructureByIdAsync(id);
            return Ok(salaryStructure);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalaryStructures()
        {
            var salaryStructures = await _salaryStructureService.GetAllSalaryStructuresAsync();
            return Ok(salaryStructures);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateSalaryStructure([FromBody] CreateSalaryStructureRequest request)
        {
            var createdSalaryStructure = await _salaryStructureService.CreateSalaryStructureAsync(request);
            return Created("", createdSalaryStructure);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateSalaryStructure(int id, [FromBody] UpdateSalaryStructureRequest request)
        {
            var updatedSalaryStructure = await _salaryStructureService.UpdateSalaryStructureAsync(id, request);
            return Ok(updatedSalaryStructure);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteSalaryStructure(int id)
        {
            var deletedSalaryStructure = await _salaryStructureService.DeleteSalaryStructureAsync(id);
            return Ok(deletedSalaryStructure);
        }
    }
}
