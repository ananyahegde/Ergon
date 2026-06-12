using Asp.Versioning;
using Ergon.DTOs.Role;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(role);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var createdRole = await _roleService.CreateRoleAsync(request);
            return Created("", createdRole);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            var updatedRole = await _roleService.UpdateRoleAsync(id, request);
            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var deletedRole = await _roleService.DeleteRoleAsync(id);
            return Ok(deletedRole);
        }
    }
}
