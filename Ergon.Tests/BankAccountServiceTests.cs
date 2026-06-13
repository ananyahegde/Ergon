using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.BankAccount;
using Ergon.Exceptions;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class BankAccountServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IMapper> _mockMapper = null!;
        private BankAccountService _bankAccountService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockMapper = new Mock<IMapper>();
            _bankAccountService = new BankAccountService(_context, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllBankAccounts_ReturnsEmployeeBankAccounts()
        {
            var employeeId = Guid.NewGuid();
            _context.BankAccounts.AddRange(
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" },
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), BankName = "HDFC", AccountNumber = "5678", IfscCode = "HDFC001", AccountHolderName = "Priya" }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<BankAccountResponse>>(It.IsAny<List<BankAccount>>()))
                .Returns(new List<BankAccountResponse> { new() });

            var result = await _bankAccountService.GetAllBankAccountsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetBankAccountById_BankAccountExists_ReturnsBankAccountResponse()
        {
            var bankAccountId = Guid.NewGuid();
            _context.BankAccounts.Add(new BankAccount { BankAccountId = bankAccountId, EmployeeId = Guid.NewGuid(), BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { BankAccountId = bankAccountId });

            var result = await _bankAccountService.GetBankAccountByIdAsync(bankAccountId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetBankAccountById_BankAccountNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.GetBankAccountByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreateBankAccount_ValidRequest_ReturnsBankAccountResponse()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            await _context.SaveChangesAsync();

            var request = new BankAccountRequest { BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" };
            var bankAccount = new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" };

            _mockMapper.Setup(m => m.Map<BankAccount>(request)).Returns(bankAccount);
            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { BankAccountId = bankAccount.BankAccountId });

            var result = await _bankAccountService.CreateBankAccountAsync(employeeId, request);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CreateBankAccount_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.CreateBankAccountAsync(Guid.NewGuid(), new BankAccountRequest()));
        }

        [Test]
        public async Task UpdateBankAccount_BankAccountNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.UpdateBankAccountAsync(Guid.NewGuid(), new BankAccountRequest()));
        }

        [Test]
        public async Task DeleteBankAccount_BankAccountNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.DeleteBankAccountAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task SetPrimary_SetsOnlyOneAsPrimary()
        {
            var employeeId = Guid.NewGuid();
            var primaryId = Guid.NewGuid();
            _context.BankAccounts.AddRange(
                new BankAccount { BankAccountId = primaryId, EmployeeId = employeeId, BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun", IsPrimary = false },
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, BankName = "HDFC", AccountNumber = "5678", IfscCode = "HDFC001", AccountHolderName = "Arjun", IsPrimary = true }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { IsPrimary = true });

            await _bankAccountService.SetPrimaryAsync(employeeId, primaryId);

            var primary = await _context.BankAccounts.Where(b => b.EmployeeId == employeeId && b.IsPrimary).ToListAsync();
            Assert.That(primary.Count, Is.EqualTo(1));
            Assert.That(primary[0].BankAccountId, Is.EqualTo(primaryId));
        }

        [Test]
        public async Task SetPrimary_BankAccountNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.SetPrimaryAsync(Guid.NewGuid(), Guid.NewGuid()));
        }
    }
}
