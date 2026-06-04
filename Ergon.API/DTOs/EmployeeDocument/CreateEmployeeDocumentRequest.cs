using Ergon.Models;

namespace Ergon.DTOs.EmployeeDocument
{
    public class CreateEmployeeDocumentRequest
    {
        public string DocumentName { get; set; } = string.Empty;
        public DocumentTypeEnum DocumentType { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
