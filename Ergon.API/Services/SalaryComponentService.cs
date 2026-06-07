using AutoMapper;
using Ergon.DTOs.SalaryComponent;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class SalaryComponentService : ISalaryComponentService
    {
        private readonly IRepository<int, SalaryComponent> _repository;
        private readonly IRepository<int, SalaryStructure> _salaryStructureRepository;
        private readonly IMapper _mapper;

        public SalaryComponentService(IRepository<int, SalaryComponent> repository, IRepository<int, SalaryStructure> salaryStructureRepository, IMapper mapper)
        {
            _repository = repository;
            _salaryStructureRepository = salaryStructureRepository;
            _mapper = mapper;
        }

        public async Task<SalaryComponentResponse> GetSalaryComponentByIdAsync(int id)
        {
            var salaryComponent = await _repository.Get(id);
            if (salaryComponent == null) throw new NotFoundException("Salary component not found.");
            return _mapper.Map<SalaryComponentResponse>(salaryComponent);
        }

        public async Task<IEnumerable<SalaryComponentResponse>> GetAllSalaryComponentsAsync(int salaryStructureId)
        {
            var salaryComponents = await _repository.GetAll();
            var filtered = salaryComponents.Where(sc => sc.SalaryStructureId == salaryStructureId);
            return _mapper.Map<List<SalaryComponentResponse>>(filtered);
        }

        public async Task<SalaryComponentResponse> CreateSalaryComponentAsync(int salaryStructureId, CreateSalaryComponentRequest request)
        {
            var salaryStructure = await _salaryStructureRepository.Get(salaryStructureId);
            if (salaryStructure == null) throw new NotFoundException("Salary structure not found.");
            var salaryComponent = _mapper.Map<SalaryComponent>(request);
            salaryComponent.SalaryStructureId = salaryStructureId;
            var createdSalaryComponent = await _repository.Create(salaryComponent);
            return _mapper.Map<SalaryComponentResponse>(createdSalaryComponent);
        }

        public async Task<SalaryComponentResponse> UpdateSalaryComponentAsync(int id, UpdateSalaryComponentRequest request)
        {
            var salaryComponent = await _repository.Get(id);
            if (salaryComponent == null) throw new NotFoundException("Salary component not found.");
            _mapper.Map(request, salaryComponent);
            var updatedSalaryComponent = await _repository.Update(id, salaryComponent);
            return _mapper.Map<SalaryComponentResponse>(updatedSalaryComponent);
        }

        public async Task<SalaryComponentResponse> DeleteSalaryComponentAsync(int id)
        {
            var salaryComponent = await _repository.Get(id);
            if (salaryComponent == null) throw new NotFoundException("Salary component not found.");
            var deletedSalaryComponent = await _repository.Delete(id);
            return _mapper.Map<SalaryComponentResponse>(deletedSalaryComponent);
        }
    }
}
