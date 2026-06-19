using AutoMapper;
using Ergon.DTOs.TaxSlab;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class TaxSlabService : ITaxSlabService
    {
        private readonly IRepository<int, TaxSlab> _repository;
        private readonly IMapper _mapper;

        public TaxSlabService(IRepository<int, TaxSlab> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TaxSlabResponse> GetTaxSlabByIdAsync(int id)
        {
            var taxSlab = await _repository.Get(id);
            if (taxSlab == null) throw new NotFoundException("Tax slab not found.");
            return _mapper.Map<TaxSlabResponse>(taxSlab);
        }

        public async Task<IEnumerable<TaxSlabResponse>> GetAllTaxSlabsAsync()
        {
            var taxSlabs = await _repository.GetAll();
            return _mapper.Map<List<TaxSlabResponse>>(taxSlabs);
        }

        public async Task<TaxSlabResponse> CreateTaxSlabAsync(CreateTaxSlabRequest request)
        {
            if (request.MinIncome >= request.MaxIncome)
                throw new BadRequestException("MinIncome must be less than MaxIncome.");

            var all = await _repository.GetAll();
            if (all.Any(s => request.MinIncome <= s.MaxIncome && request.MaxIncome >= s.MinIncome))
                throw new ConflictException("Tax slab range overlaps with an existing slab.");

            var taxSlab = _mapper.Map<TaxSlab>(request);
            var createdTaxSlab = await _repository.Create(taxSlab);
            return _mapper.Map<TaxSlabResponse>(createdTaxSlab);
        }

        public async Task<TaxSlabResponse> UpdateTaxSlabAsync(int id, UpdateTaxSlabRequest request)
        {
            var taxSlab = await _repository.Get(id);
            if (taxSlab == null) throw new NotFoundException("Tax slab not found.");

            if (request.MinIncome >= request.MaxIncome)
                throw new BadRequestException("MinIncome must be less than MaxIncome.");

            var all = await _repository.GetAll();
            if (all.Any(s => s.TaxSlabId != id && request.MinIncome <= s.MaxIncome && request.MaxIncome >= s.MinIncome))
                throw new ConflictException("Tax slab range overlaps with an existing slab.");

            _mapper.Map(request, taxSlab);
            var updatedTaxSlab = await _repository.Update(id, taxSlab);
            return _mapper.Map<TaxSlabResponse>(updatedTaxSlab);
        }

        public async Task<TaxSlabResponse> DeleteTaxSlabAsync(int id)
        {
            var taxSlab = await _repository.Get(id);
            if (taxSlab == null) throw new NotFoundException("Tax slab not found.");
            var deletedTaxSlab = await _repository.Delete(id);
            return _mapper.Map<TaxSlabResponse>(deletedTaxSlab);
        }
    }
}
