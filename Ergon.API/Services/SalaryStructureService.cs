using AutoMapper;
using Ergon.DTOs.SalaryStructure;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class SalaryStructureService : ISalaryStructureService
    {
        private readonly IRepository<int, SalaryStructure> _repository;
        private readonly IMapper _mapper;

        public SalaryStructureService(IRepository<int, SalaryStructure> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SalaryStructureResponse> GetSalaryStructureByIdAsync(int id)
        {
            var salaryStructure = await _repository.Get(id);
            if (salaryStructure == null) throw new NotFoundException("Salary structure not found.");
            return _mapper.Map<SalaryStructureResponse>(salaryStructure);
        }

        public async Task<IEnumerable<SalaryStructureResponse>> GetAllSalaryStructuresAsync()
        {
            var salaryStructures = await _repository.GetAll();
            return _mapper.Map<List<SalaryStructureResponse>>(salaryStructures);
        }

        public async Task<SalaryStructureResponse> CreateSalaryStructureAsync(CreateSalaryStructureRequest request)
        {
            var salaryStructure = _mapper.Map<SalaryStructure>(request);
            var createdSalaryStructure = await _repository.Create(salaryStructure);
            return _mapper.Map<SalaryStructureResponse>(createdSalaryStructure);
        }

        public async Task<SalaryStructureResponse> UpdateSalaryStructureAsync(int id, UpdateSalaryStructureRequest request)
        {
            var salaryStructure = await _repository.Get(id);
            if (salaryStructure == null) throw new NotFoundException("Salary structure not found.");
            _mapper.Map(request, salaryStructure);
            var updatedSalaryStructure = await _repository.Update(id, salaryStructure);
            return _mapper.Map<SalaryStructureResponse>(updatedSalaryStructure);
        }

        public async Task<SalaryStructureResponse> DeleteSalaryStructureAsync(int id)
        {
            var salaryStructure = await _repository.Get(id);
            if (salaryStructure == null) throw new NotFoundException("Salary structure not found.");
            var deletedSalaryStructure = await _repository.Delete(id);
            return _mapper.Map<SalaryStructureResponse>(deletedSalaryStructure);
        }
    }
}
