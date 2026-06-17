using Ergon.DTOs.Auth;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IRepository<Guid, Employee> _employeeRepository;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService, IRepository<Guid, Employee> employeeRepository)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _employeeRepository = employeeRepository;
        }

        public async Task<LoginResponse> LoginAsync(CreateLoginRequest request)
        {
            var employee = await _authRepository.GetEmployeeByEmailAsync(request.WorkEmail);
            if (employee == null)
                throw new NotFoundException("Employee not found.");

            if (employee.EmploymentStatus == EmploymentStatusEnum.Resigned ||
                employee.EmploymentStatus == EmploymentStatusEnum.Terminated ||
                employee.EmploymentStatus == EmploymentStatusEnum.Suspended)
                throw new ForbiddenException("Account is inactive.");

            if (!PasswordHasher.VerifyPassword(request.Password, employee.PasswordHash))
                throw new BadRequestException("Invalid email or password.");

            var accessToken = _tokenService.GenerateAccessToken(employee);
            var refreshToken = _tokenService.GenerateRefreshToken(employee.EmployeeId);

            await _authRepository.AddRefreshTokenAsync(refreshToken);
            await _authRepository.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Role = employee.Role.RoleName
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                throw new NotFoundException("Refresh token not found.");

            token.IsRevoked = true;
            await _authRepository.SaveChangesAsync();
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                throw new NotFoundException("Refresh token not found.");

            if (token.IsRevoked)
                throw new UnauthorizedException("Refresh token has been revoked.");

            if (token.Expiry < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token has expired.");

            var accessToken = _tokenService.GenerateAccessToken(token.Employee);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = token.Token,
                Role = token.Employee.Role.RoleName
            };
        }

        public async Task ChangePasswordAsync(Guid employeeId, ChangePasswordRequest request)
        {
            var employee = await _employeeRepository.Get(employeeId);
            if (!PasswordHasher.VerifyPassword(request.OldPassword, employee.PasswordHash))
                throw new BadRequestException("Invalid password.");

            employee.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            await _employeeRepository.Update(employeeId, employee);
        }
    }
}
