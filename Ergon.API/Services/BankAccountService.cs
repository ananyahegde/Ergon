using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.BankAccount;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly ErgonContext _context;
        private readonly IMapper _mapper;

        public BankAccountService(ErgonContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BankAccountResponse>> GetAllBankAccountsAsync(Guid employeeId)
        {
            var bankAccounts = await _context.BankAccounts
                .Where(b => b.EmployeeId == employeeId)
                .ToListAsync();
            return _mapper.Map<List<BankAccountResponse>>(bankAccounts);
        }

        public async Task<BankAccountResponse> GetBankAccountByIdAsync(Guid bankAccountId)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> CreateBankAccountAsync(Guid employeeId, BankAccountRequest request)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var bankAccount = _mapper.Map<BankAccount>(request);
            bankAccount.BankAccountId = Guid.NewGuid();
            bankAccount.EmployeeId = employeeId;
            bankAccount.CreatedAt = DateTime.Now;
            bankAccount.UpdatedAt = DateTime.Now;

            await _context.BankAccounts.AddAsync(bankAccount);
            await _context.SaveChangesAsync();
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> UpdateBankAccountAsync(Guid bankAccountId, BankAccountRequest request)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            _mapper.Map(request, bankAccount);
            bankAccount.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> DeleteBankAccountAsync(Guid bankAccountId)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BankAccountResponse> SetPrimaryAsync(Guid employeeId, Guid bankAccountId)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId);
            if (bankAccount == null) throw new NotFoundException("Bank account not found.");

            var allAccounts = await _context.BankAccounts
                .Where(b => b.EmployeeId == employeeId)
                .ToListAsync();

            foreach (var account in allAccounts)
                account.IsPrimary = false;

            bankAccount.IsPrimary = true;
            await _context.SaveChangesAsync();
            return _mapper.Map<BankAccountResponse>(bankAccount);
        }
    }
}
