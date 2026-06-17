using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IAuthRepository
    {
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
