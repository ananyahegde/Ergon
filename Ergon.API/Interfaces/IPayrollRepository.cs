namespace Ergon.Interfaces
{
    public interface IPayrollRepository
    {
        Task BulkApprovePayrollsAsync(int month, int year, Guid approvedBy);
    }
}
