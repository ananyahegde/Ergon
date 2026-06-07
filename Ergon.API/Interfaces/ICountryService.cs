using Ergon.DTOs.Country;

namespace Ergon.Interfaces
{
    public interface ICountryService
    {
        Task<CountryResponse> GetCountryByIdAsync(int id);
        Task<IEnumerable<CountryResponse>> GetAllCountriesAsync();
        Task<CountryResponse> CreateCountryAsync(CreateCountryRequest request);
        Task<CountryResponse> UpdateCountryAsync(int id, UpdateCountryRequest request);
        Task<CountryResponse> DeleteCountryAsync(int id);
    }
}
