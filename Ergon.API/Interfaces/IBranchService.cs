using Ergon.DTOs.Branch;

namespace Ergon.Interfaces
{
    public interface IBranchService
    {
        Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request);
        Task<BranchResponse> GetBranchByIdAsync(int id);
        Task<IEnumerable<BranchResponse>> GetAllBranchesAsync();
        Task<BranchResponse> UpdateBranchAsync(int id, UpdateBranchRequest request);
        Task<BranchResponse> DeleteBranchAsync(int id);
    }
}
