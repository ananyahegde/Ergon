using Ergon.DTOs.Leave;

namespace Ergon.Interfaces
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync();
    }
}
