using AutoMapper;
using Ergon.DTOs.PublicHoliday;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class PublicHolidayServiceTests
    {
        private Mock<IRepository<int, PublicHoliday>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private PublicHolidayService _publicHolidayService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, PublicHoliday>>();
            _mockMapper = new Mock<IMapper>();
            _publicHolidayService = new PublicHolidayService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetPublicHolidayById_PublicHolidayExists_ReturnsPublicHolidayResponse()
        {
            var publicHoliday = new PublicHoliday { PublicHolidayId = 1, PublicHolidayName = "Diwali", PublicHolidayDate = new DateOnly(2026, 10, 20) };
            var publicHolidayResponse = new PublicHolidayResponse { PublicHolidayId = 1, PublicHolidayName = "Diwali" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(publicHoliday);
            _mockMapper.Setup(m => m.Map<PublicHolidayResponse>(publicHoliday)).Returns(publicHolidayResponse);

            var result = await _publicHolidayService.GetPublicHolidayByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PublicHolidayName, Is.EqualTo("Diwali"));
        }

        [Test]
        public async Task GetPublicHolidayById_PublicHolidayNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((PublicHoliday?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _publicHolidayService.GetPublicHolidayByIdAsync(1));
        }

        [Test]
        public async Task CreatePublicHoliday_ValidRequest_ReturnsPublicHolidayResponse()
        {
            var request = new CreatePublicHolidayRequest { PublicHolidayName = "Diwali", PublicHolidayDate = new DateOnly(2026, 10, 20) };
            var publicHoliday = new PublicHoliday { PublicHolidayId = 1, PublicHolidayName = "Diwali" };
            var publicHolidayResponse = new PublicHolidayResponse { PublicHolidayId = 1, PublicHolidayName = "Diwali" };
            _mockMapper.Setup(m => m.Map<PublicHoliday>(request)).Returns(publicHoliday);
            _mockRepo.Setup(r => r.Create(publicHoliday)).ReturnsAsync(publicHoliday);
            _mockMapper.Setup(m => m.Map<PublicHolidayResponse>(publicHoliday)).Returns(publicHolidayResponse);

            var result = await _publicHolidayService.CreatePublicHolidayAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PublicHolidayName, Is.EqualTo("Diwali"));
        }

        [Test]
        public async Task UpdatePublicHoliday_PublicHolidayNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((PublicHoliday?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _publicHolidayService.UpdatePublicHolidayAsync(1, new UpdatePublicHolidayRequest()));
        }

        [Test]
        public async Task DeletePublicHoliday_PublicHolidayNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((PublicHoliday?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _publicHolidayService.DeletePublicHolidayAsync(1));
        }
    }
}
