using Ergon.DTOs.Designation;

namespace Ergon.Interfaces
{
    public interface IDesignationService
    {
        Task<DesignationResponse> GetDesignationByIdAsync(int id);
        Task<IEnumerable<DesignationResponse>> GetAllDesignationsAsync();
        Task<DesignationResponse> CreateDesignationAsync(CreateDesignationRequest request);
        Task<DesignationResponse> UpdateDesignationAsync(int id, UpdateDesignationRequest request);
        Task<DesignationResponse> DeleteDesignationAsync(int id);
    }
}
