using AutoMapper;
using Ergon.DTOs.State;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class StateService : IStateService
    {
        private readonly IRepository<int, State> _repository;
        private readonly IMapper _mapper;

        public StateService(IRepository<int, State> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StateResponse> GetStateByIdAsync(int id)
        {
            var state = await _repository.Get(id);
            if (state == null) throw new NotFoundException("State not found.");
            return _mapper.Map<StateResponse>(state);
        }

        public async Task<IEnumerable<StateResponse>> GetAllStatesAsync()
        {
            var states = await _repository.GetAll();
            return _mapper.Map<List<StateResponse>>(states);
        }

        public async Task<StateResponse> CreateStateAsync(CreateStateRequest request)
        {
            var state = _mapper.Map<State>(request);
            var createdState = await _repository.Create(state);
            return _mapper.Map<StateResponse>(createdState);
        }

        public async Task<StateResponse> UpdateStateAsync(int id, UpdateStateRequest request)
        {
            var state = await _repository.Get(id);
            if (state == null) throw new NotFoundException("State not found.");
            _mapper.Map(request, state);
            var updatedState = await _repository.Update(id, state);
            return _mapper.Map<StateResponse>(updatedState);
        }

        public async Task<StateResponse> DeleteStateAsync(int id)
        {
            var state = await _repository.Get(id);
            if (state == null) throw new NotFoundException("State not found.");
            var deletedState = await _repository.Delete(id);
            return _mapper.Map<StateResponse>(deletedState);
        }
    }
}
