using Ergon.DTOs.Auth;

namespace Ergon.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(CreateLoginRequest request);
        Task LogoutAsync(string refreshToken);
        Task<LoginResponse> RefreshAsync(string refreshToken);
        Task ChangePasswordAsync(Guid employeeId, ChangePasswordRequest request);
    }
}
