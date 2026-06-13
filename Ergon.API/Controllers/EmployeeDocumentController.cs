using Asp.Versioning;
using Ergon.DTOs.EmployeeDocument;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Ergon.Exceptions;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees/{employeeId}/documents")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class EmployeeDocumentController : ControllerBase
    {
        private readonly IEmployeeDocumentService _employeeDocumentService;

        public EmployeeDocumentController(IEmployeeDocumentService employeeDocumentService)
        {
            _employeeDocumentService = employeeDocumentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeDocuments(Guid employeeId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only view your own documents.");

            var documents = await _employeeDocumentService.GetAllEmployeeDocumentsAsync(employeeId);
            return Ok(documents);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetEmployeeDocumentById(Guid documentId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var document = await _employeeDocumentService.GetEmployeeDocumentByIdAsync(documentId);

            if (role == "Employee" && loggedInId != document.EmployeeId)
                throw new ForbiddenException("You can only view your own documents.");

            return Ok(document);
        }

        [HttpGet("{documentId}/download")]
        public async Task<IActionResult> DownloadEmployeeDocument(Guid employeeId, Guid documentId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only download your own documents.");

            var (fileBytes, mimeType, fileName) = await _employeeDocumentService.DownloadEmployeeDocumentAsync(documentId);
            return File(fileBytes, mimeType, fileName);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeDocument(Guid employeeId, [FromForm] CreateEmployeeDocumentRequest request)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "HR" || role == "HRAdmin")
                throw new ForbiddenException("HR cannot upload documents on behalf of employees.");

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only upload your own documents.");

            var document = await _employeeDocumentService.CreateEmployeeDocumentAsync(employeeId, request);
            return Created("", document);
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteEmployeeDocument(Guid employeeId, Guid documentId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "HR" || role == "HRAdmin")
                throw new ForbiddenException("HR cannot delete employee documents.");

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only delete your own documents.");

            var document = await _employeeDocumentService.DeleteEmployeeDocumentAsync(documentId);
            return Ok(new { message = "Document deleted.", data = document });
        }
    }
}
