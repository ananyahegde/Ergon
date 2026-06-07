using Ergon.DTOs.SalaryComponent;

namespace Ergon.Interfaces
{
    public interface ISalaryComponentService
    {
        Task<SalaryComponentResponse> GetSalaryComponentByIdAsync(int id);
        Task<IEnumerable<SalaryComponentResponse>> GetAllSalaryComponentsAsync(int salaryStructureId);
        Task<SalaryComponentResponse> CreateSalaryComponentAsync(int salaryStructureId, CreateSalaryComponentRequest request);
        Task<SalaryComponentResponse> UpdateSalaryComponentAsync(int id, UpdateSalaryComponentRequest request);
        Task<SalaryComponentResponse> DeleteSalaryComponentAsync(int id);
    }
}
