using AutoMapper;
using Ergon.DTOs.LeaveType;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IRepository<int, LeaveType> _repository;
        private readonly IMapper _mapper;

        public LeaveTypeService(IRepository<int, LeaveType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LeaveTypeResponse> GetLeaveTypeByIdAsync(int id)
        {
            var leaveType = await _repository.Get(id);
            if (leaveType == null) throw new NotFoundException("Leave type not found.");
            return _mapper.Map<LeaveTypeResponse>(leaveType);
        }

        public async Task<IEnumerable<LeaveTypeResponse>> GetAllLeaveTypesAsync()
        {
            var leaveTypes = await _repository.GetAll();
            return _mapper.Map<List<LeaveTypeResponse>>(leaveTypes);
        }

        public async Task<LeaveTypeResponse> CreateLeaveTypeAsync(CreateLeaveTypeRequest request)
        {
            request.LeaveTypeName = request.LeaveTypeName.ToPascalCase();
            var leaveType = _mapper.Map<LeaveType>(request);
            var createdLeaveType = await _repository.Create(leaveType);
            return _mapper.Map<LeaveTypeResponse>(createdLeaveType);
        }

        public async Task<LeaveTypeResponse> UpdateLeaveTypeAsync(int id, UpdateLeaveTypeRequest request)
        {
            var leaveType = await _repository.Get(id);
            if (leaveType == null) throw new NotFoundException("Leave type not found.");

            request.LeaveTypeName = request.LeaveTypeName.ToPascalCase();
            _mapper.Map(request, leaveType);
            var updatedLeaveType = await _repository.Update(id, leaveType);
            return _mapper.Map<LeaveTypeResponse>(updatedLeaveType);
        }

        public async Task<LeaveTypeResponse> DeleteLeaveTypeAsync(int id)
        {
            var leaveType = await _repository.Get(id);
            if (leaveType == null) throw new NotFoundException("Leave type not found.");
            var deletedLeaveType = await _repository.Delete(id);
            return _mapper.Map<LeaveTypeResponse>(deletedLeaveType);
        }
    }
}
