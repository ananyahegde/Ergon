using Asp.Versioning;
using Ergon.DTOs.Designation;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/designations")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            var designation = await _designationService.GetDesignationByIdAsync(id);
            return Ok(designation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDesignations()
        {
            var designations = await _designationService.GetAllDesignationsAsync();
            return Ok(designations);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationRequest request)
        {
            var createdDesignation = await _designationService.CreateDesignationAsync(request);
            return Created("", createdDesignation);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody] UpdateDesignationRequest request)
        {
            var updatedDesignation = await _designationService.UpdateDesignationAsync(id, request);
            return Ok(updatedDesignation);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var deletedDesignation = await _designationService.DeleteDesignationAsync(id);
            return Ok(deletedDesignation);
        }
    }
}
