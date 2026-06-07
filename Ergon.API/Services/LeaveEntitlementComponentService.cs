using AutoMapper;
using Ergon.DTOs.LeaveEntitlementComponent;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class LeaveEntitlementComponentService : ILeaveEntitlementComponentService
    {
        private readonly IRepository<int, LeaveEntitlementComponent> _repository;
        private readonly IRepository<int, LeaveEntitlement> _leaveEntitlementRepository;
        private readonly IMapper _mapper;

        public LeaveEntitlementComponentService(IRepository<int, LeaveEntitlementComponent> repository, IRepository<int, LeaveEntitlement> leaveEntitlementRepository, IMapper mapper)
        {
            _repository = repository;
            _leaveEntitlementRepository = leaveEntitlementRepository;
            _mapper = mapper;
        }

        public async Task<LeaveEntitlementComponentResponse> GetLeaveEntitlementComponentByIdAsync(int id)
        {
            var leaveEntitlementComponent = await _repository.Get(id);
            if (leaveEntitlementComponent == null) throw new NotFoundException("Leave entitlement component not found.");
            return _mapper.Map<LeaveEntitlementComponentResponse>(leaveEntitlementComponent);
        }

        public async Task<IEnumerable<LeaveEntitlementComponentResponse>> GetAllLeaveEntitlementComponentsAsync(int leaveEntitlementId)
        {
            var leaveEntitlementComponents = await _repository.GetAll();
            var filtered = leaveEntitlementComponents.Where(lec => lec.LeaveEntitlementId == leaveEntitlementId);
            return _mapper.Map<List<LeaveEntitlementComponentResponse>>(filtered);
        }

        public async Task<LeaveEntitlementComponentResponse> CreateLeaveEntitlementComponentAsync(int leaveEntitlementId, CreateLeaveEntitlementComponentRequest request)
        {
            var leaveEntitlement = await _leaveEntitlementRepository.Get(leaveEntitlementId);
            if (leaveEntitlement == null) throw new NotFoundException("Leave entitlement not found.");
            var leaveEntitlementComponent = _mapper.Map<LeaveEntitlementComponent>(request);
            leaveEntitlementComponent.LeaveEntitlementId = leaveEntitlementId;
            var createdLeaveEntitlementComponent = await _repository.Create(leaveEntitlementComponent);
            return _mapper.Map<LeaveEntitlementComponentResponse>(createdLeaveEntitlementComponent);
        }

        public async Task<LeaveEntitlementComponentResponse> UpdateLeaveEntitlementComponentAsync(int id, UpdateLeaveEntitlementComponentRequest request)
        {
            var leaveEntitlementComponent = await _repository.Get(id);
            if (leaveEntitlementComponent == null) throw new NotFoundException("Leave entitlement component not found.");
            _mapper.Map(request, leaveEntitlementComponent);
            var updatedLeaveEntitlementComponent = await _repository.Update(id, leaveEntitlementComponent);
            return _mapper.Map<LeaveEntitlementComponentResponse>(updatedLeaveEntitlementComponent);
        }

        public async Task<LeaveEntitlementComponentResponse> DeleteLeaveEntitlementComponentAsync(int id)
        {
            var leaveEntitlementComponent = await _repository.Get(id);
            if (leaveEntitlementComponent == null) throw new NotFoundException("Leave entitlement component not found.");
            var deletedLeaveEntitlementComponent = await _repository.Delete(id);
            return _mapper.Map<LeaveEntitlementComponentResponse>(deletedLeaveEntitlementComponent);
        }
    }
}
