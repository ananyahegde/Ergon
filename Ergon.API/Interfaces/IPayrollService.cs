using Ergon.DTOs.Payroll;

namespace Ergon.Interfaces
{
    public interface IPayrollService
    {
        Task<PagedPayrollResponse> GetAllPayrollsAsync(GetAllPayrollsRequest request);
        Task<PayrollResponse> GetPayrollByIdAsync(Guid payrollId);
        Task<PayrollResponse> CreatePayrollAsync(CreatePayrollRequest request);
        Task<IEnumerable<PayrollResponse>> GetUnapprovedPayrollsAsync();
        Task RunPayrollAsync(Guid createdBy, int month, int year);
        Task BulkApprovePayrollsAsync(Guid approvedBy, BulkApprovePayrollRequest request);
        Task<PayrollResponse> DeletePayrollAsync(Guid payrollId);
    }
}
