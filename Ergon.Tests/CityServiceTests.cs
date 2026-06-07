using AutoMapper;
using Ergon.DTOs.City;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class CityServiceTests
    {
        private Mock<IRepository<int, City>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private CityService _cityService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, City>>();
            _mockMapper = new Mock<IMapper>();
            _cityService = new CityService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetCityById_CityExists_ReturnsCityResponse()
        {
            var city = new City { CityId = 1, CityName = "Bangalore" };
            var cityResponse = new CityResponse { CityId = 1, CityName = "Bangalore" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(city);
            _mockMapper.Setup(m => m.Map<CityResponse>(city)).Returns(cityResponse);

            var result = await _cityService.GetCityByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CityName, Is.EqualTo("Bangalore"));
        }

        [Test]
        public async Task GetCityById_CityNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((City?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _cityService.GetCityByIdAsync(1));
        }

        [Test]
        public async Task CreateCity_ValidRequest_ReturnsCityResponse()
        {
            var request = new CreateCityRequest { CityName = "Bangalore" };
            var city = new City { CityId = 1, CityName = "Bangalore" };
            var cityResponse = new CityResponse { CityId = 1, CityName = "Bangalore" };
            _mockMapper.Setup(m => m.Map<City>(request)).Returns(city);
            _mockRepo.Setup(r => r.Create(city)).ReturnsAsync(city);
            _mockMapper.Setup(m => m.Map<CityResponse>(city)).Returns(cityResponse);

            var result = await _cityService.CreateCityAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CityName, Is.EqualTo("Bangalore"));
        }

        [Test]
        public async Task UpdateCity_CityNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((City?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _cityService.UpdateCityAsync(1, new UpdateCityRequest()));
        }

        [Test]
        public async Task DeleteCity_CityNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((City?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _cityService.DeleteCityAsync(1));
        }
    }
}
