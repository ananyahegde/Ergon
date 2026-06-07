using Ergon.DTOs.State;

namespace Ergon.Interfaces
{
    public interface IStateService
    {
        Task<StateResponse> GetStateByIdAsync(int id);
        Task<IEnumerable<StateResponse>> GetAllStatesAsync();
        Task<StateResponse> CreateStateAsync(CreateStateRequest request);
        Task<StateResponse> UpdateStateAsync(int id, UpdateStateRequest request);
        Task<StateResponse> DeleteStateAsync(int id);
    }
}
