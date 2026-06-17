using Ergon.Contexts;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ErgonContext _context;

        public AuthRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.WorkEmail == email);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.Employee)
                    .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
