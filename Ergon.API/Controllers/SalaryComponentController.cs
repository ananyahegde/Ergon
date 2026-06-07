using Asp.Versioning;
using Ergon.DTOs.SalaryComponent;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/salary-structures/{salaryStructureId}/salary-components")]
    [EnableRateLimiting("Fixed")]
    public class SalaryComponentController : ControllerBase
    {
        private readonly ISalaryComponentService _salaryComponentService;

        public SalaryComponentController(ISalaryComponentService salaryComponentService)
        {
            _salaryComponentService = salaryComponentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalaryComponentById(int id)
        {
            var salaryComponent = await _salaryComponentService.GetSalaryComponentByIdAsync(id);
            return Ok(salaryComponent);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalaryComponents(int salaryStructureId)
        {
            var salaryComponents = await _salaryComponentService.GetAllSalaryComponentsAsync(salaryStructureId);
            return Ok(salaryComponents);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalaryComponent(int salaryStructureId, [FromBody] CreateSalaryComponentRequest request)
        {
            var createdSalaryComponent = await _salaryComponentService.CreateSalaryComponentAsync(salaryStructureId, request);
            return Created("", createdSalaryComponent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalaryComponent(int id, [FromBody] UpdateSalaryComponentRequest request)
        {
            var updatedSalaryComponent = await _salaryComponentService.UpdateSalaryComponentAsync(id, request);
            return Ok(updatedSalaryComponent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalaryComponent(int id)
        {
            var deletedSalaryComponent = await _salaryComponentService.DeleteSalaryComponentAsync(id);
            return Ok(deletedSalaryComponent);
        }
    }
}
