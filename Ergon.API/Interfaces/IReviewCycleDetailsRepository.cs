using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IReviewCycleDetailsRepository
    {
        Task<ReviewCycle?> GetReviewCycleAsync(Guid reviewCycleId);
        Task<Employee?> GetEmployeeAsync(Guid employeeId);
        Task<bool> DetailsExistAsync(Guid reviewCycleId, Guid employeeId);
        Task<(List<ReviewCycleDetails> Items, int TotalCount)> GetPagedDetailsAsync(Guid reviewCycleId, GetAllReviewCycleDetailsRequest request);
        Task<(List<ReviewCycleDetails> Items, int TotalCount)> GetPagedTeamDetailsAsync(Guid reviewCycleId, Guid managerId, GetAllReviewCycleDetailsRequest request);
        Task<ReviewCycleDetails?> GetDetailsByIdAsync(Guid reviewCycleDetailsId);
        Task<ReviewCycleDetails?> GetDetailsWithCycleAsync(Guid reviewCycleDetailsId);
        Task<ReviewCycleDetails?> GetDetailsWithCycleAndEmployeeAsync(Guid reviewCycleDetailsId);
        Task AddDetailsAsync(ReviewCycleDetails details);
        Task SaveChangesAsync();
    }
}
