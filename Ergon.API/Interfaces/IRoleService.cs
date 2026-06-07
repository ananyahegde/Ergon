using Ergon.DTOs.Role;

namespace Ergon.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync();
        Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request);
        Task<RoleResponse> UpdateRoleAsync(int id, UpdateRoleRequest request);
        Task<RoleResponse> DeleteRoleAsync(int id);
    }
}
