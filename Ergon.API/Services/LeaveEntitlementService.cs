using AutoMapper;
using Ergon.DTOs.LeaveEntitlement;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class LeaveEntitlementService : ILeaveEntitlementService
    {
        private readonly IRepository<int, LeaveEntitlement> _repository;
        private readonly IMapper _mapper;

        public LeaveEntitlementService(IRepository<int, LeaveEntitlement> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LeaveEntitlementResponse> GetLeaveEntitlementByIdAsync(int id)
        {
            var leaveEntitlement = await _repository.Get(id);
            if (leaveEntitlement == null) throw new NotFoundException("Leave entitlement not found.");
            return _mapper.Map<LeaveEntitlementResponse>(leaveEntitlement);
        }

        public async Task<IEnumerable<LeaveEntitlementResponse>> GetAllLeaveEntitlementsAsync()
        {
            var leaveEntitlements = await _repository.GetAll();
            return _mapper.Map<List<LeaveEntitlementResponse>>(leaveEntitlements);
        }

        public async Task<LeaveEntitlementResponse> CreateLeaveEntitlementAsync(CreateLeaveEntitlementRequest request)
        {
            var leaveEntitlement = _mapper.Map<LeaveEntitlement>(request);
            var createdLeaveEntitlement = await _repository.Create(leaveEntitlement);
            return _mapper.Map<LeaveEntitlementResponse>(createdLeaveEntitlement);
        }

        public async Task<LeaveEntitlementResponse> UpdateLeaveEntitlementAsync(int id, UpdateLeaveEntitlementRequest request)
        {
            var leaveEntitlement = await _repository.Get(id);
            if (leaveEntitlement == null) throw new NotFoundException("Leave entitlement not found.");
            _mapper.Map(request, leaveEntitlement);
            var updatedLeaveEntitlement = await _repository.Update(id, leaveEntitlement);
            return _mapper.Map<LeaveEntitlementResponse>(updatedLeaveEntitlement);
        }

        public async Task<LeaveEntitlementResponse> DeleteLeaveEntitlementAsync(int id)
        {
            var leaveEntitlement = await _repository.Get(id);
            if (leaveEntitlement == null) throw new NotFoundException("Leave entitlement not found.");
            var deletedLeaveEntitlement = await _repository.Delete(id);
            return _mapper.Map<LeaveEntitlementResponse>(deletedLeaveEntitlement);
        }
    }
}
