using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IReviewCycleRepository
    {
        Task<List<Employee>> GetActiveEmployeesAsync();
        Task AddReviewCycleDetailsRangeAsync(List<ReviewCycleDetails> details);
        Task<List<Guid>> GetExistingDetailEmployeeIdsAsync(Guid reviewCycleId);
        Task SaveChangesAsync();
    }
}
