using AutoMapper;
using Ergon.DTOs.Role;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<int, Role> _repository;
        private readonly IMapper _mapper;

        public RoleService(IRepository<int, Role> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RoleResponse> GetRoleByIdAsync(int id)
        {
            var role = await _repository.Get(id);
            if (role == null) throw new NotFoundException("Role not found.");
            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await _repository.GetAll();
            return _mapper.Map<List<RoleResponse>>(roles);
        }

        public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request)
        {
            request.RoleName = request.RoleName.ToPascalCase();
            var role = _mapper.Map<Role>(request);
            var createdRole = await _repository.Create(role);
            return _mapper.Map<RoleResponse>(createdRole);
        }

        public async Task<RoleResponse> UpdateRoleAsync(int id, UpdateRoleRequest request)
        {
            var role = await _repository.Get(id);
            if (role == null) throw new NotFoundException("Role not found.");

            request.RoleName = request.RoleName.ToPascalCase();
            _mapper.Map(request, role);
            var updatedRole = await _repository.Update(id, role);
            return _mapper.Map<RoleResponse>(updatedRole);
        }

        public async Task<RoleResponse> DeleteRoleAsync(int id)
        {
            var role = await _repository.Get(id);
            if (role == null) throw new NotFoundException("Role not found.");
            var deletedRole = await _repository.Delete(id);
            return _mapper.Map<RoleResponse>(deletedRole);
        }
    }
}
