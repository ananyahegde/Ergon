using Ergon.DTOs.Payroll;
using Ergon.Models;

namespace Ergon.Interfaces
{
    public interface IPayrollRepository
    {
        Task<Employee?> GetEmployeeWithSalaryAsync(Guid employeeId);
        Task<List<Employee>> GetActiveEmployeesWithSalaryAsync();
        Task<bool> PayrollExistsAsync(Guid employeeId, int month, int year);
        Task<(List<Payroll> Items, int TotalCount)> GetPagedPayrollsAsync(GetAllPayrollsRequest request);
        Task<Payroll?> GetPayrollByIdAsync(Guid payrollId);
        Task<List<Payroll>> GetUnapprovedPayrollsAsync();
        Task<List<Payroll>> GetPayrollsByMonthYearAsync(int month, int year, PayrollStatusEnum? status = null);
        Task<List<TaxSlab>> GetTaxSlabsAsync();
        Task<List<Leave>> GetUnpaidLeavesAsync(Guid? employeeId, int month, int year);
        Task AddPayrollAsync(Payroll payroll);
        Task AddPayrollComponentsAsync(List<PayrollComponent> components);
        Task SaveChangesAsync();
        Task BulkApprovePayrollsAsync(int month, int year, Guid approvedBy);
    }
}
