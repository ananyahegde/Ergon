using AutoMapper;
using Ergon.DTOs.Department;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<int, Department> _repository;
        private readonly IMapper _mapper;

        public DepartmentService(IRepository<int, Department> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DepartmentResponse> GetDepartmentByIdAsync(int id)
        {
            var department = await _repository.Get(id);
            if (department == null) throw new NotFoundException("Department not found.");
            return _mapper.Map<DepartmentResponse>(department);
        }

        public async Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync()
        {
            var departments = await _repository.GetAll();
            return _mapper.Map<List<DepartmentResponse>>(departments);
        }

        public async Task<DepartmentResponse> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            request.DepartmentName = request.DepartmentName.ToPascalCase();
            var department = _mapper.Map<Department>(request);
            var createdDepartment = await _repository.Create(department);
            return _mapper.Map<DepartmentResponse>(createdDepartment);
        }

        public async Task<DepartmentResponse> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request)
        {
            var department = await _repository.Get(id);
            if (department == null) throw new NotFoundException("Department not found.");
            _mapper.Map(request, department);

            request.DepartmentName = request.DepartmentName.ToPascalCase();
            var updatedDepartment = await _repository.Update(id, department);
            return _mapper.Map<DepartmentResponse>(updatedDepartment);
        }

        public async Task<DepartmentResponse> DeleteDepartmentAsync(int id)
        {
            var department = await _repository.Get(id);
            if (department == null) throw new NotFoundException("Department not found.");
            var deletedDepartment = await _repository.Delete(id);
            return _mapper.Map<DepartmentResponse>(deletedDepartment);
        }
    }
}
