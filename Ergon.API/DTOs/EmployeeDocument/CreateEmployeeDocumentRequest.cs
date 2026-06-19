using System.ComponentModel.DataAnnotations;
using Ergon.Models;

namespace Ergon.DTOs.EmployeeDocument
{
    public class CreateEmployeeDocumentRequest
    {
        [Required(ErrorMessage = "Document name is required.")]
        public string DocumentName { get; set; } = string.Empty;

        [EnumDataType(typeof(DocumentTypeEnum), ErrorMessage = "Invalid document type.")]
        public DocumentTypeEnum DocumentType { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = null!;
    }
}
