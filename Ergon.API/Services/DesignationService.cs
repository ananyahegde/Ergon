using AutoMapper;
using Ergon.DTOs.Designation;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IRepository<int, Designation> _repository;
        private readonly IMapper _mapper;

        public DesignationService(IRepository<int, Designation> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DesignationResponse> GetDesignationByIdAsync(int id)
        {
            var designation = await _repository.Get(id);
            if (designation == null) throw new NotFoundException("Designation not found.");
            return _mapper.Map<DesignationResponse>(designation);
        }

        public async Task<IEnumerable<DesignationResponse>> GetAllDesignationsAsync()
        {
            var designations = await _repository.GetAll();
            return _mapper.Map<List<DesignationResponse>>(designations);
        }

        public async Task<DesignationResponse> CreateDesignationAsync(CreateDesignationRequest request)
        {
            var designation = _mapper.Map<Designation>(request);
            var createdDesignation = await _repository.Create(designation);
            return _mapper.Map<DesignationResponse>(createdDesignation);
        }

        public async Task<DesignationResponse> UpdateDesignationAsync(int id, UpdateDesignationRequest request)
        {
            var designation = await _repository.Get(id);
            if (designation == null) throw new NotFoundException("Designation not found.");
            _mapper.Map(request, designation);
            var updatedDesignation = await _repository.Update(id, designation);
            return _mapper.Map<DesignationResponse>(updatedDesignation);
        }

        public async Task<DesignationResponse> DeleteDesignationAsync(int id)
        {
            var designation = await _repository.Get(id);
            if (designation == null) throw new NotFoundException("Designation not found.");
            var deletedDesignation = await _repository.Delete(id);
            return _mapper.Map<DesignationResponse>(deletedDesignation);
        }
    }
}
