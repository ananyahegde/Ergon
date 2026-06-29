using Ergon.Contexts;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class ReviewCycleDetailsRepository : IReviewCycleDetailsRepository
    {
        protected ErgonContext _context;

        public ReviewCycleDetailsRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<ReviewCycle?> GetReviewCycleAsync(Guid reviewCycleId)
        {
            return await _context.ReviewCycles.FindAsync(reviewCycleId);
        }

        public async Task<Employee?> GetEmployeeAsync(Guid employeeId)
        {
            return await _context.Employees.FindAsync(employeeId);
        }

        public async Task<bool> DetailsExistAsync(Guid reviewCycleId, Guid employeeId)
        {
            return await _context.ReviewCycleDetails
                .AnyAsync(r => r.ReviewCycleId == reviewCycleId && r.EmployeeId == employeeId);
        }

        public async Task<(List<ReviewCycleDetails> Items, int TotalCount)> GetPagedDetailsAsync(Guid reviewCycleId, GetAllReviewCycleDetailsRequest request)
        {
            var q = _context.ReviewCycleDetails
                .Include(r => r.Employee)
                    .ThenInclude(e => e.Department)
                .Include(r => r.ReviewCycle)
                .Where(r => r.ReviewCycleId == reviewCycleId)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(r => r.EmployeeId == request.EmployeeId.Value);

            var totalCount = await q.CountAsync();

            var items = await q
                .Skip((Math.Max(1, request.PageNumber) - 1) * Math.Max(1, request.PageSize))
                .Take(Math.Max(1, request.PageSize))
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<ReviewCycleDetails> Items, int TotalCount)> GetPagedTeamDetailsAsync(Guid reviewCycleId, Guid managerId, GetAllReviewCycleDetailsRequest request)
        {
            var q = _context.ReviewCycleDetails
                .Include(r => r.Employee)
                    .ThenInclude(e => e.Department)
                .Include(r => r.ReviewCycle)
                .Where(r => r.ReviewCycleId == reviewCycleId && r.Employee.ReportsTo == managerId)
                .AsQueryable();

            var totalCount = await q.CountAsync();

            var items = await q
                .Skip((Math.Max(1, request.PageNumber) - 1) * Math.Max(1, request.PageSize))
                .Take(Math.Max(1, request.PageSize))
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ReviewCycleDetails?> GetDetailsByIdAsync(Guid reviewCycleDetailsId)
        {
            return await _context.ReviewCycleDetails
                .Include(r => r.Employee)
                .Include(r => r.ReviewCycle)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetails?> GetDetailsWithCycleAsync(Guid reviewCycleDetailsId)
        {
            return await _context.ReviewCycleDetails
                .Include(r => r.ReviewCycle)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetails?> GetDetailsWithCycleAndEmployeeAsync(Guid reviewCycleDetailsId)
        {
            return await _context.ReviewCycleDetails
                .Include(r => r.ReviewCycle)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
        }

        public async Task AddDetailsAsync(ReviewCycleDetails details)
        {
            await _context.ReviewCycleDetails.AddAsync(details);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
