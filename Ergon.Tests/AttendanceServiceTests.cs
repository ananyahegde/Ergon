using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Attendance;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
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
            _attendanceService = new AttendanceService(_context, _mockAttendanceRepo.Object, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAttendanceById_AttendanceExists_ReturnsAttendanceResponse()
        {
            var attendanceId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.Attendances.Add(new Attendance { AttendanceId = attendanceId, EmployeeId = employeeId, Date = DateOnly.FromDateTime(DateTime.Now), ClockInTime = new TimeOnly(9, 0) });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<AttendanceResponse>(It.IsAny<Attendance>()))
                .Returns(new AttendanceResponse { AttendanceId = attendanceId });

            var result = await _attendanceService.GetAttendanceByIdAsync(attendanceId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAttendanceById_AttendanceNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _attendanceService.GetAttendanceByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task ClockIn_ValidEmployee_CreatesAttendanceRecord()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                ShiftId = 1,
                Shift = new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) }
            });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<AttendanceResponse>(It.IsAny<Attendance>()))
                .Returns(new AttendanceResponse());

            var result = await _attendanceService.ClockInAsync(employeeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(_context.Attendances.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ClockIn_AlreadyClockedIn_ThrowsConflictException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                ShiftId = 1,
                Shift = new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) }
            });
            _context.Attendances.Add(new Attendance
            {
                AttendanceId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                ClockInTime = new TimeOnly(9, 0)
            });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<ConflictException>(() =>
                _attendanceService.ClockInAsync(employeeId));
        }

        [Test]
        public async Task ClockIn_EmployeeNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _attendanceService.ClockInAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task ClockOut_ValidAttendance_UpdatesRecord()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            _context.Employees.Add(new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                ShiftId = 1,
                Shift = new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) }
            });
            _context.Attendances.Add(new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                ClockInTime = new TimeOnly(9, 0),
                AttendanceStatus = AttendanceStatusEnum.Incomplete
            });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<AttendanceResponse>(It.IsAny<Attendance>()))
                .Returns(new AttendanceResponse { AttendanceId = attendanceId });

            var result = await _attendanceService.ClockOutAsync(attendanceId, employeeId);

            Assert.That(result, Is.Not.Null);
            var updated = await _context.Attendances.FindAsync(attendanceId);
            Assert.That(updated!.ClockOutTime, Is.Not.Null);
        }

        [Test]
        public async Task ClockOut_AttendanceNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _attendanceService.ClockOutAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Test]
        public async Task ClockOut_WrongEmployee_ThrowsForbiddenException()
        {
            var employeeId = Guid.NewGuid();
            var attendanceId = Guid.NewGuid();
            _context.Employees.Add(new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Arjun",
                LastName = "Nair",
                ShiftId = 1,
                Shift = new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) }
            });
            _context.Attendances.Add(new Attendance
            {
                AttendanceId = attendanceId,
                EmployeeId = employeeId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                ClockInTime = new TimeOnly(9, 0)
            });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<ForbiddenException>(() =>
                _attendanceService.ClockOutAsync(attendanceId, Guid.NewGuid()));
        }

        [Test]
        public async Task GetTodaySummary_ReturnsSummary()
        {
            var summary = new AttendanceTodaySummaryResponse { TotalPresent = 5, TotalAbsent = 2 };
            _mockAttendanceRepo.Setup(r => r.GetTodaySummaryAsync()).ReturnsAsync(summary);

            var result = await _attendanceService.GetTodaySummaryAsync();

            Assert.That(result.TotalPresent, Is.EqualTo(5));
            Assert.That(result.TotalAbsent, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAttendances_ReturnsPagedResult()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            _context.Attendances.AddRange(
                new Attendance { AttendanceId = Guid.NewGuid(), EmployeeId = employeeId, Date = DateOnly.FromDateTime(DateTime.Now), ClockInTime = new TimeOnly(9, 0) },
                new Attendance { AttendanceId = Guid.NewGuid(), EmployeeId = employeeId, Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), ClockInTime = new TimeOnly(9, 0) }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<AttendanceResponse>>(It.IsAny<List<Attendance>>()))
                .Returns(new List<AttendanceResponse> { new(), new() });

            var request = new GetAllAttendancesRequest { PageNumber = 1, PageSize = 10, EmployeeId = employeeId };
            var result = await _attendanceService.GetAllAttendancesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(2));
        }
    }
}
