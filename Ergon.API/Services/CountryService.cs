using AutoMapper;
using Ergon.DTOs.Country;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<int, Country> _repository;
        private readonly IMapper _mapper;

        public CountryService(IRepository<int, Country> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CountryResponse> GetCountryByIdAsync(int id)
        {
            var country = await _repository.Get(id);
            if (country == null) throw new NotFoundException("Country not found.");
            return _mapper.Map<CountryResponse>(country);
        }

        public async Task<IEnumerable<CountryResponse>> GetAllCountriesAsync()
        {
            var countries = await _repository.GetAll();
            return _mapper.Map<List<CountryResponse>>(countries);
        }

        public async Task<CountryResponse> CreateCountryAsync(CreateCountryRequest request)
        {
            var country = _mapper.Map<Country>(request);
            var createdCountry = await _repository.Create(country);
            return _mapper.Map<CountryResponse>(createdCountry);
        }

        public async Task<CountryResponse> UpdateCountryAsync(int id, UpdateCountryRequest request)
        {
            var country = await _repository.Get(id);
            if (country == null) throw new NotFoundException("Country not found.");
            _mapper.Map(request, country);
            var updatedCountry = await _repository.Update(id, country);
            return _mapper.Map<CountryResponse>(updatedCountry);
        }

        public async Task<CountryResponse> DeleteCountryAsync(int id)
        {
            var country = await _repository.Get(id);
            if (country == null) throw new NotFoundException("Country not found.");
            var deletedCountry = await _repository.Delete(id);
            return _mapper.Map<CountryResponse>(deletedCountry);
        }
    }
}
