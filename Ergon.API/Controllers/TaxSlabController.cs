using Asp.Versioning;
using Ergon.DTOs.TaxSlab;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tax-slabs")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class TaxSlabController : ControllerBase
    {
        private readonly ITaxSlabService _taxSlabService;

        public TaxSlabController(ITaxSlabService taxSlabService)
        {
            _taxSlabService = taxSlabService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxSlabById(int id)
        {
            var taxSlab = await _taxSlabService.GetTaxSlabByIdAsync(id);
            return Ok(taxSlab);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTaxSlabs()
        {
            var taxSlabs = await _taxSlabService.GetAllTaxSlabsAsync();
            return Ok(taxSlabs);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateTaxSlab([FromBody] CreateTaxSlabRequest request)
        {
            var createdTaxSlab = await _taxSlabService.CreateTaxSlabAsync(request);
            return Created("", createdTaxSlab);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateTaxSlab(int id, [FromBody] UpdateTaxSlabRequest request)
        {
            var updatedTaxSlab = await _taxSlabService.UpdateTaxSlabAsync(id, request);
            return Ok(updatedTaxSlab);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteTaxSlab(int id)
        {
            var deletedTaxSlab = await _taxSlabService.DeleteTaxSlabAsync(id);
            return Ok(deletedTaxSlab);
        }
    }
}
