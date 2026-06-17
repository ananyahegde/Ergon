using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Employee;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Repositories;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Http;

namespace Ergon.Tests
{
    public class EmployeeServiceTests
    {
        private ErgonContext _context = null!;
        private IRepository<Guid, Employee> _repository = null!;
        private IEmployeeRepository _employeeRepository = null!;
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
            _employeeRepository = new EmployeeRepository(_context);
            _mockMapper = new Mock<IMapper>();
            _mockNotification = new Mock<INotificationService>();

            _employeeService = new EmployeeService(
                _repository,
                _employeeRepository,
                _mockMapper.Object,
                _mockNotification.Object
            );

            _context.Roles.Add(new Role { RoleId = 1, RoleName = "Employee" });
            _context.Departments.Add(new Department { DepartmentId = 1, DepartmentName = "Engineering" });
            _context.Branches.Add(new Branch { BranchId = 1, BranchName = "Head Office" });
            _context.Designations.Add(new Designation { DesignationId = 1, DesignationName = "Software Engineer" });
            _context.Shifts.Add(new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) });
            _context.SalaryStructures.Add(new SalaryStructure { SalaryStructureId = 1, SalaryStructureName = "Structure A" });
            _context.LeaveEntitlements.Add(new LeaveEntitlement { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard" });
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


        private Employee MakeEmployee(Guid? id = null, string firstName = "Arjun", string lastName = "Nair",
            string workEmail = "arjun@ergon.com", string personalEmail = "arjun@gmail.com",
            string phone = "9876543210", EmploymentStatusEnum status = EmploymentStatusEnum.Active,
            Guid? reportsTo = null)
        {
            return new Employee
            {
                EmployeeId = id ?? Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                WorkEmail = workEmail,
                PersonalEmail = personalEmail,
                Phone = phone,
                PasswordHash = "hash",
                AddressLine1 = "123 Main St",
                DateOfBirth = new DateOnly(1995, 1, 1),
                DateOfJoining = DateOnly.FromDateTime(DateTime.UtcNow),
                Gender = GenderEnum.Male,
                EmploymentType = EmploymentTypeEnum.FullTime,
                EmploymentStatus = status,
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                LeaveEntitlementId = 1,
                CityId = 1,
                StateId = 1,
                CountryId = 1,
                ReportsTo = reportsTo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private CreateEmployeeRequest MakeCreateRequest(
            string workEmail = "new@ergon.com",
            string personalEmail = "new@gmail.com",
            string phone = "9000000001",
            DateOnly? dateOfBirth = null,
            DateOnly? dateOfJoining = null,
            Guid? reportsTo = null)
        {
            return new CreateEmployeeRequest
            {
                FirstName = "New",
                LastName = "Employee",
                WorkEmail = workEmail,
                PersonalEmail = personalEmail,
                Phone = phone,
                DateOfBirth = dateOfBirth ?? new DateOnly(1995, 6, 15),
                DateOfJoining = dateOfJoining ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Gender = GenderEnum.Female,
                AddressLine1 = "456 Side St",
                StateId = 1,
                CountryId = 1,
                EmploymentType = EmploymentTypeEnum.FullTime,
                RoleId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                LeaveEntitlementId = 1,
                ReportsTo = reportsTo
            };
        }

        private void SetupMapperForCreate(CreateEmployeeRequest request, Employee employee, Guid employeeId)
        {
            _mockMapper.Setup(m => m.Map<Employee>(request)).Returns(employee);
            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employeeId, FirstName = employee.FirstName });
        }


        [Test]
        public async Task GetEmployeeById_EmployeeExists_ReturnsDetailResponse()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId, FirstName = employee.FirstName });

            var result = await _employeeService.GetEmployeeByIdAsync(employee.EmployeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Arjun"));
        }

        [Test]
        public void GetEmployeeById_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.GetEmployeeByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task GetAllEmployees_NoFilters_ReturnsAllEmployees()
        {
            _context.Employees.AddRange(
                MakeEmployee(workEmail: "a@ergon.com", personalEmail: "a@gmail.com", phone: "1111111111"),
                MakeEmployee(workEmail: "b@ergon.com", personalEmail: "b@gmail.com", phone: "2222222222")
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse> { new(), new() });

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10 });

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllEmployees_SearchByFirstName_ReturnsMatchingEmployees()
        {
            _context.Employees.AddRange(
                MakeEmployee(firstName: "Arjun", workEmail: "arjun@ergon.com", personalEmail: "arjun@gmail.com", phone: "1111111111"),
                MakeEmployee(firstName: "Priya", workEmail: "priya@ergon.com", personalEmail: "priya@gmail.com", phone: "2222222222")
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, Search = "arjun" });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_SearchCaseInsensitive_ReturnsMatchingEmployees()
        {
            _context.Employees.Add(MakeEmployee(firstName: "Rahul", workEmail: "rahul@ergon.com", personalEmail: "rahul@gmail.com", phone: "3333333333"));
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, Search = "RAHUL" });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_SearchNoMatch_ReturnsEmpty()
        {
            _context.Employees.Add(MakeEmployee());
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse>());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, Search = "xyz" });

            Assert.That(result.TotalCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllEmployees_DepartmentFilter_ReturnsMatchingEmployees()
        {
            _context.Departments.Add(new Department { DepartmentId = 2, DepartmentName = "HR" });
            await _context.SaveChangesAsync();

            _context.Employees.AddRange(
                MakeEmployee(workEmail: "a@ergon.com", personalEmail: "a@gmail.com", phone: "1111111111"),
                MakeEmployee(workEmail: "b@ergon.com", personalEmail: "b@gmail.com", phone: "2222222222")
            );
            var emp = MakeEmployee(workEmail: "c@ergon.com", personalEmail: "c@gmail.com", phone: "3333333333");
            emp.DepartmentId = 2;
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest
            {
                PageNumber = 1,
                PageSize = 10,
                DepartmentIds = new List<int> { 2 }
            });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_BranchFilter_ReturnsMatchingEmployees()
        {
            _context.Branches.Add(new Branch { BranchId = 2, BranchName = "Chennai" });
            await _context.SaveChangesAsync();

            _context.Employees.Add(MakeEmployee(workEmail: "a@ergon.com", personalEmail: "a@gmail.com", phone: "1111111111"));
            var emp = MakeEmployee(workEmail: "b@ergon.com", personalEmail: "b@gmail.com", phone: "2222222222");
            emp.BranchId = 2;
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest
            {
                PageNumber = 1,
                PageSize = 10,
                BranchIds = new List<int> { 2 }
            });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_EmploymentStatusFilter_ReturnsMatchingEmployees()
        {
            _context.Employees.AddRange(
                MakeEmployee(workEmail: "a@ergon.com", personalEmail: "a@gmail.com", phone: "1111111111", status: EmploymentStatusEnum.Active),
                MakeEmployee(workEmail: "b@ergon.com", personalEmail: "b@gmail.com", phone: "2222222222", status: EmploymentStatusEnum.Resigned)
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest
            {
                PageNumber = 1,
                PageSize = 10,
                EmploymentStatuses = new List<string> { "Resigned" }
            });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_InvalidEmploymentStatus_IgnoredGracefully()
        {
            _context.Employees.Add(MakeEmployee());
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse>());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest
            {
                PageNumber = 1,
                PageSize = 10,
                EmploymentStatuses = new List<string> { "NotARealStatus" }
            });

            Assert.That(result.TotalCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllEmployees_Pagination_ReturnsCorrectPage()
        {
            for (int i = 0; i < 15; i++)
            {
                _context.Employees.Add(MakeEmployee(
                    workEmail: $"emp{i}@ergon.com",
                    personalEmail: $"emp{i}@gmail.com",
                    phone: $"900000{i:D4}"));
            }
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 2, PageSize = 5 });

            Assert.That(result.Items.Count, Is.EqualTo(5));
            Assert.That(result.TotalCount, Is.EqualTo(15));
            Assert.That(result.TotalPages, Is.EqualTo(3));
            Assert.That(result.PageNumber, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllEmployees_PageNumberExceedsTotalPages_ReturnsLastPage()
        {
            _context.Employees.Add(MakeEmployee());
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 99, PageSize = 5 });

            Assert.That(result.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllEmployees_SortDescending_ReturnsSortedResults()
        {
            _context.Employees.AddRange(
                MakeEmployee(firstName: "Arjun", workEmail: "arjun@ergon.com", personalEmail: "arjun@gmail.com", phone: "1111111111"),
                MakeEmployee(firstName: "Zara", workEmail: "zara@ergon.com", personalEmail: "zara@gmail.com", phone: "2222222222")
            );
            await _context.SaveChangesAsync();

            var capturedList = new List<Employee>();
            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Callback<object>(src => capturedList = (List<Employee>)src)
                .Returns(new List<EmployeeListResponse> { new(), new() });

            await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10, SortDirection = "desc" });

            Assert.That(capturedList.First().FirstName, Is.EqualTo("Zara"));
        }

        [Test]
        public async Task GetAllEmployees_EmptyDatabase_ReturnsEmptyResult()
        {
            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeListResponse>());

            var result = await _employeeService.GetAllEmployeesAsync(new GetAllEmployeesRequest { PageNumber = 1, PageSize = 10 });

            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.Items, Is.Empty);
        }


        [Test]
        public async Task GetMyTeam_ReturnsOnlyDirectReports()
        {
            var managerId = Guid.NewGuid();
            _context.Employees.AddRange(
                MakeEmployee(workEmail: "sub1@ergon.com", personalEmail: "sub1@gmail.com", phone: "1111111111", reportsTo: managerId),
                MakeEmployee(workEmail: "sub2@ergon.com", personalEmail: "sub2@gmail.com", phone: "2222222222", reportsTo: managerId),
                MakeEmployee(workEmail: "other@ergon.com", personalEmail: "other@gmail.com", phone: "3333333333", reportsTo: Guid.NewGuid())
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeListResponse>>(It.IsAny<List<Employee>>()))
                .Returns((List<Employee> src) => src.Select(_ => new EmployeeListResponse()).ToList());

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
            var request = MakeCreateRequest();
            var employee = MakeEmployee(id: employeeId);

            SetupMapperForCreate(request, employee, employeeId);

            var result = await _employeeService.CreateEmployeeAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TempPassword, Is.Not.Null);
            Assert.That(result.TempPassword!.Length, Is.EqualTo(8));
        }

        [Test]
        public async Task CreateEmployee_EmployeeIsCreatedInDb()
        {
            var employeeId = Guid.NewGuid();
            var request = MakeCreateRequest();
            var employee = MakeEmployee(id: employeeId);

            SetupMapperForCreate(request, employee, employeeId);

            await _employeeService.CreateEmployeeAsync(request);

            var saved = await _context.Employees.FindAsync(employeeId);
            Assert.That(saved, Is.Not.Null);
        }

        [Test]
        public void CreateEmployee_WorkEmailNotErgonDomain_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(workEmail: "new@gmail.com");

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_WorkEmailSameAsPersonalEmail_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(workEmail: "same@ergon.com", personalEmail: "same@ergon.com");

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_DuplicateWorkEmail_ThrowsConflictException()
        {
            _context.Employees.Add(MakeEmployee(workEmail: "taken@ergon.com"));
            await _context.SaveChangesAsync();

            var request = MakeCreateRequest(workEmail: "taken@ergon.com");

            Assert.ThrowsAsync<ConflictException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_DuplicatePersonalEmail_ThrowsConflictException()
        {
            _context.Employees.Add(MakeEmployee(personalEmail: "taken@gmail.com"));
            await _context.SaveChangesAsync();

            var request = MakeCreateRequest(personalEmail: "taken@gmail.com");

            Assert.ThrowsAsync<ConflictException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_DuplicatePhone_ThrowsConflictException()
        {
            _context.Employees.Add(MakeEmployee(phone: "9999999999"));
            await _context.SaveChangesAsync();

            var request = MakeCreateRequest(phone: "9999999999");

            Assert.ThrowsAsync<ConflictException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfBirthTooYoung_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)));

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfBirthTooOld_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-85)));

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfBirthExactly18_Succeeds()
        {
            var request = MakeCreateRequest(
                dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)),
                workEmail: "young@ergon.com"
            );
            var employee = MakeEmployee(workEmail: "young@ergon.com");
            SetupMapperForCreate(request, employee, employee.EmployeeId);

            Assert.DoesNotThrowAsync(() => _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfJoiningTooFarInFuture_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(dateOfJoining: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(7)));

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfJoiningTooFarInPast_ThrowsBadRequestException()
        {
            var request = MakeCreateRequest(dateOfJoining: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-81)));

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public void CreateEmployee_DateOfJoiningExactly6MonthsFuture_Succeeds()
        {
            var request = MakeCreateRequest(
                dateOfJoining: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
                workEmail: "future@ergon.com"
            );
            var employee = MakeEmployee(workEmail: "future@ergon.com");
            SetupMapperForCreate(request, employee, employee.EmployeeId);

            Assert.DoesNotThrowAsync(() => _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_InvalidManager_ThrowsNotFoundException()
        {
            var request = MakeCreateRequest(reportsTo: Guid.NewGuid());

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_ManagerWithInactiveStatus_ThrowsBadRequestException()
        {
            var manager = MakeEmployee(
                workEmail: "manager@ergon.com",
                personalEmail: "manager@gmail.com",
                phone: "8888888888",
                status: EmploymentStatusEnum.Resigned);
            _context.Employees.Add(manager);
            await _context.SaveChangesAsync();

            var request = MakeCreateRequest(reportsTo: manager.EmployeeId);

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.CreateEmployeeAsync(request));
        }

        [Test]
        public async Task CreateEmployee_ValidManager_Succeeds()
        {
            var manager = MakeEmployee(
                workEmail: "manager@ergon.com",
                personalEmail: "manager@gmail.com",
                phone: "8888888888",
                status: EmploymentStatusEnum.Active);
            _context.Employees.Add(manager);
            await _context.SaveChangesAsync();

            var employeeId = Guid.NewGuid();
            var request = MakeCreateRequest(reportsTo: manager.EmployeeId);
            var employee = MakeEmployee(id: employeeId, reportsTo: manager.EmployeeId);
            SetupMapperForCreate(request, employee, employeeId);

            Assert.DoesNotThrowAsync(() => _employeeService.CreateEmployeeAsync(request));
        }


        [Test]
        public async Task UpdateEmployee_ValidRequest_ReturnsUpdatedResponse()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest
            {
                FirstName = "Updated",
                LastName = "Name",
                PersonalEmail = "updated@gmail.com",
                Phone = "7777777777",
                AddressLine1 = "New Address",
                StateId = 1,
                CountryId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                LeaveEntitlementId = 1
            };

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId, FirstName = "Updated" });

            var result = await _employeeService.UpdateEmployeeAsync(employee.EmployeeId, request);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UpdateEmployee_EmployeeNotFound_ThrowsNotFoundException()
        {
            var request = new UpdateEmployeeRequest { PersonalEmail = "x@gmail.com", Phone = "1234567890" };

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.UpdateEmployeeAsync(Guid.NewGuid(), request));
        }

        [Test]
        public async Task UpdateEmployee_DuplicatePersonalEmail_ThrowsConflictException()
        {
            var emp1 = MakeEmployee(workEmail: "emp1@ergon.com", personalEmail: "emp1@gmail.com", phone: "1111111111");
            var emp2 = MakeEmployee(workEmail: "emp2@ergon.com", personalEmail: "emp2@gmail.com", phone: "2222222222");
            _context.Employees.AddRange(emp1, emp2);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest { PersonalEmail = "emp2@gmail.com", Phone = "9090909090" };

            Assert.ThrowsAsync<ConflictException>(() =>
                _employeeService.UpdateEmployeeAsync(emp1.EmployeeId, request));
        }

        [Test]
        public async Task UpdateEmployee_SamePersonalEmailAsSelf_Succeeds()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest
            {
                PersonalEmail = employee.PersonalEmail,
                Phone = "7777777777",
                AddressLine1 = "New Address",
                StateId = 1,
                CountryId = 1,
                DepartmentId = 1,
                BranchId = 1,
                DesignationId = 1,
                ShiftId = 1,
                SalaryStructureId = 1,
                LeaveEntitlementId = 1
            };

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId });

            Assert.DoesNotThrowAsync(() =>
                _employeeService.UpdateEmployeeAsync(employee.EmployeeId, request));
        }

        [Test]
        public async Task UpdateEmployee_DuplicatePhone_ThrowsConflictException()
        {
            var emp1 = MakeEmployee(workEmail: "emp1@ergon.com", personalEmail: "emp1@gmail.com", phone: "1111111111");
            var emp2 = MakeEmployee(workEmail: "emp2@ergon.com", personalEmail: "emp2@gmail.com", phone: "2222222222");
            _context.Employees.AddRange(emp1, emp2);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest { PersonalEmail = "new@gmail.com", Phone = "2222222222" };

            Assert.ThrowsAsync<ConflictException>(() =>
                _employeeService.UpdateEmployeeAsync(emp1.EmployeeId, request));
        }

        [Test]
        public async Task UpdateEmployee_PersonalEmailSameAsWorkEmail_ThrowsBadRequestException()
        {
            var employee = MakeEmployee(workEmail: "arjun@ergon.com", personalEmail: "arjun@gmail.com");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest { PersonalEmail = "arjun@ergon.com", Phone = "7777777777" };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.UpdateEmployeeAsync(employee.EmployeeId, request));
        }

        [Test]
        public async Task UpdateEmployee_SelfReference_ThrowsBadRequestException()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest
            {
                PersonalEmail = "other@gmail.com",
                Phone = "7777777777",
                ReportsTo = employee.EmployeeId
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.UpdateEmployeeAsync(employee.EmployeeId, request));
        }

        [Test]
        public async Task UpdateEmployee_ManagerWithInactiveStatus_ThrowsBadRequestException()
        {
            var manager = MakeEmployee(workEmail: "manager@ergon.com", personalEmail: "mgr@gmail.com", phone: "8888888888", status: EmploymentStatusEnum.Terminated);
            var employee = MakeEmployee(workEmail: "emp@ergon.com", personalEmail: "emp@gmail.com", phone: "9999999999");
            _context.Employees.AddRange(manager, employee);
            await _context.SaveChangesAsync();

            var request = new UpdateEmployeeRequest
            {
                PersonalEmail = "emp@gmail.com",
                Phone = "9999999999",
                ReportsTo = manager.EmployeeId
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.UpdateEmployeeAsync(employee.EmployeeId, request));
        }


        [Test]
        public async Task DeleteEmployee_EmployeeExists_DeletesAndReturnsResponse()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId, FirstName = employee.FirstName });

            var result = await _employeeService.DeleteEmployeeAsync(employee.EmployeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FirstName, Is.EqualTo("Arjun"));

            var deleted = await _context.Employees.FindAsync(employee.EmployeeId);
            Assert.That(deleted, Is.Null);
        }

        [Test]
        public void DeleteEmployee_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.DeleteEmployeeAsync(Guid.NewGuid()));
        }


        [Test]
        public void UpdateEmployeeStatus_EmployeeNotFound_ThrowsNotFoundException()
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
            var manager = MakeEmployee(workEmail: "mgr@ergon.com", personalEmail: "mgr@gmail.com", phone: "1111111111");
            var report = MakeEmployee(workEmail: "rep@ergon.com", personalEmail: "rep@gmail.com", phone: "2222222222", reportsTo: manager.EmployeeId);
            _context.Employees.AddRange(manager, report);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeService.UpdateEmployeeStatusAsync(manager.EmployeeId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Suspended
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_HasDirectReports_ActiveStatus_Succeeds()
        {
            var manager = MakeEmployee(workEmail: "mgr@ergon.com", personalEmail: "mgr@gmail.com", phone: "1111111111");
            var report = MakeEmployee(workEmail: "rep@ergon.com", personalEmail: "rep@gmail.com", phone: "2222222222", reportsTo: manager.EmployeeId);
            _context.Employees.AddRange(manager, report);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = manager.EmployeeId });

            Assert.DoesNotThrowAsync(() =>
                _employeeService.UpdateEmployeeStatusAsync(manager.EmployeeId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Active
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_NoDirectReports_InactiveStatus_Succeeds()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId });

            Assert.DoesNotThrowAsync(() =>
                _employeeService.UpdateEmployeeStatusAsync(employee.EmployeeId, new UpdateEmployeeStatusRequest
                {
                    EmploymentStatus = EmploymentStatusEnum.Resigned
                }));
        }

        [Test]
        public async Task UpdateEmployeeStatus_SendsNotification()
        {
            var employee = MakeEmployee();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDetailResponse>(It.IsAny<Employee>()))
                .Returns(new EmployeeDetailResponse { EmployeeId = employee.EmployeeId });

            await _employeeService.UpdateEmployeeStatusAsync(employee.EmployeeId, new UpdateEmployeeStatusRequest
            {
                EmploymentStatus = EmploymentStatusEnum.Active
            });

            _mockNotification.Verify(n => n.CreateNotificationAsync(
                employee.EmployeeId,
                "Employment Status Update",
                It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void UpdateEmployeePfp_EmployeeNotFound_ThrowsNotFoundException()
        {
            var mockFile = new Mock<IFormFile>();

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeService.UpdateEmployeePfpAsync(Guid.NewGuid(), mockFile.Object));
        }
    }
}
