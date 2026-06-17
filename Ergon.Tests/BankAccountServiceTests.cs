using AutoMapper;
using Ergon.DTOs.BankAccount;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class BankAccountServiceTests
    {
        private Mock<IRepository<Guid, BankAccount>> _mockRepo = null!;
        private Mock<IRepository<Guid, Employee>> _mockEmployeeRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private BankAccountService _bankAccountService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Guid, BankAccount>>();
            _mockEmployeeRepo = new Mock<IRepository<Guid, Employee>>();
            _mockMapper = new Mock<IMapper>();
            _bankAccountService = new BankAccountService(_mockRepo.Object, _mockEmployeeRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAllBankAccounts_ReturnsEmployeeBankAccounts()
        {
            var employeeId = Guid.NewGuid();
            var bankAccounts = new List<BankAccount>
            {
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" },
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), BankName = "HDFC", AccountNumber = "5678", IfscCode = "HDFC001", AccountHolderName = "Priya" }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bankAccounts);
            _mockMapper.Setup(m => m.Map<List<BankAccountResponse>>(It.IsAny<List<BankAccount>>()))
                .Returns(new List<BankAccountResponse> { new() });

            var result = await _bankAccountService.GetAllBankAccountsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetBankAccountById_BankAccountExists_ReturnsBankAccountResponse()
        {
            var bankAccountId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(bankAccountId))
                .ReturnsAsync(new BankAccount { BankAccountId = bankAccountId, EmployeeId = Guid.NewGuid(), BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" });
            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { BankAccountId = bankAccountId });

            var result = await _bankAccountService.GetBankAccountByIdAsync(bankAccountId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetBankAccountById_BankAccountNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.GetBankAccountByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreateBankAccount_ValidRequest_ReturnsBankAccountResponse()
        {
            var employeeId = Guid.NewGuid();
            var request = new BankAccountRequest { BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" };
            var bankAccount = new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, BankName = "SBI", AccountNumber = "1234", IfscCode = "SBI001", AccountHolderName = "Arjun" };

            _mockEmployeeRepo.Setup(r => r.Get(employeeId)).ReturnsAsync(new Employee { EmployeeId = employeeId });
            _mockMapper.Setup(m => m.Map<BankAccount>(request)).Returns(bankAccount);
            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { BankAccountId = bankAccount.BankAccountId });

            var result = await _bankAccountService.CreateBankAccountAsync(employeeId, request);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CreateBankAccount_EmployeeNotFound_ThrowsNotFoundException()
        {
            _mockEmployeeRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.CreateBankAccountAsync(Guid.NewGuid(), new BankAccountRequest()));
        }

        [Test]
        public async Task UpdateBankAccount_BankAccountNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.UpdateBankAccountAsync(Guid.NewGuid(), new BankAccountRequest()));
        }

        [Test]
        public async Task DeleteBankAccount_BankAccountNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.DeleteBankAccountAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task SetPrimary_SetsOnlyOneAsPrimary()
        {
            var employeeId = Guid.NewGuid();
            var primaryId = Guid.NewGuid();
            var accounts = new List<BankAccount>
            {
                new BankAccount { BankAccountId = primaryId, EmployeeId = employeeId, IsPrimary = false },
                new BankAccount { BankAccountId = Guid.NewGuid(), EmployeeId = employeeId, IsPrimary = true }
            };

            _mockRepo.Setup(r => r.Get(primaryId)).ReturnsAsync(accounts[0]);
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(accounts);
            _mockMapper.Setup(m => m.Map<BankAccountResponse>(It.IsAny<BankAccount>()))
                .Returns(new BankAccountResponse { IsPrimary = true });

            await _bankAccountService.SetPrimaryAsync(employeeId, primaryId);

            Assert.That(accounts.Count(a => a.IsPrimary), Is.EqualTo(1));
            Assert.That(accounts.First(a => a.BankAccountId == primaryId).IsPrimary, Is.True);
        }

        [Test]
        public async Task SetPrimary_BankAccountNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _bankAccountService.SetPrimaryAsync(Guid.NewGuid(), Guid.NewGuid()));
        }
    }
}
