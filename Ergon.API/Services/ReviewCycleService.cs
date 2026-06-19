using AutoMapper;
using Ergon.DTOs.ReviewCycle;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class ReviewCycleService : IReviewCycleService
    {
        private readonly IRepository<Guid, ReviewCycle> _repository;
        private readonly IReviewCycleRepository _reviewCycleRepository;
        private readonly IMapper _mapper;

        public ReviewCycleService(
            IRepository<Guid, ReviewCycle> repository,
            IReviewCycleRepository reviewCycleRepository,
            IMapper mapper)
        {
            _repository = repository;
            _reviewCycleRepository = reviewCycleRepository;
            _mapper = mapper;
        }

        public async Task<PagedReviewCycleResponse> GetAllReviewCyclesAsync(GetAllReviewCyclesRequest request)
        {
            var all = await _repository.GetAll();
            var q = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<ReviewCycleStatusEnum>(request.Status, out var status))
                    q = q.Where(rc => rc.ReviewCycleStatus == status);
            }

            q = q.OrderByDescending(rc => rc.StartDate);

            var totalCount = q.Count();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var reviewCycles = q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedReviewCycleResponse
            {
                Items = _mapper.Map<List<ReviewCycleResponse>>(reviewCycles),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<ReviewCycleResponse> GetReviewCycleByIdAsync(Guid reviewCycleId)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> CreateReviewCycleAsync(CreateReviewCycleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ReviewName))
                throw new BadRequestException("Review name is required.");

            if (request.EndDate < request.StartDate)
                throw new BadRequestException("End date cannot be before start date.");

            if ((request.EndDate.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays < 14)
                throw new BadRequestException("Review cycle must run for at least 14 days.");

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (request.StartDate < today || request.StartDate > today.AddDays(20))
                throw new BadRequestException("Start date must be within the next 20 days.");

            var reviewCycle = _mapper.Map<ReviewCycle>(request);
            reviewCycle.ReviewCycleId = Guid.NewGuid();
            reviewCycle.ReviewCycleStatus = ReviewCycleStatusEnum.Draft;
            reviewCycle.CreatedAt = DateTime.Now;
            reviewCycle.UpdatedAt = DateTime.Now;

            await _repository.Create(reviewCycle);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> StartReviewCycleAsync(Guid reviewCycleId)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus != ReviewCycleStatusEnum.Draft)
                throw new BadRequestException("Only a draft review cycle can be started.");

            var activeEmployees = await _reviewCycleRepository.GetActiveEmployeesAsync();
            var existingDetailEmployeeIds = await _reviewCycleRepository.GetExistingDetailEmployeeIdsAsync(reviewCycleId);

            var detailsToCreate = activeEmployees
                .Where(e => !existingDetailEmployeeIds.Contains(e.EmployeeId))
                .Select(e => new ReviewCycleDetails
                {
                    ReviewCycleDetailsId = Guid.NewGuid(),
                    ReviewCycleId = reviewCycleId,
                    EmployeeId = e.EmployeeId,
                    SelfScore = 0,
                    FeedbackScore = 0,
                    ManagerComments = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                })
                .ToList();

            if (detailsToCreate.Any())
                await _reviewCycleRepository.AddReviewCycleDetailsRangeAsync(detailsToCreate);

            reviewCycle.ReviewCycleStatus = ReviewCycleStatusEnum.Active;
            reviewCycle.UpdatedAt = DateTime.Now;

            await _reviewCycleRepository.SaveChangesAsync();
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> UpdateReviewCycleAsync(Guid reviewCycleId, UpdateReviewCycleRequest request)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot update a closed review cycle.");

            if (string.IsNullOrWhiteSpace(request.ReviewName))
                throw new BadRequestException("Review name is required.");

            if (request.EndDate < request.StartDate)
                throw new BadRequestException("End date cannot be before start date.");

            if ((request.EndDate.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays < 14)
                throw new BadRequestException("Review cycle must run for at least 14 days.");

            _mapper.Map(request, reviewCycle);
            reviewCycle.UpdatedAt = DateTime.Now;
            await _repository.Update(reviewCycleId, reviewCycle);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> DeleteReviewCycleAsync(Guid reviewCycleId)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Active)
                throw new BadRequestException("Cannot delete an active review cycle.");

            await _repository.Delete(reviewCycleId);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> CloseReviewCycleAsync(Guid reviewCycleId)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Review cycle is already closed.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Draft)
                throw new BadRequestException("Cannot close a review cycle that hasn't started.");

            reviewCycle.ReviewCycleStatus = ReviewCycleStatusEnum.Closed;
            reviewCycle.UpdatedAt = DateTime.Now;
            await _repository.Update(reviewCycleId, reviewCycle);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }
    }
}
