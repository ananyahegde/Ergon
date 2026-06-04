namespace Ergon.DTOs.EmployeeDocument
{
    public class EmployeeDocumentResponse
    {
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
