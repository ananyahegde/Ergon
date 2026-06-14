using Ergon.DTOs.ReviewCycleDetails;

namespace Ergon.Interfaces
{
    public interface IReviewCycleDetailsService
    {
        Task<PagedReviewCycleDetailsResponse> GetAllReviewCycleDetailsAsync(Guid reviewCycleId, GetAllReviewCycleDetailsRequest request);
        Task<ReviewCycleDetailsResponse> GetReviewCycleDetailsByIdAsync(Guid reviewCycleDetailsId);
        Task<PagedReviewCycleDetailsResponse> GetMyTeamReviewDetailsAsync(Guid reviewCycleId, Guid managerId, GetAllReviewCycleDetailsRequest request);
        Task<ReviewCycleDetailsResponse> CreateReviewCycleDetailsAsync(Guid reviewCycleId, CreateReviewCycleDetailsRequest request);
        Task<ReviewCycleDetailsResponse> UpdateSelfScoreAsync(Guid reviewCycleDetailsId, Guid employeeId, UpdateSelfScoreRequest request);
        Task<ReviewCycleDetailsResponse> UpdateFeedbackAsync(Guid reviewCycleDetailsId, Guid managerId, UpdateFeedbackRequest request);
        Task<ReviewCycleDetailsResponse> DeleteReviewCycleDetailsAsync(Guid reviewCycleDetailsId);
    }
}
