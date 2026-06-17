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
        private readonly IMapper _mapper;

        public ReviewCycleService(IRepository<Guid, ReviewCycle> repository, IMapper mapper)
        {
            _repository = repository;
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
            if (request.EndDate < request.StartDate)
                throw new BadRequestException("End date cannot be before start date.");

            var reviewCycle = _mapper.Map<ReviewCycle>(request);
            reviewCycle.ReviewCycleId = Guid.NewGuid();
            reviewCycle.ReviewCycleStatus = ReviewCycleStatusEnum.Active;
            reviewCycle.CreatedAt = DateTime.UtcNow;
            reviewCycle.UpdatedAt = DateTime.UtcNow;

            await _repository.Create(reviewCycle);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }

        public async Task<ReviewCycleResponse> UpdateReviewCycleAsync(Guid reviewCycleId, UpdateReviewCycleRequest request)
        {
            var reviewCycle = await _repository.Get(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot update a closed review cycle.");

            _mapper.Map(request, reviewCycle);
            reviewCycle.UpdatedAt = DateTime.UtcNow;
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

            reviewCycle.ReviewCycleStatus = ReviewCycleStatusEnum.Closed;
            reviewCycle.UpdatedAt = DateTime.UtcNow;
            await _repository.Update(reviewCycleId, reviewCycle);
            return _mapper.Map<ReviewCycleResponse>(reviewCycle);
        }
    }
}
