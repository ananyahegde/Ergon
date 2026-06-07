using Ergon.DTOs.LeaveType;

namespace Ergon.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<LeaveTypeResponse> GetLeaveTypeByIdAsync(int id);
        Task<IEnumerable<LeaveTypeResponse>> GetAllLeaveTypesAsync();
        Task<LeaveTypeResponse> CreateLeaveTypeAsync(CreateLeaveTypeRequest request);
        Task<LeaveTypeResponse> UpdateLeaveTypeAsync(int id, UpdateLeaveTypeRequest request);
        Task<LeaveTypeResponse> DeleteLeaveTypeAsync(int id);
    }
}
