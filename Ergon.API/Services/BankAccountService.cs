using AutoMapper;
using Ergon.DTOs.BankAccount;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IRepository<Guid, BankAccount> _repository;
        private readonly IRepository<Guid, Employee> _employeeRepository;
        private readonly IMapper _mapper;

        public BankAccountService(IRepository<Guid, BankAccount> repository, IRepository<Guid, Employee> employeeRepository, IMapper mapper)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BankAccountResponse>> GetAllBankAccountsAsync(Guid employeeId)
        {
            var all = await _repository.GetAll();
            var bankAccounts = all.Where(b => b.EmployeeId == employeeId).ToList();
            return _mapper.Map<List<BankAccountResponse>>(bankAccounts);
        }

        public async Task<BankAccountResponse> GetBankAccountByIdAsync(Guid bankAccountId)
        {
            var bankAccount = await _repository.Get(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> CreateBankAccountAsync(Guid employeeId, BankAccountRequest request)
        {
            var employee = await _employeeRepository.Get(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var bankAccount = _mapper.Map<BankAccount>(request);
            bankAccount.BankAccountId = Guid.NewGuid();
            bankAccount.EmployeeId = employeeId;
            bankAccount.CreatedAt = DateTime.UtcNow;
            bankAccount.UpdatedAt = DateTime.UtcNow;

            await _repository.Create(bankAccount);
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> UpdateBankAccountAsync(Guid bankAccountId, BankAccountRequest request)
        {
            var bankAccount = await _repository.Get(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            _mapper.Map(request, bankAccount);
            bankAccount.UpdatedAt = DateTime.UtcNow;
            await _repository.Update(bankAccountId, bankAccount);
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> DeleteBankAccountAsync(Guid bankAccountId)
        {
            var bankAccount = await _repository.Get(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            await _repository.Delete(bankAccountId);
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> SetPrimaryAsync(Guid employeeId, Guid bankAccountId)
        {
            var bankAccount = await _repository.Get(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");

            var all = await _repository.GetAll();
            var allAccounts = all.Where(b => b.EmployeeId == employeeId).ToList();

            foreach (var account in allAccounts)
            {
                account.IsPrimary = false;
                await _repository.Update(account.BankAccountId, account);
            }

            bankAccount.IsPrimary = true;
            await _repository.Update(bankAccountId, bankAccount);
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }
    }
}
