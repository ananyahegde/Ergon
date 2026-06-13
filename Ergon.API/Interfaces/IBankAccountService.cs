using Ergon.DTOs.BankAccount;

namespace Ergon.Interfaces
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccountResponse>> GetAllBankAccountsAsync(Guid employeeId);
        Task<BankAccountResponse> GetBankAccountByIdAsync(Guid bankAccountId);
        Task<BankAccountResponse> CreateBankAccountAsync(Guid employeeId, BankAccountRequest request);
        Task<BankAccountResponse> UpdateBankAccountAsync(Guid bankAccountId, BankAccountRequest request);
        Task<BankAccountResponse> DeleteBankAccountAsync(Guid bankAccountId);
        Task<BankAccountResponse> SetPrimaryAsync(Guid employeeId, Guid bankAccountId);
    }
}
