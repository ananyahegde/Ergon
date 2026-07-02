using Ergon.DTOs.Employee;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(GetAllEmployeesRequest request);
        Task<Employee?> GetByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetTeamAsync(Guid managerId);
        Task<bool> HasDirectReportsAsync(Guid managerId);
        Task<bool> ExistsByWorkEmailAsync(string workEmail, Guid? excludeId = null);
        Task<bool> ExistsByPersonalEmailAsync(string personalEmail, Guid? excludeId = null);
        Task<bool> ExistsByPhoneAsync(string phone, Guid? excludeId = null);
        Task<Employee?> GetManagerAsync(Guid managerId);
        Task<EmployeeStatsResponse> GetEmployeeStatsAsync();
    }
}
