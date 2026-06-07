using Asp.Versioning;
using Ergon.DTOs.Department;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/departments")]
    [EnableRateLimiting("Fixed")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return Ok(department);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            var createdDepartment = await _departmentService.CreateDepartmentAsync(request);
            return Created("", createdDepartment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
        {
            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, request);
            return Ok(updatedDepartment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var deletedDepartment = await _departmentService.DeleteDepartmentAsync(id);
            return Ok(deletedDepartment);
        }
    }
}
