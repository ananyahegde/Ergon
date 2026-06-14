using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Leave;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class LeaveServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<ILeaveRepository> _mockLeaveRepo = null!;
        private Mock<INotificationService> _mockNotification = null!;
        private Mock<IMapper> _mockMapper = null!;
        private LeaveService _leaveService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockNotification = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _leaveService = new LeaveService(_context, _mockLeaveRepo.Object, _mockNotification.Object, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetLeaveById_LeaveExists_ReturnsLeaveResponse()
        {
            var leaveId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.LeaveTypes.Add(new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" });
            _context.Leaves.Add(new Leave { LeaveId = leaveId, EmployeeId = employeeId, LeaveTypeId = 1, FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), Status = LeaveStatusEnum.Open, AppliedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<LeaveResponse>(It.IsAny<Leave>()))
                .Returns(new LeaveResponse { LeaveId = leaveId });

            var result = await _leaveService.GetLeaveByIdAsync(leaveId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetLeaveById_LeaveNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _leaveService.GetLeaveByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task ApplyLeave_ValidRequest_CreatesLeave()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.LeaveTypes.Add(new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" });
            await _context.SaveChangesAsync();

            var request = new CreateLeaveRequest
            {
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Reason = "Family function",
                IsHalfDay = false,
                LeaveTypeId = 1
            };

            _mockMapper.Setup(m => m.Map<Leave>(request)).Returns(new Leave { LeaveId = Guid.NewGuid(), LeaveTypeId = 1 });
            _mockMapper.Setup(m => m.Map<LeaveResponse>(It.IsAny<Leave>())).Returns(new LeaveResponse());

            var result = await _leaveService.ApplyLeaveAsync(employeeId, request);

            Assert.That(result, Is.Not.Null);
            Assert.That(_context.Leaves.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ApplyLeave_PastDate_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            await _context.SaveChangesAsync();

            var request = new CreateLeaveRequest
            {
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Reason = "Test",
                LeaveTypeId = 1
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_OverlappingLeave_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.Leaves.Add(new Leave
            {
                LeaveId = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeId = 1,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                Status = LeaveStatusEnum.Open,
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var request = new CreateLeaveRequest
            {
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                Reason = "Test",
                LeaveTypeId = 1
            };

            _mockMapper.Setup(m => m.Map<Leave>(request)).Returns(new Leave());

            Assert.ThrowsAsync<ConflictException>(() =>
                _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ActionLeave_ValidAction_UpdatesStatus()
        {
            var leaveId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var actionedBy = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.LeaveTypes.Add(new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" });
            _context.Leaves.Add(new Leave
            {
                LeaveId = leaveId,
                EmployeeId = employeeId,
                LeaveTypeId = 1,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Status = LeaveStatusEnum.Open,
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<LeaveResponse>(It.IsAny<Leave>())).Returns(new LeaveResponse());

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Approved };
            var result = await _leaveService.ActionLeaveAsync(leaveId, actionedBy, request);

            var updated = await _context.Leaves.FindAsync(leaveId);
            Assert.That(updated!.Status, Is.EqualTo(LeaveStatusEnum.Approved));
        }

        [Test]
        public async Task ActionLeave_AlreadyActioned_ThrowsBadRequestException()
        {
            var leaveId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.Leaves.Add(new Leave
            {
                LeaveId = leaveId,
                EmployeeId = employeeId,
                LeaveTypeId = 1,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Status = LeaveStatusEnum.Approved,
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _leaveService.ActionLeaveAsync(leaveId, Guid.NewGuid(), new LeaveActionRequest { Action = LeaveStatusEnum.Rejected }));
        }

        [Test]
        public async Task CancelLeave_ValidRequest_CancelsLeave()
        {
            var leaveId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.LeaveTypes.Add(new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" });
            _context.Leaves.Add(new Leave
            {
                LeaveId = leaveId,
                EmployeeId = employeeId,
                LeaveTypeId = 1,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Status = LeaveStatusEnum.Open,
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<LeaveResponse>(It.IsAny<Leave>())).Returns(new LeaveResponse());

            var result = await _leaveService.CancelLeaveAsync(leaveId, employeeId);

            var updated = await _context.Leaves.FindAsync(leaveId);
            Assert.That(updated!.Status, Is.EqualTo(LeaveStatusEnum.Cancelled));
        }

        [Test]
        public async Task CancelLeave_WrongEmployee_ThrowsForbiddenException()
        {
            var leaveId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _context.Leaves.Add(new Leave
            {
                LeaveId = leaveId,
                EmployeeId = employeeId,
                LeaveTypeId = 1,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Status = LeaveStatusEnum.Open,
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<ForbiddenException>(() =>
                _leaveService.CancelLeaveAsync(leaveId, Guid.NewGuid()));
        }

        [Test]
        public async Task GetLeaveBalances_ReturnsBalances()
        {
            var balances = new List<LeaveBalanceResponse> { new() { LeaveTypeName = "Casual Leave", TotalLeaves = 12, UsedLeaves = 3, RemainingLeaves = 9 } };
            _mockLeaveRepo.Setup(r => r.GetLeaveBalancesAsync()).ReturnsAsync(balances);

            var result = await _leaveService.GetLeaveBalancesAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
