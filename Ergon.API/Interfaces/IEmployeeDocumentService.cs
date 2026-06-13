using Ergon.DTOs.EmployeeDocument;

namespace Ergon.Interfaces
{
    public interface IEmployeeDocumentService
    {
        Task<IEnumerable<EmployeeDocumentResponse>> GetAllEmployeeDocumentsAsync(Guid employeeId);
        Task<EmployeeDocumentResponse> GetEmployeeDocumentByIdAsync(Guid documentId);
        Task<EmployeeDocumentResponse> CreateEmployeeDocumentAsync(Guid employeeId, CreateEmployeeDocumentRequest request);
        Task<EmployeeDocumentResponse> DeleteEmployeeDocumentAsync(Guid documentId);
        Task<(byte[] fileBytes, string mimeType, string fileName)> DownloadEmployeeDocumentAsync(Guid documentId);
    }
}
