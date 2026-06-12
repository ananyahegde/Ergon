using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Employee;
using Ergon.Exceptions;
using Ergon.Repositories;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class EmployeeServiceTests
    {
        private ErgonContext _context = null!;
        private IRepository<Guid, Employee> _repository = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<INotificationService> _mockNotification = null!;
        private EmployeeService _employeeService = null!;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ErgonContext(options);
            _repository = new Repository<Guid, Employee>(_context);
            _mockMapper = new Mock<IMapper>();
            _mockNotification = new Mock<INotificationService>();

            _employeeService = new EmployeeService(
                _repository,
                _context,
                _mockMapper.Object,
                _mockNotification.Object
            );

            _context.Roles.Add(new Role { RoleId = 1, RoleName = "Employee" });
            _context.Departments.Add(new Department { DepartmentId = 1, DepartmentName = "Engineering" });
            _context.Branches.Add(new Branch { BranchId = 1, BranchName = "Head Office" });
            _context.Designations.Add(new Designation { DesignationId = 1, DesignationName = "Software Engineer" });
            _context.Shifts.Add(new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) });
            _context.SalaryStructures.Add(new SalaryStructure { SalaryStructureId = 1, SalaryStructureName = "Structure A" });
            _context.Cities.Add(new City { CityId = 1, CityName = "Bangalore" });
            _context.States.Add(new State { StateId = 1, StateName = "Karnataka" });
            _context.Countries.Add(new Country { CountryId = 1, CountryName = "India" });
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetEmployeeById_EmployeeExists_ReturnsDetailResponse()
        {
            var employeeId = Guid.NewGuid();

            var employee = new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var response = new EmployeeDetailResponse { EmployeeId = employeeId, FirstName = "Arjun" };
            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>())).Returns(response);

            var result = await _employeeService.GetEmployeeByIdAsync(employeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Arjun"));
        }

        [Test]
        public async Task GetEmployeeById_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.GetEmployeeByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task GetAllEmployees_ReturnsPagedResult()
        {
            _context.Employees.AddRange(
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Arjun", LastName = "Nair" },
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Priya", LastName = "Menon" }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse> { new(), new() });

            var request = new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10 };
            var result = await _employeeService.GetAllEmployeesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllEmployees_SearchFilter_ReturnsMatchingEmployees()
        {
            _context.Employees.AddRange(
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Arjun", LastName = "Nair" },
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Priya", LastName = "Menon" }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse> { new() });

            var request = new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, Search = "Arjun" };
            var result = await _employeeService.GetAllEmployeesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_BranchFilter_ReturnsMatchingEmployees()
        {
            _context.Employees.AddRange(
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Arjun", LastName = "Nair", BranchId = 1 },
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Priya", LastName = "Menon", BranchId = 2 }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse> { new() });

            var request = new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, BranchIds = new List<int> { 1 } };
            var result = await _employeeService.GetAllEmployeesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }


        [Test]
        public async Task GetMyTeam_ReturnsDirectReports()
        {
            var managerId = Guid.NewGuid();
            _context.Employees.AddRange(
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Sub1", LastName = "A", ReportsTo = managerId },
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Sub2", LastName = "B", ReportsTo = managerId },
                new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Other", LastName = "C", ReportsTo = Guid.NewGuid() }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse> { new(), new() });

            var result = await _employeeService.GetMyTeamAsync(managerId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetMyTeam_NoDirectReports_ReturnsEmptyList()
        {
            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse>());

            var result = await _employeeService.GetMyTeamAsync(Guid.NewGuid());

            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task CreateEmployee_ValidRequest_ReturnsResponseWithTempPassword()
        {
            var employeeId = Guid.NewGuid();
            var request = new CreateEmployeeRequest { FirstName = "Arjun", LastName = "Nair" };

            var employee = new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };

            _mockMapper.Setup(m => m.Map<Employee>(request)).Returns(employee);

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employeeId, FirstName = "Arjun" });

            var result = await _employeeService.CreateEmployeeAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TempPassword, Is.Not.Null);
            Assert.That(result.TempPassword!.Length, Is.EqualTo(8));
        }


        [Test]
        public async Task UpdateEmployee_EmployeeExists_ReturnsUpdatedResponse()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };
            var request = new UpdateEmployeeRequest { FirstName = "Arjun", LastName = "Updated" };

            _mockMapper.Setup(m => m.Map(request, employee));

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employeeId, FirstName = "Arjun" });

            var result = await _employeeService.UpdateEmployeeAsync(employeeId, request);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateEmployee_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.UpdateEmployeeAsync(Guid.NewGuid(), new UpdateEmployeeRequest()));
        }


        [Test]
        public async Task DeleteEmployee_EmployeeExists_ReturnsDeletedEmployee()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(employee))
                .Returns(new EmployeeDetailResponse { EmployeeId = employeeId, FirstName = "Arjun" });

            var result = await _employeeService.DeleteEmployeeAsync(employeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Arjun"));
        }

        [Test]
        public async Task DeleteEmployee_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.DeleteEmployeeAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task UpdateEmployeeStatus_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.UpdateEmployeeStatusAsync(Guid.NewGuid(), new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Suspended
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_HasDirectReports_InactiveStatus_ThrowsBadRequestException()
        {
            var managerId = Guid.NewGuid();
            var manager = new Employee { EmployeeId = managerId, FirstName = "Arjun", LastName = "Nair" };
            var report = new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Sub", LastName = "A", ReportsTo = managerId };

            _context.Employees.AddRange(manager, report);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.UpdateEmployeeStatusAsync(managerId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Suspended
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_HasDirectReports_ActiveStatus_Succeeds()
        {
            var managerId = Guid.NewGuid();

            var manager = new Employee
            {
                EmployeeId = managerId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };

            var report = new Employee { EmployeeId = Guid.NewGuid(), FirstName = "Sub", LastName = "A", ReportsTo = managerId };

            _context.Employees.AddRange(manager, report);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = managerId });

            Assert.DoesNotThrowAsync(() =>
                _employeeService.UpdateEmployeeStatusAsync(managerId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Active
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_NoDirectReports_InactiveStatus_Succeeds()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employeeId });

            Assert.DoesNotThrowAsync(() =>
                _employeeService.UpdateEmployeeStatusAsync(employeeId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Resigned
                }));
        }
    }
}
