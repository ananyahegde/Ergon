using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Attendance;
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
    public class AttendanceServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IAttendanceRepository> _mockAttendanceRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private AttendanceService _attendanceService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);

            _mockAttendanceRepo = new Mock<IAttendanceRepository>();
            _mockMapper = new Mock<IMapper>();

            _attendanceService = new AttendanceService(
                _mockAttendanceRepo.Object,
                _mockMapper.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAttendances_ValidRequest_ReturnsPagedResponse()
        {
            var request = new GetAllAttendancesRequest { PageSize = 10, PageNumber = 1 };
            var attendances = new List<Attendance> { new() { AttendanceId = Guid.NewGuid() } };

            _mockAttendanceRepo.Setup(r => r.GetPagedAttendancesAsync(request)).ReturnsAsync((attendances, 1));
            _mockMapper.Setup(m => m.Map<List<AttendanceResponse>>(It.IsAny<List<Attendance>>()))
                .Returns(new List<AttendanceResponse> { new() });

            var result = await _attendanceService.GetAllAttendancesAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.PageNumber, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAttendanceById_ExistingId_ReturnsResponse()
        {
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance { AttendanceId = attendanceId };

            _mockAttendanceRepo.Setup(r => r.GetAttendanceByIdAsync(attendanceId)).ReturnsAsync(attendance);
            _mockMapper.Setup(m => m.Map<AttendanceResponse>(attendance)).Returns(new AttendanceResponse { AttendanceId = attendanceId });

            var result = await _attendanceService.GetAttendanceByIdAsync(attendanceId);

            Assert.That(result.AttendanceId, Is.EqualTo(attendanceId));
        }

        [Test]
        public void GetAttendanceById_NotFound_ThrowsNotFoundException()
        {
            _mockAttendanceRepo.Setup(r => r.GetAttendanceByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Attendance?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _attendanceService.GetAttendanceByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task ClockIn_ValidRequest_CreatesAttendanceAndReturnsResponse()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmploymentStatus = EmploymentStatusEnum.Active,
                Shift = new Shift { StartTime = new TimeOnly(9, 0) }
            };

            _mockAttendanceRepo.Setup(r => r.GetEmployeeWithShiftAsync(employeeId)).ReturnsAsync(employee);
            _mockAttendanceRepo.Setup(r => r.GetAttendanceForDateAsync(employeeId, It.IsAny<DateOnly>())).ReturnsAsync((Attendance?)null);
            _mockMapper.Setup(m => m.Map<AttendanceResponse>(It.IsAny<Attendance>())).Returns(new AttendanceResponse { EmployeeId = employeeId });

            var result = await _attendanceService.ClockInAsync(employeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployeeId, Is.EqualTo(employeeId));
            _mockAttendanceRepo.Verify(r => r.AddAttendanceAsync(It.IsAny<Attendance>()), Times.Once);
            _mockAttendanceRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void ClockIn_EmployeeNotFound_ThrowsNotFoundException()
        {
            _mockAttendanceRepo.Setup(r => r.GetEmployeeWithShiftAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _attendanceService.ClockInAsync(Guid.NewGuid()));
        }

        [Test]
        public void ClockIn_InactiveEmployee_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee { EmployeeId = employeeId, EmploymentStatus = EmploymentStatusEnum.Resigned };
            _mockAttendanceRepo.Setup(r => r.GetEmployeeWithShiftAsync(employeeId)).ReturnsAsync(employee);

            Assert.ThrowsAsync<BadRequestException>(() => _attendanceService.ClockInAsync(employeeId));
        }

        [Test]
        public void ClockIn_AlreadyClockedInToday_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmploymentStatus = EmploymentStatusEnum.Active,
                Shift = new Shift { StartTime = new TimeOnly(9, 0) }
            };
            var existing = new Attendance { AttendanceId = Guid.NewGuid(), EmployeeId = employeeId };

            _mockAttendanceRepo.Setup(r => r.GetEmployeeWithShiftAsync(employeeId)).ReturnsAsync(employee);
            _mockAttendanceRepo.Setup(r => r.GetAttendanceForDateAsync(employeeId, It.IsAny<DateOnly>())).ReturnsAsync(existing);

            Assert.ThrowsAsync<ConflictException>(() => _attendanceService.ClockInAsync(employeeId));
        }

        [Test]
        public async Task ClockOut_FullDayHours_SetsPresentStatus()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                ClockInTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(-9),
                Employee = new Employee { Shift = new Shift { EndTime = new TimeOnly(18, 0) } }
            };

            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(attendanceId)).ReturnsAsync(attendance);
            _mockMapper.Setup(m => m.Map<AttendanceResponse>(attendance)).Returns(new AttendanceResponse { AttendanceId = attendanceId });

            var result = await _attendanceService.ClockOutAsync(attendanceId, employeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(attendance.AttendanceStatus, Is.EqualTo(AttendanceStatusEnum.Present));
            _mockAttendanceRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ClockOut_HalfDayHours_SetsHalfDayStatus()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                ClockInTime = TimeOnly.FromDateTime(DateTime.Now).AddHours(-2),
                Employee = new Employee { Shift = new Shift { EndTime = new TimeOnly(18, 0) } }
            };

            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(attendanceId)).ReturnsAsync(attendance);
            _mockMapper.Setup(m => m.Map<AttendanceResponse>(attendance)).Returns(new AttendanceResponse { AttendanceId = attendanceId });

            await _attendanceService.ClockOutAsync(attendanceId, employeeId);

            Assert.That(attendance.AttendanceStatus, Is.EqualTo(AttendanceStatusEnum.HalfDay));
        }

        [Test]
        public async Task ClockOut_UnderOneHour_SetsAbsentStatus()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                ClockInTime = TimeOnly.FromDateTime(DateTime.Now).AddMinutes(-30),
                Employee = new Employee { Shift = new Shift { EndTime = new TimeOnly(18, 0) } }
            };

            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(attendanceId)).ReturnsAsync(attendance);
            _mockMapper.Setup(m => m.Map<AttendanceResponse>(attendance)).Returns(new AttendanceResponse { AttendanceId = attendanceId });

            await _attendanceService.ClockOutAsync(attendanceId, employeeId);

            Assert.That(attendance.AttendanceStatus, Is.EqualTo(AttendanceStatusEnum.Absent));
        }

        [Test]
        public void ClockOut_NotFound_ThrowsNotFoundException()
        {
            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(It.IsAny<Guid>())).ReturnsAsync((Attendance?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _attendanceService.ClockOutAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Test]
        public void ClockOut_WrongEmployee_ThrowsForbiddenException()
        {
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance { AttendanceId = attendanceId, EmployeeId = Guid.NewGuid() };
            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(attendanceId)).ReturnsAsync(attendance);

            Assert.ThrowsAsync<ForbiddenException>(() => _attendanceService.ClockOutAsync(attendanceId, Guid.NewGuid()));
        }

        [Test]
        public void ClockOut_AlreadyClockedOut_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            var attendance = new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                ClockOutTime = new TimeOnly(18, 0)
            };
            _mockAttendanceRepo.Setup(r => r.GetAttendanceWithShiftAsync(attendanceId)).ReturnsAsync(attendance);

            Assert.ThrowsAsync<BadRequestException>(() => _attendanceService.ClockOutAsync(attendanceId, employeeId));
        }

        [Test]
        public async Task GetTodaySummary_InvokesRepository_ReturnsSummary()
        {
            var expectedSummary = new AttendanceTodaySummaryResponse { TotalPresent = 5, TotalAbsent = 2 };
            _mockAttendanceRepo.Setup(r => r.GetTodaySummaryAsync()).ReturnsAsync(expectedSummary);

            var result = await _attendanceService.GetTodaySummaryAsync();

            Assert.That(result, Is.EqualTo(expectedSummary));
            _mockAttendanceRepo.Verify(r => r.GetTodaySummaryAsync(), Times.Once);
        }
    }
}
