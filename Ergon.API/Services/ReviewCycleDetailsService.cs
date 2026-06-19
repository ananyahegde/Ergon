using AutoMapper;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class ReviewCycleDetailsService : IReviewCycleDetailsService
    {
        private readonly IReviewCycleDetailsRepository _reviewCycleDetailsRepository;
        private readonly IRepository<Guid, ReviewCycleDetails> _genericRepository;
        private readonly IMapper _mapper;

        public ReviewCycleDetailsService(
            IReviewCycleDetailsRepository reviewCycleDetailsRepository,
            IRepository<Guid, ReviewCycleDetails> genericRepository,
            IMapper mapper)
        {
            _reviewCycleDetailsRepository = reviewCycleDetailsRepository;
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        private static void ValidateScore(decimal score, string fieldName)
        {
            if (score < 1 || score > 10)
                throw new BadRequestException($"{fieldName} must be between 1 and 10.");
        }

        private static void ValidateScoreWindow(ReviewCycle reviewCycle)
        {
            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot update details for a closed review cycle.");

            var today = DateOnly.FromDateTime(DateTime.Now);
            var windowStart = reviewCycle.EndDate.AddDays(-10);

            if (today < windowStart || today > reviewCycle.EndDate)
                throw new BadRequestException("Scores can only be submitted within 10 days of the review cycle's end date.");
        }

        public async Task<PagedReviewCycleDetailsResponse> GetAllReviewCycleDetailsAsync(Guid reviewCycleId, GetAllReviewCycleDetailsRequest request)
        {
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var (items, totalCount) = await _reviewCycleDetailsRepository.GetPagedDetailsAsync(reviewCycleId, request);

            return new PagedReviewCycleDetailsResponse
            {
                Items = _mapper.Map<List<ReviewCycleDetailsResponse>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PagedReviewCycleDetailsResponse> GetMyTeamReviewDetailsAsync(Guid reviewCycleId, Guid managerId, GetAllReviewCycleDetailsRequest request)
        {
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var (items, totalCount) = await _reviewCycleDetailsRepository.GetPagedTeamDetailsAsync(reviewCycleId, managerId, request);

            return new PagedReviewCycleDetailsResponse
            {
                Items = _mapper.Map<List<ReviewCycleDetailsResponse>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<ReviewCycleDetailsResponse> GetReviewCycleDetailsByIdAsync(Guid reviewCycleDetailsId)
        {
            var details = await _reviewCycleDetailsRepository.GetDetailsByIdAsync(reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");
            return _mapper.Map<ReviewCycleDetailsResponse>(details);
        }

        public async Task<ReviewCycleDetailsResponse> CreateReviewCycleDetailsAsync(Guid reviewCycleId, CreateReviewCycleDetailsRequest request)
        {
            var reviewCycle = await _reviewCycleDetailsRepository.GetReviewCycleAsync(reviewCycleId);
            if (reviewCycle == null) throw new NotFoundException("Review cycle not found.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Closed)
                throw new BadRequestException("Cannot add details to a closed review cycle.");

            if (reviewCycle.ReviewCycleStatus == ReviewCycleStatusEnum.Draft)
                throw new BadRequestException("Cannot add details to a draft review cycle.");

            var employee = await _reviewCycleDetailsRepository.GetEmployeeAsync(request.EmployeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            if (employee.EmploymentStatus != EmploymentStatusEnum.Active)
                throw new BadRequestException("Cannot add review details for an inactive employee.");

            var exists = await _reviewCycleDetailsRepository.DetailsExistAsync(reviewCycleId, request.EmployeeId);
            if (exists) throw new ConflictException("Review cycle details already exist for this employee.");

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

            await _reviewCycleDetailsRepository.AddDetailsAsync(details);
            await _reviewCycleDetailsRepository.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(details.ReviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> UpdateSelfScoreAsync(Guid reviewCycleDetailsId, Guid employeeId, UpdateSelfScoreRequest request)
        {
            ValidateScore(request.SelfScore, "Self score");

            var details = await _reviewCycleDetailsRepository.GetDetailsWithCycleAsync(reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");

            if (details.EmployeeId != employeeId)
                throw new ForbiddenException("You can only update your own self score.");

            ValidateScoreWindow(details.ReviewCycle);

            details.SelfScore = request.SelfScore;
            details.UpdatedAt = DateTime.Now;
            await _reviewCycleDetailsRepository.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> UpdateFeedbackAsync(Guid reviewCycleDetailsId, Guid managerId, UpdateFeedbackRequest request)
        {
            ValidateScore(request.FeedbackScore, "Feedback score");

            var details = await _reviewCycleDetailsRepository.GetDetailsWithCycleAndEmployeeAsync(reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");

            ValidateScoreWindow(details.ReviewCycle);

            if (details.Employee.ReportsTo != managerId)
                throw new ForbiddenException("You can only provide feedback for your own subordinates.");

            details.FeedbackScore = request.FeedbackScore;
            details.ManagerComments = request.ManagerComments;
            details.UpdatedAt = DateTime.Now;
            await _reviewCycleDetailsRepository.SaveChangesAsync();
            return await GetReviewCycleDetailsByIdAsync(reviewCycleDetailsId);
        }

        public async Task<ReviewCycleDetailsResponse> DeleteReviewCycleDetailsAsync(Guid reviewCycleDetailsId)
        {
            var details = await _reviewCycleDetailsRepository.GetDetailsByIdAsync(reviewCycleDetailsId);
            if (details == null) throw new NotFoundException("Review cycle details not found.");

            await _genericRepository.Delete(reviewCycleDetailsId);
            return _mapper.Map<ReviewCycleDetailsResponse>(details);
        }
    }
}
