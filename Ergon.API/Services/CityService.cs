using AutoMapper;
using Ergon.DTOs.City;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class CityService : ICityService
    {
        private readonly IRepository<int, City> _repository;
        private readonly IMapper _mapper;

        public CityService(IRepository<int, City> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CityResponse> GetCityByIdAsync(int id)
        {
            var city = await _repository.Get(id);
            if (city == null) throw new NotFoundException("City not found.");
            return _mapper.Map<CityResponse>(city);
        }

        public async Task<IEnumerable<CityResponse>> GetAllCitiesAsync()
        {
            var cities = await _repository.GetAll();
            return _mapper.Map<List<CityResponse>>(cities);
        }

        public async Task<CityResponse> CreateCityAsync(CreateCityRequest request)
        {
            request.CityName = request.CityName.ToPascalCase();
            var city = _mapper.Map<City>(request);
            var createdCity = await _repository.Create(city);
            return _mapper.Map<CityResponse>(createdCity);
        }

        public async Task<CityResponse> UpdateCityAsync(int id, UpdateCityRequest request)
        {
            var city = await _repository.Get(id);
            if (city == null) throw new NotFoundException("City not found.");

            request.CityName = request.CityName.ToPascalCase();
            _mapper.Map(request, city);
            var updatedCity = await _repository.Update(id, city);
            return _mapper.Map<CityResponse>(updatedCity);
        }

        public async Task<CityResponse> DeleteCityAsync(int id)
        {
            var city = await _repository.Get(id);
            if (city == null) throw new NotFoundException("City not found.");
            var deletedCity = await _repository.Delete(id);
            return _mapper.Map<CityResponse>(deletedCity);
        }
    }
}
