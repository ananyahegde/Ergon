using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Leave;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Ergon.Tests
{
    [TestFixture]
    public class LeaveServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IRepository<Guid, Leave>> _mockGenericRepo = null!;
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

            _mockGenericRepo = new Mock<IRepository<Guid, Leave>>();
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockNotification = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();

            _leaveService = new LeaveService(
                _mockGenericRepo.Object,
                _mockLeaveRepo.Object,
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
        public async Task GetAllLeaves_ValidRequest_ReturnsPagedResponse()
        {
            var request = new GetAllLeavesRequest { PageSize = 10, PageNumber = 1 };
            var leaves = new List<Leave> { new() { LeaveId = Guid.NewGuid() } };

            _mockLeaveRepo.Setup(r => r.GetAllAsync(request)).ReturnsAsync((leaves, 1));
            _mockMapper.Setup(m => m.Map<List<LeaveResponse>>(It.IsAny<List<Leave>>()))
                .Returns(new List<LeaveResponse> { new() });

            var result = await _leaveService.GetAllLeavesAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.PageNumber, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllLeaves_PageNumberExceedsTotalPages_AdjustsToLastPage()
        {
            var request = new GetAllLeavesRequest { PageSize = 5, PageNumber = 5 };
            var leaves = new List<Leave> { new(), new() };

            _mockLeaveRepo.Setup(r => r.GetAllAsync(request)).ReturnsAsync((leaves, 2));
            _mockMapper.Setup(m => m.Map<List<LeaveResponse>>(It.IsAny<List<Leave>>()))
                .Returns(new List<LeaveResponse> { new(), new() });

            var result = await _leaveService.GetAllLeavesAsync(request);

            Assert.That(result.PageNumber, Is.EqualTo(1));
        }

        [Test]
        public async Task GetLeaveById_LeaveExists_ReturnsLeaveResponse()
        {
            var leaveId = Guid.NewGuid();
            var leave = new Leave { LeaveId = leaveId };

            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leaveId)).ReturnsAsync(leave);
            _mockMapper.Setup(m => m.Map<LeaveResponse>(leave)).Returns(new LeaveResponse { LeaveId = leaveId });

            var result = await _leaveService.GetLeaveByIdAsync(leaveId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LeaveId, Is.EqualTo(leaveId));
        }

        [Test]
        public async Task GetLeaveById_LeaveNotFound_ThrowsNotFoundException()
        {
            var missingLeaveId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(missingLeaveId)).ReturnsAsync((Leave)null!);

            Assert.ThrowsAsync<NotFoundException>(() => _leaveService.GetLeaveByIdAsync(missingLeaveId));
        }

        [Test]
        public async Task ApplyLeave_EmployeeInactive_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(false);

            var request = new CreateLeaveRequest { FromDate = DateOnly.FromDateTime(DateTime.UtcNow), ToDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_ToDateBeforeFromDate_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);

            var request = new CreateLeaveRequest
            {
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                ToDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_PastFromDate_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);

            var request = new CreateLeaveRequest
            {
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                ToDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_HalfDayMultiDays_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);

            var request = new CreateLeaveRequest
            {
                IsHalfDay = true,
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2))
            };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_HalfDayCountTwoOrMore_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.CountHalfDaysOnDateAsync(employeeId, targetDate)).ReturnsAsync(2);

            var request = new CreateLeaveRequest { IsHalfDay = true, FromDate = targetDate, ToDate = targetDate };

            Assert.ThrowsAsync<ConflictException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_HalfDayAlreadyStartedToday_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.CountHalfDaysOnDateAsync(employeeId, today)).ReturnsAsync(1);

            var request = new CreateLeaveRequest { IsHalfDay = true, FromDate = today, ToDate = today };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_FullDayOverlapping_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.HasOverlappingLeaveAsync(employeeId, fromDate, toDate, It.IsAny<Guid?>())).ReturnsAsync(true);

            var request = new CreateLeaveRequest { IsHalfDay = false, FromDate = fromDate, ToDate = toDate };

            Assert.ThrowsAsync<ConflictException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_EntitlementNotFound_ThrowsNotFoundException()
        {
            var employeeId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.GetEmployeeLeaveEntitlementIdAsync(employeeId)).ReturnsAsync((int?)null);

            var request = new CreateLeaveRequest
            {
                IsHalfDay = false,
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            };

            Assert.ThrowsAsync<NotFoundException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_InsufficientBalance_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.GetEmployeeLeaveEntitlementIdAsync(employeeId)).ReturnsAsync(10);
            _mockLeaveRepo.Setup(r => r.GetEntitlementDaysAsync(10, 1)).ReturnsAsync(5.0m);
            _mockLeaveRepo.Setup(r => r.GetUsedLeaveDaysAsync(employeeId, 1)).ReturnsAsync(3.0m);

            var request = new CreateLeaveRequest { IsHalfDay = false, FromDate = fromDate, ToDate = toDate, LeaveTypeId = 1 };

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ApplyLeaveAsync(employeeId, request));
        }

        [Test]
        public async Task ApplyLeave_ValidRequest_SavesToDatabaseAndReturnsResponse()
        {
            var employeeId = Guid.NewGuid();
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var request = new CreateLeaveRequest { IsHalfDay = false, FromDate = fromDate, ToDate = toDate, LeaveTypeId = 1 };
            var leaveEntity = new Leave { LeaveId = Guid.NewGuid() };

            _mockLeaveRepo.Setup(r => r.EmployeeExistsAndActiveAsync(employeeId)).ReturnsAsync(true);
            _mockLeaveRepo.Setup(r => r.GetEmployeeLeaveEntitlementIdAsync(employeeId)).ReturnsAsync(10);
            _mockLeaveRepo.Setup(r => r.GetEntitlementDaysAsync(10, 1)).ReturnsAsync(10.0m);
            _mockLeaveRepo.Setup(r => r.GetUsedLeaveDaysAsync(employeeId, 1)).ReturnsAsync(0.0m);
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leaveEntity);
            _mockMapper.Setup(m => m.Map<Leave>(request)).Returns(leaveEntity);
            _mockMapper.Setup(m => m.Map<LeaveResponse>(leaveEntity)).Returns(new LeaveResponse { LeaveId = leaveEntity.LeaveId });

            var result = await _leaveService.ApplyLeaveAsync(employeeId, request);

            Assert.That(result, Is.Not.Null);
            _mockGenericRepo.Verify(r => r.Create(It.IsAny<Leave>()), Times.Once);
        }

        [Test]
        public async Task ActionLeave_InvalidStatusAction_ThrowsBadRequestException()
        {
            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Cancelled };
            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ActionLeaveAsync(Guid.NewGuid(), Guid.NewGuid(), request));
        }

        [Test]
        public async Task ActionLeave_ActorNotFound_ThrowsNotFoundException()
        {
            var actionedBy = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.GetActioningEmployeeAsync(actionedBy)).ReturnsAsync((Employee)null!);

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Approved };
            Assert.ThrowsAsync<NotFoundException>(() => _leaveService.ActionLeaveAsync(Guid.NewGuid(), actionedBy, request));
        }

        [Test]
        public async Task ActionLeave_ActorNotHR_ThrowsForbiddenException()
        {
            var actionedBy = Guid.NewGuid();
            var actor = new Employee { EmployeeId = actionedBy, RoleId = 3 };
            _mockLeaveRepo.Setup(r => r.GetActioningEmployeeAsync(actionedBy)).ReturnsAsync(actor);

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Approved };
            Assert.ThrowsAsync<ForbiddenException>(() => _leaveService.ActionLeaveAsync(Guid.NewGuid(), actionedBy, request));
        }

        [Test]
        public async Task ActionLeave_LeaveNotFound_ThrowsNotFoundException()
        {
            var actionedBy = Guid.NewGuid();
            var actor = new Employee { EmployeeId = actionedBy, RoleId = 1 };
            _mockLeaveRepo.Setup(r => r.GetActioningEmployeeAsync(actionedBy)).ReturnsAsync(actor);

            var missingLeaveId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(missingLeaveId)).ReturnsAsync((Leave)null!);

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Approved };
            Assert.ThrowsAsync<NotFoundException>(() => _leaveService.ActionLeaveAsync(missingLeaveId, actionedBy, request));
        }

        [Test]
        public async Task ActionLeave_LeaveNotOpen_ThrowsBadRequestException()
        {
            var actionedBy = Guid.NewGuid();
            var leaveId = Guid.NewGuid();
            var actor = new Employee { EmployeeId = actionedBy, RoleId = 1 };
            var leave = new Leave { LeaveId = leaveId, Status = LeaveStatusEnum.Approved };

            _mockLeaveRepo.Setup(r => r.GetActioningEmployeeAsync(actionedBy)).ReturnsAsync(actor);
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leaveId)).ReturnsAsync(leave);

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Rejected };
            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.ActionLeaveAsync(leaveId, actionedBy, request));
        }

        [Test]
        public async Task ActionLeave_ValidRequest_UpdatesStatusAndSendsNotification()
        {
            var actionedBy = Guid.NewGuid();
            var leaveId = Guid.NewGuid();
            var actor = new Employee { EmployeeId = actionedBy, RoleId = 1 };
            var leave = new Leave { LeaveId = leaveId, EmployeeId = Guid.NewGuid(), Status = LeaveStatusEnum.Open };

            _mockLeaveRepo.Setup(r => r.GetActioningEmployeeAsync(actionedBy)).ReturnsAsync(actor);
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leaveId)).ReturnsAsync(leave);
            _mockMapper.Setup(m => m.Map<LeaveResponse>(leave)).Returns(new LeaveResponse { Status = "Approved" });

            var request = new LeaveActionRequest { Action = LeaveStatusEnum.Approved };
            var result = await _leaveService.ActionLeaveAsync(leaveId, actionedBy, request);

            Assert.That(result, Is.Not.Null);
            _mockGenericRepo.Verify(r => r.Update(leaveId, leave), Times.Once);
            _mockNotification.Verify(n => n.CreateNotificationAsync(leave.EmployeeId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CancelLeave_LeaveNotFound_ThrowsNotFoundException()
        {
            var missingLeaveId = Guid.NewGuid();
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(missingLeaveId)).ReturnsAsync((Leave)null!);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveService.CancelLeaveAsync(missingLeaveId, Guid.NewGuid()));
        }

        [Test]
        public async Task CancelLeave_WrongEmployee_ThrowsForbiddenException()
        {
            var leave = new Leave { LeaveId = Guid.NewGuid(), EmployeeId = Guid.NewGuid() };
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leave.LeaveId)).ReturnsAsync(leave);

            Assert.ThrowsAsync<ForbiddenException>(() => _leaveService.CancelLeaveAsync(leave.LeaveId, Guid.NewGuid()));
        }

        [Test]
        public async Task CancelLeave_AlreadyRejectedOrCancelled_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var leave = new Leave { LeaveId = Guid.NewGuid(), EmployeeId = employeeId, Status = LeaveStatusEnum.Rejected };
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leave.LeaveId)).ReturnsAsync(leave);

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.CancelLeaveAsync(leave.LeaveId, employeeId));
        }

        [Test]
        public async Task CancelLeave_LeaveAlreadyStarted_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var leave = new Leave
            {
                LeaveId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Status = LeaveStatusEnum.Open,
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
            };
            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leave.LeaveId)).ReturnsAsync(leave);

            Assert.ThrowsAsync<BadRequestException>(() => _leaveService.CancelLeaveAsync(leave.LeaveId, employeeId));
        }

        [Test]
        public async Task CancelLeave_ValidRequest_CancelsLeaveSuccessfully()
        {
            var employeeId = Guid.NewGuid();
            var leave = new Leave
            {
                LeaveId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Status = LeaveStatusEnum.Open,
                FromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
            };

            _mockLeaveRepo.Setup(r => r.GetByIdAsync(leave.LeaveId)).ReturnsAsync(leave);
            _mockMapper.Setup(m => m.Map<LeaveResponse>(leave)).Returns(new LeaveResponse { Status = "Cancelled" });

            var result = await _leaveService.CancelLeaveAsync(leave.LeaveId, employeeId);

            Assert.That(result, Is.Not.Null);
            _mockGenericRepo.Verify(r => r.Update(leave.LeaveId, leave), Times.Once);
            Assert.That(leave.Status, Is.EqualTo(LeaveStatusEnum.Cancelled));
        }

        [Test]
        public async Task GetLeaveBalances_InvokesRepository_ReturnsCollection()
        {
            var expectedBalances = new List<LeaveBalanceResponse> { new() };
            _mockLeaveRepo.Setup(r => r.GetLeaveBalancesAsync()).ReturnsAsync(expectedBalances);

            var result = await _leaveService.GetLeaveBalancesAsync();

            Assert.That(result, Is.EqualTo(expectedBalances));
            _mockLeaveRepo.Verify(r => r.GetLeaveBalancesAsync(), Times.Once);
        }
    }
}
