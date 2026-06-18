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
    [TestFixture]
    public class PayrollServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IPayrollRepository> _mockPayrollRepo = null!;
        private Mock<IRepository<Guid, Payroll>> _mockGenericRepo = null!;
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
            _mockGenericRepo = new Mock<IRepository<Guid, Payroll>>();
            _mockNotification = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _payrollService = new PayrollService(
                _mockPayrollRepo.Object,
                _mockGenericRepo.Object,
                _mockNotification.Object,
                _mockMapper.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetAllPayrolls_ValidRequest_ReturnsPagedResponse()
        {
            var request = new GetAllPayrollsRequest { PageSize = 10, PageNumber = 1 };
            var payrolls = new List<Payroll> { new() { PayrollId = Guid.NewGuid() } };
            _mockPayrollRepo.Setup(r => r.GetPagedPayrollsAsync(request)).ReturnsAsync((payrolls, 1));
            _mockMapper.Setup(m => m.Map<List<PayrollResponse>>(It.IsAny<List<Payroll>>()))
                .Returns(new List<PayrollResponse> { new() });

            var result = await _payrollService.GetAllPayrollsAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Items.Count, Is.EqualTo(1));
        }


        [Test]
        public async Task GetPayrollById_ExistingId_ReturnsResponse()
        {
            var payrollId = Guid.NewGuid();
            var payroll = new Payroll { PayrollId = payrollId };
            _mockPayrollRepo.Setup(r => r.GetPayrollByIdAsync(payrollId)).ReturnsAsync(payroll);
            _mockMapper.Setup(m => m.Map<PayrollResponse>(payroll)).Returns(new PayrollResponse { PayrollId = payrollId });

            var result = await _payrollService.GetPayrollByIdAsync(payrollId);

            Assert.That(result.PayrollId, Is.EqualTo(payrollId));
        }

        [Test]
        public void GetPayrollById_NotFound_ThrowsNotFoundException()
        {
            _mockPayrollRepo.Setup(r => r.GetPayrollByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Payroll?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _payrollService.GetPayrollByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task CreatePayroll_ValidRequest_ReturnsResponse()
        {
            var employeeId = Guid.NewGuid();
            var request = new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2025 };
            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmploymentStatus = EmploymentStatusEnum.Active,
                SalaryStructure = new SalaryStructure
                {
                    SalaryComponents = new List<SalaryComponent>
                    {
                        new() { ComponentName = "Basic", ComponentType = SalaryComponentEnum.Earning, Amount = 50000 }
                    }
                }
            };
            var taxSlabs = new List<TaxSlab>();
            var leaves = new List<Leave>();

            _mockPayrollRepo.Setup(r => r.GetEmployeeWithSalaryAsync(employeeId)).ReturnsAsync(employee);
            _mockPayrollRepo.Setup(r => r.PayrollExistsAsync(employeeId, 6, 2025)).ReturnsAsync(false);
            _mockPayrollRepo.Setup(r => r.GetTaxSlabsAsync()).ReturnsAsync(taxSlabs);
            _mockPayrollRepo.Setup(r => r.GetUnpaidLeavesAsync(employeeId, 6, 2025)).ReturnsAsync(leaves);
            _mockMapper.Setup(m => m.Map<PayrollResponse>(It.IsAny<Payroll>())).Returns(new PayrollResponse { EmployeeId = employeeId });

            var result = await _payrollService.CreatePayrollAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployeeId, Is.EqualTo(employeeId));
            _mockPayrollRepo.Verify(r => r.AddPayrollAsync(It.IsAny<Payroll>()), Times.Once);
            _mockPayrollRepo.Verify(r => r.AddPayrollComponentsAsync(It.IsAny<List<PayrollComponent>>()), Times.Once);
            _mockPayrollRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreatePayroll_InvalidMonth_ThrowsBadRequestException()
        {
            var request = new CreatePayrollRequest { EmployeeId = Guid.NewGuid(), Month = 13, Year = 2025 };

            Assert.ThrowsAsync<BadRequestException>(() => _payrollService.CreatePayrollAsync(request));
        }

        [Test]
        public void CreatePayroll_EmployeeNotFound_ThrowsNotFoundException()
        {
            var request = new CreatePayrollRequest { EmployeeId = Guid.NewGuid(), Month = 6, Year = 2025 };
            _mockPayrollRepo.Setup(r => r.GetEmployeeWithSalaryAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _payrollService.CreatePayrollAsync(request));
        }

        [Test]
        public void CreatePayroll_InactiveEmployee_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var request = new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2025 };
            var employee = new Employee { EmployeeId = employeeId, EmploymentStatus = EmploymentStatusEnum.Resigned };
            _mockPayrollRepo.Setup(r => r.GetEmployeeWithSalaryAsync(employeeId)).ReturnsAsync(employee);

            Assert.ThrowsAsync<BadRequestException>(() => _payrollService.CreatePayrollAsync(request));
        }

        [Test]
        public void CreatePayroll_DuplicatePayroll_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            var request = new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2025 };
            var employee = new Employee { EmployeeId = employeeId, EmploymentStatus = EmploymentStatusEnum.Active };
            _mockPayrollRepo.Setup(r => r.GetEmployeeWithSalaryAsync(employeeId)).ReturnsAsync(employee);
            _mockPayrollRepo.Setup(r => r.PayrollExistsAsync(employeeId, 6, 2025)).ReturnsAsync(true);

            Assert.ThrowsAsync<ConflictException>(() => _payrollService.CreatePayrollAsync(request));
        }

        [Test]
        public void CreatePayroll_NoEarningComponents_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var request = new CreatePayrollRequest { EmployeeId = employeeId, Month = 6, Year = 2025 };
            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmploymentStatus = EmploymentStatusEnum.Active,
                SalaryStructure = new SalaryStructure
                {
                    SalaryComponents = new List<SalaryComponent>
                    {
                        new() { ComponentName = "PF", ComponentType = SalaryComponentEnum.Deduction, Amount = 1800 }
                    }
                }
            };
            _mockPayrollRepo.Setup(r => r.GetEmployeeWithSalaryAsync(employeeId)).ReturnsAsync(employee);
            _mockPayrollRepo.Setup(r => r.PayrollExistsAsync(employeeId, 6, 2025)).ReturnsAsync(false);

            Assert.ThrowsAsync<BadRequestException>(() => _payrollService.CreatePayrollAsync(request));
        }


        [Test]
        public async Task RunPayroll_ValidMonthYear_SkipsExistingAndCreatesNew()
        {
            var emp1 = Guid.NewGuid();
            var emp2 = Guid.NewGuid();
            var employees = new List<Employee>
            {
                new()
                {
                    EmployeeId = emp1,
                    EmploymentStatus = EmploymentStatusEnum.Active,
                    SalaryStructure = new SalaryStructure
                    {
                        SalaryComponents = new List<SalaryComponent>
                        {
                            new() { ComponentName = "Basic", ComponentType = SalaryComponentEnum.Earning, Amount = 40000 }
                        }
                    }
                },
                new()
                {
                    EmployeeId = emp2,
                    EmploymentStatus = EmploymentStatusEnum.Active,
                    SalaryStructure = new SalaryStructure
                    {
                        SalaryComponents = new List<SalaryComponent>
                        {
                            new() { ComponentName = "Basic", ComponentType = SalaryComponentEnum.Earning, Amount = 60000 }
                        }
                    }
                }
            };

            _mockPayrollRepo.Setup(r => r.GetActiveEmployeesWithSalaryAsync()).ReturnsAsync(employees);
            _mockPayrollRepo.Setup(r => r.GetTaxSlabsAsync()).ReturnsAsync(new List<TaxSlab>());
            _mockPayrollRepo.Setup(r => r.GetUnpaidLeavesAsync(null, 5, 2025)).ReturnsAsync(new List<Leave>());
            _mockPayrollRepo.Setup(r => r.PayrollExistsAsync(emp1, 5, 2025)).ReturnsAsync(true);
            _mockPayrollRepo.Setup(r => r.PayrollExistsAsync(emp2, 5, 2025)).ReturnsAsync(false);

            await _payrollService.RunPayrollAsync(Guid.NewGuid(), 5, 2025);

            _mockPayrollRepo.Verify(r => r.AddPayrollAsync(It.IsAny<Payroll>()), Times.Once);
            _mockPayrollRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void RunPayroll_FutureMonth_ThrowsBadRequestException()
        {
            var future = DateTime.Now.AddMonths(2);
            Assert.ThrowsAsync<BadRequestException>(() =>
                _payrollService.RunPayrollAsync(Guid.NewGuid(), future.Month, future.Year));
        }


        [Test]
        public async Task BulkApprovePayrolls_ValidRequest_ApprovesAndNotifies()
        {
            var request = new BulkApprovePayrollRequest { Month = 5, Year = 2025 };
            var approvedBy = Guid.NewGuid();
            var emp1 = Guid.NewGuid();
            var payrolls = new List<Payroll>
            {
                new() { PayrollId = Guid.NewGuid(), EmployeeId = emp1, Month = 5, Year = 2025, PayrollStatus = PayrollStatusEnum.ApprovalPending }
            };

            _mockPayrollRepo.Setup(r => r.GetPayrollsByMonthYearAsync(5, 2025, PayrollStatusEnum.ApprovalPending)).ReturnsAsync(payrolls);

            await _payrollService.BulkApprovePayrollsAsync(approvedBy, request);

            _mockPayrollRepo.Verify(r => r.BulkApprovePayrollsAsync(5, 2025, approvedBy), Times.Once);
            _mockNotification.Verify(r => r.CreateNotificationAsync(emp1, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BulkApprovePayrolls_NoPendingPayrolls_ThrowsNotFoundException()
        {
            var request = new BulkApprovePayrollRequest { Month = 5, Year = 2025 };
            _mockPayrollRepo.Setup(r => r.GetPayrollsByMonthYearAsync(5, 2025, PayrollStatusEnum.ApprovalPending)).ReturnsAsync(new List<Payroll>());

            Assert.ThrowsAsync<NotFoundException>(() => _payrollService.BulkApprovePayrollsAsync(Guid.NewGuid(), request));
        }

        [Test]
        public void BulkApprovePayrolls_FutureMonth_ThrowsBadRequestException()
        {
            var future = DateTime.Now.AddMonths(2);
            var request = new BulkApprovePayrollRequest { Month = future.Month, Year = future.Year };

            Assert.ThrowsAsync<BadRequestException>(() => _payrollService.BulkApprovePayrollsAsync(Guid.NewGuid(), request));
        }


        [Test]
        public async Task DeletePayroll_PendingPayroll_DeletesAndReturnsResponse()
        {
            var payrollId = Guid.NewGuid();
            var payroll = new Payroll { PayrollId = payrollId, PayrollStatus = PayrollStatusEnum.ApprovalPending };
            _mockPayrollRepo.Setup(r => r.GetPayrollByIdAsync(payrollId)).ReturnsAsync(payroll);
            _mockGenericRepo.Setup(r => r.Delete(payrollId)).ReturnsAsync(payroll);
            _mockMapper.Setup(m => m.Map<PayrollResponse>(payroll)).Returns(new PayrollResponse { PayrollId = payrollId });

            var result = await _payrollService.DeletePayrollAsync(payrollId);

            Assert.That(result.PayrollId, Is.EqualTo(payrollId));
            _mockGenericRepo.Verify(r => r.Delete(payrollId), Times.Once);
        }

        [Test]
        public void DeletePayroll_ApprovedPayroll_ThrowsBadRequestException()
        {
            var payrollId = Guid.NewGuid();
            var payroll = new Payroll { PayrollId = payrollId, PayrollStatus = PayrollStatusEnum.Approved };
            _mockPayrollRepo.Setup(r => r.GetPayrollByIdAsync(payrollId)).ReturnsAsync(payroll);

            Assert.ThrowsAsync<BadRequestException>(() => _payrollService.DeletePayrollAsync(payrollId));
        }

        [Test]
        public void DeletePayroll_NotFound_ThrowsNotFoundException()
        {
            _mockPayrollRepo.Setup(r => r.GetPayrollByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Payroll?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _payrollService.DeletePayrollAsync(Guid.NewGuid()));
        }
    }
}
