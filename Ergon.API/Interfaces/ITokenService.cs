using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Employee employee);
        RefreshToken GenerateRefreshToken(Guid employeeId);
    }
}
