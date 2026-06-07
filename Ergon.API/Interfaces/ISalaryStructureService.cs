using Ergon.DTOs.SalaryStructure;

namespace Ergon.Interfaces
{
    public interface ISalaryStructureService
    {
        Task<SalaryStructureResponse> GetSalaryStructureByIdAsync(int id);
        Task<IEnumerable<SalaryStructureResponse>> GetAllSalaryStructuresAsync();
        Task<SalaryStructureResponse> CreateSalaryStructureAsync(CreateSalaryStructureRequest request);
        Task<SalaryStructureResponse> UpdateSalaryStructureAsync(int id, UpdateSalaryStructureRequest request);
        Task<SalaryStructureResponse> DeleteSalaryStructureAsync(int id);
    }
}
