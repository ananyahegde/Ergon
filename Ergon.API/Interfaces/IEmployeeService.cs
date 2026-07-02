using Ergon.DTOs.Employee;

namespace Ergon.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDetailResponse> GetEmployeeByIdAsync(Guid id);
        Task<PagedEmployeeResponse> GetAllEmployeesAsync(GetAllEmployeesRequest request);
        Task<IEnumerable<EmployeeListResponse>> GetMyTeamAsync(Guid managerId);
        Task<EmployeeDetailResponse> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<EmployeeDetailResponse> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request);
        Task<EmployeeDetailResponse> DeleteEmployeeAsync(Guid id);
        Task<EmployeeDetailResponse> UpdateEmployeeStatusAsync(Guid id, UpdateEmployeeStatusRequest request);
        Task<EmployeeDetailResponse> UpdateEmployeePfpAsync(Guid id, IFormFile pfp);
        Task<(byte[] FileBytes, string ContentType)> GetEmployeePfpAsync(Guid id);
        Task<EmployeeDetailResponse> UpdateProfileAsync(Guid id, UpdateProfileRequest request);
        Task<EmployeeStatsResponse> GetEmployeeStatsAsync();
    }
}
