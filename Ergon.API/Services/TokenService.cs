using Ergon.Interfaces;
using Ergon.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ergon.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _duration;
        private readonly string _refreshTokenExpiry;


        public TokenService(IConfiguration configuration)
        {
            _key = configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key is not configured.");
            _issuer = configuration["JWT:Issuer"] ?? "Ergon";
            _duration = configuration["JWT:DurationInMinutes"] ?? "60";
            _refreshTokenExpiry = configuration["JWT:RefreshTokenExpiryInDays"] ?? "7";
        }

        public string GenerateAccessToken(Employee employee)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                new Claim(ClaimTypes.Email, employee.WorkEmail),
                new Claim(ClaimTypes.GivenName, employee.FirstName),
                new Claim(ClaimTypes.Role, employee.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_duration)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(Guid employeeId)
        {
            return new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                EmployeeId = employeeId,
                Expiry = DateTime.Now.AddDays(Convert.ToDouble(_refreshTokenExpiry)),
                IsRevoked = false,
                CreatedAt = DateTime.Now
            };
        }
    }
}
