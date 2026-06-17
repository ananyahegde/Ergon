using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Payroll;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class PayrollServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IPayrollRepository> _mockPayrollRepo = null!;
        private Mock<INotificationService> _mockNotification = null!;
        private Mock<IMapper> _mockMapper = null!;
        private PayrollService _payrollService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockPayrollRepo = new Mock<IPayrollRepository>();
            _mockNotification = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _payrollService = new PayrollService(_context, _mockPayrollRepo.Object, _mockNotification.Object, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private async Task<(Guid employeeId, int salaryStructureId)> SeedEmployeeWithSalaryAsync()
        {
            var salaryStructureId = 1;
            _context.SalaryStructures.Add(new SalaryStructure { SalaryStructureId = salaryStructureId, SalaryStructureName = "Structure A" });
            _context.SalaryComponents.AddRange(
                new SalaryComponent { SalaryComponentId = 1, SalaryStructureId = salaryStructureId, ComponentName = "Basic", ComponentType = SalaryComponentEnum.Earning, Amount = 50000 },
                new SalaryComponent { SalaryComponentId = 2, SalaryStructureId = salaryStructureId, ComponentName = "PF", ComponentType = SalaryComponentEnum.Deduction, Amount = 1800 }
            );
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                SalaryStructureId = salaryStructureId,
                EmploymentStatus = EmploymentStatusEnum.Active
            });
            await _context.SaveChangesAsync();
            return (employeeId, salaryStructureId);
        }

        [Test]
        public async Task GetPayrollById_Exists_ReturnsResponse()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            var payrollId = Guid.NewGuid();
            _context.Payrolls.Add(new Payroll { PayrollId = payrollId, EmployeeId = employeeId, Month = 6, Year = 2026, NetSalary = 48200, PayrollStatus = PayrollStatusEnum.ApprovalPending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<PayrollResponse>(It.IsAny<Payroll>()))
                .Returns(new PayrollResponse { PayrollId = payrollId });

            var result = await _payrollService.GetPayrollByIdAsync(payrollId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetPayrollById_NotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _payrollService.GetPayrollByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreatePayroll_ValidRequest_CreatesPayroll()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            var request = new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2026 };

            _mockMapper.Setup(m => m.Map<PayrollResponse>(It.IsAny<Payroll>()))
                .Returns(new PayrollResponse());

            var result = await _payrollService.CreatePayrollAsync(request);

            Assert.That(_context.Payrolls.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CreatePayroll_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _payrollService.CreatePayrollAsync(new CreatePayrollRequest { EmployeeId = Guid.NewGuid(), Month = 6, Year = 2026 }));
        }

        [Test]
        public async Task CreatePayroll_Duplicate_ThrowsConflictException()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            _context.Payrolls.Add(new Payroll { PayrollId = Guid.NewGuid(), EmployeeId = employeeId, Month = 6, Year = 2026, NetSalary = 48200, PayrollStatus = PayrollStatusEnum.ApprovalPending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<ConflictException>(() =>
                _payrollService.CreatePayrollAsync(new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2026 }));
        }

        [Test]
        public async Task RunPayroll_GeneratesForAllActiveEmployees()
        {
            await SeedEmployeeWithSalaryAsync();
            await SeedEmployeeWithSalaryAsync();

            await _payrollService.RunPayrollAsync(Guid.NewGuid(), 6, 2026);

            Assert.That(_context.Payrolls.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task RunPayroll_SkipsExistingPayrolls()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            _context.Payrolls.Add(new Payroll { PayrollId = Guid.NewGuid(), EmployeeId = employeeId, Month = 6, Year = 2026, NetSalary = 48200, PayrollStatus = PayrollStatusEnum.ApprovalPending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _payrollService.RunPayrollAsync(Guid.NewGuid(), 6, 2026);

            Assert.That(_context.Payrolls.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task BulkApprove_NoUnapprovedPayrolls_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _payrollService.BulkApprovePayrollsAsync(Guid.NewGuid(), new BulkApprovePayrollRequest { Month = 6, Year = 2026 }));
        }

        [Test]
        public async Task BulkApprove_ValidRequest_ApprovesAndNotifies()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            _context.Payrolls.Add(new Payroll { PayrollId = Guid.NewGuid(), EmployeeId = employeeId, Month = 6, Year = 2026, NetSalary = 48200, PayrollStatus = PayrollStatusEnum.ApprovalPending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            _mockPayrollRepo.Setup(r => r.BulkApprovePayrollsAsync(6, 2026, It.IsAny<Guid>())).Returns(Task.CompletedTask);

            await _payrollService.BulkApprovePayrollsAsync(Guid.NewGuid(), new BulkApprovePayrollRequest { Month = 6, Year = 2026 });

            _mockNotification.Verify(n => n.CreateNotificationAsync(employeeId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeletePayroll_ApprovedPayroll_ThrowsBadRequestException()
        {
            var (employeeId, _) = await SeedEmployeeWithSalaryAsync();
            var payrollId = Guid.NewGuid();
            _context.Payrolls.Add(new Payroll { PayrollId = payrollId, EmployeeId = employeeId, Month = 6, Year = 2026, NetSalary = 48200, PayrollStatus = PayrollStatusEnum.Approved, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _payrollService.DeletePayrollAsync(payrollId));
        }

        [Test]
        public async Task DeletePayroll_NotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _payrollService.DeletePayrollAsync(Guid.NewGuid()));
        }
    }
}
