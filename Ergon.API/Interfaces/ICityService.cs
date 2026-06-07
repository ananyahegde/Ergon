using Ergon.DTOs.City;

namespace Ergon.Interfaces
{
    public interface ICityService
    {
        Task<CityResponse> GetCityByIdAsync(int id);
        Task<IEnumerable<CityResponse>> GetAllCitiesAsync();
        Task<CityResponse> CreateCityAsync(CreateCityRequest request);
        Task<CityResponse> UpdateCityAsync(int id, UpdateCityRequest request);
        Task<CityResponse> DeleteCityAsync(int id);
    }
}
