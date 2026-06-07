using Ergon.DTOs.Department;

namespace Ergon.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentResponse> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync();
        Task<DepartmentResponse> CreateDepartmentAsync(CreateDepartmentRequest request);
        Task<DepartmentResponse> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request);
        Task<DepartmentResponse> DeleteDepartmentAsync(int id);
    }
}
