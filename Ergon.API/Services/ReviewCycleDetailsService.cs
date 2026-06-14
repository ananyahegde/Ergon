using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class ReviewCycleDetailsService : IReviewCycleDetailsService
    {
        private readonly ErgonContext _context;
        private readonly IMapper _mapper;

        public ReviewCycleDetailsService(ErgonContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedReviewCycleDetailsResponse> GetAllReviewCycleDetailsAsync(Guid reviewCycleId, GetAllReviewCycleDetailsRequest request)
        {
            var q = _context.ReviewCycleDetails
                .Include(r => r.Employee)
                .Include(r => r.ReviewCycle)
                .Where(r => r.ReviewCycleId == reviewCycleId)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(r => r.EmployeeId == request.EmployeeId.Value);

            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var details = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedReviewCycleDetailsResponse
            {
                Items = _mapper.Map<List<ReviewCycleDetailsResponse>>(details),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<PagedReviewCycleDetailsResponse> GetMyTeamReviewDetailsAsync(Guid reviewCycleId, Guid managerId, GetAllReviewCycleDetailsRequest request)
        {
            var q = _context.ReviewCycleDetails
                .Include(r => r.Employee)
                .Include(r => r.ReviewCycle)
                .Where(r => r.ReviewCycleId == reviewCycleId && r.Employee.ReportsTo == managerId)
                .AsQueryable();

            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var details = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedReviewCycleDetailsResponse
            {
                Items = _mapper.Map<List<ReviewCycleDetailsResponse>>(details),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<ReviewCycleDetailsResponse> GetReviewCycleDetailsByIdAsync(Guid reviewCycleDetailsId)
        {
            var details = await _context.ReviewCycleDetails
                .Include(r => r.Employee)
                .Include(r => r.ReviewCycle)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");
            return _mapper.Map<ReviewCycleDetailsResponse>(details);
        }

        public async Task<ReviewCycleDetailsResponse> CreateReviewCycleDetailsAsync(Guid reviewCycleId, CreateReviewCycleDetailsRequest request)
        {
            var reviewCycle = await _context.ReviewCycles.FindAsync(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot add details to a closed review cycle.");

            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var existing = await _context.ReviewCycleDetails
                .AnyAsync(r => r.ReviewCycleId == reviewCycleId && r.EmployeeId == request.EmployeeId);
            if (existing) throw new ConflictException("Review cycle details already exist for this employee.");

            var details = new ReviewCycleDetails
            {
                ReviewCycleDetailsId = Guid.NewGuid(),
                ReviewCycleId = reviewCycleId,
                EmployeeId = request.EmployeeId,
                SelfScore = 0,
                FeedbackScore = 0,
                ManagerComments = string.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _context.ReviewCycleDetails.AddAsync(details);
            await _context.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(details.ReviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> UpdateSelfScoreAsync(Guid reviewCycleDetailsId, Guid employeeId, UpdateSelfScoreRequest request)
        {
            var details = await _context.ReviewCycleDetails
                .Include(r => r.ReviewCycle)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");

            if (details.EmployeeId != employeeId)
                throw new ForbiddenException("You can only update your own self score.");

            if (details.ReviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot update details for a closed review cycle.");

            details.SelfScore = request.SelfScore;
            details.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> UpdateFeedbackAsync(Guid reviewCycleDetailsId, Guid managerId, UpdateFeedbackRequest request)
        {
            var details = await _context.ReviewCycleDetails
                .Include(r => r.ReviewCycle)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.ReviewCycleDetailsId == reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");

            if (details.ReviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot update details for a closed review cycle.");

            if (details.Employee.ReportsTo != managerId)
                throw new ForbiddenException("You can only provide feedback for your own subordinates.");

            details.FeedbackScore = request.FeedbackScore;
            details.ManagerComments = request.ManagerComments;
            details.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> DeleteReviewCycleDetailsAsync(Guid reviewCycleDetailsId)
        {
            var details = await _context.ReviewCycleDetails.FindAsync(reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");
            _context.ReviewCycleDetails.Remove(details);
            await _context.SaveChangesAsync();
            return _mapper.Map<ReviewCycleDetailsResponse>(details);
        }
    }
}
