using Asp.Versioning;
using Ergon.DTOs.Branch;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/branches")]
    [EnableRateLimiting("Fixed")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request)
        {
            var createdBranch = await _branchService.CreateBranchAsync(request);
            return Created("Branch Created", createdBranch);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            return Ok(branch);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return Ok(branches);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchRequest request)
        {
            var updatedBranch = await _branchService.UpdateBranchAsync(id, request);
            return Ok(new { message = "Branch updated", data = updatedBranch });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var deletedBranch = await _branchService.DeleteBranchAsync(id);
            return Ok(new { message = "Branch deleted", data = deletedBranch });
        }
    }
}
