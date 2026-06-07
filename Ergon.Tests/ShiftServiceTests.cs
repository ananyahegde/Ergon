using AutoMapper;
using Ergon.DTOs.Shift;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class ShiftServiceTests
    {
        private Mock<IRepository<int, Shift>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private ShiftService _shiftService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Shift>>();
            _mockMapper = new Mock<IMapper>();
            _shiftService = new ShiftService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetShiftById_ShiftExists_ReturnsShiftResponse()
        {
            var shift = new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) };
            var shiftResponse = new ShiftResponse { ShiftId = 1, ShiftName = "Morning" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(shift);
            _mockMapper.Setup(m => m.Map<ShiftResponse>(shift)).Returns(shiftResponse);

            var result = await _shiftService.GetShiftByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ShiftName, Is.EqualTo("Morning"));
        }

        [Test]
        public async Task GetShiftById_ShiftNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Shift?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _shiftService.GetShiftByIdAsync(1));
        }

        [Test]
        public async Task CreateShift_ValidRequest_ReturnsShiftResponse()
        {
            var request = new CreateShiftRequest { ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) };
            var shift = new Shift { ShiftId = 1, ShiftName = "Morning" };
            var shiftResponse = new ShiftResponse { ShiftId = 1, ShiftName = "Morning" };
            _mockMapper.Setup(m => m.Map<Shift>(request)).Returns(shift);
            _mockRepo.Setup(r => r.Create(shift)).ReturnsAsync(shift);
            _mockMapper.Setup(m => m.Map<ShiftResponse>(shift)).Returns(shiftResponse);

            var result = await _shiftService.CreateShiftAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ShiftName, Is.EqualTo("Morning"));
        }

        [Test]
        public async Task UpdateShift_ShiftNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Shift?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _shiftService.UpdateShiftAsync(1, new UpdateShiftRequest()));
        }

        [Test]
        public async Task DeleteShift_ShiftNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Shift?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _shiftService.DeleteShiftAsync(1));
        }
    }
}
