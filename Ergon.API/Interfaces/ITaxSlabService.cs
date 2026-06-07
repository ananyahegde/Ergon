using Ergon.DTOs.TaxSlab;

namespace Ergon.Interfaces
{
    public interface ITaxSlabService
    {
        Task<TaxSlabResponse> GetTaxSlabByIdAsync(int id);
        Task<IEnumerable<TaxSlabResponse>> GetAllTaxSlabsAsync();
        Task<TaxSlabResponse> CreateTaxSlabAsync(CreateTaxSlabRequest request);
        Task<TaxSlabResponse> UpdateTaxSlabAsync(int id, UpdateTaxSlabRequest request);
        Task<TaxSlabResponse> DeleteTaxSlabAsync(int id);
    }
}
