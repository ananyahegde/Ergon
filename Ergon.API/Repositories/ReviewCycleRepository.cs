using Ergon.Contexts;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class ReviewCycleRepository : IReviewCycleRepository
    {
        protected ErgonContext _context;

        public ReviewCycleRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetActiveEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.EmploymentStatus == EmploymentStatusEnum.Active)
                .ToListAsync();
        }

        public async Task AddReviewCycleDetailsRangeAsync(List<ReviewCycleDetails> details)
        {
            await _context.ReviewCycleDetails.AddRangeAsync(details);
        }

        public async Task<List<Guid>> GetExistingDetailEmployeeIdsAsync(Guid reviewCycleId)
        {
            return await _context.ReviewCycleDetails
                .Where(r => r.ReviewCycleId == reviewCycleId)
                .Select(r => r.EmployeeId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
