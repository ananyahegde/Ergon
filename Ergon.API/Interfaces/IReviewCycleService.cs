using Ergon.DTOs.ReviewCycle;

namespace Ergon.Interfaces
{
    public interface IReviewCycleService
    {
        Task<PagedReviewCycleResponse> GetAllReviewCyclesAsync(GetAllReviewCyclesRequest request);
        Task<ReviewCycleResponse> GetReviewCycleByIdAsync(Guid reviewCycleId);
        Task<ReviewCycleResponse> CreateReviewCycleAsync(CreateReviewCycleRequest request);
        Task<ReviewCycleResponse> StartReviewCycleAsync(Guid reviewCycleId);
        Task<ReviewCycleResponse> UpdateReviewCycleAsync(Guid reviewCycleId, UpdateReviewCycleRequest request);
        Task<ReviewCycleResponse> DeleteReviewCycleAsync(Guid reviewCycleId);
        Task<ReviewCycleResponse> CloseReviewCycleAsync(Guid reviewCycleId);
    }
}
