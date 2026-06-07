using AutoMapper;
using Ergon.DTOs.Country;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class CountryServiceTests
    {
        private Mock<IRepository<int, Country>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private CountryService _countryService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Country>>();
            _mockMapper = new Mock<IMapper>();
            _countryService = new CountryService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetCountryById_CountryExists_ReturnsCountryResponse()
        {
            var country = new Country { CountryId = 1, CountryName = "India" };
            var countryResponse = new CountryResponse { CountryId = 1, CountryName = "India" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(country);
            _mockMapper.Setup(m => m.Map<CountryResponse>(country)).Returns(countryResponse);

            var result = await _countryService.GetCountryByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CountryName, Is.EqualTo("India"));
        }

        [Test]
        public async Task GetCountryById_CountryNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Country?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _countryService.GetCountryByIdAsync(1));
        }

        [Test]
        public async Task CreateCountry_ValidRequest_ReturnsCountryResponse()
        {
            var request = new CreateCountryRequest { CountryName = "India" };
            var country = new Country { CountryId = 1, CountryName = "India" };
            var countryResponse = new CountryResponse { CountryId = 1, CountryName = "India" };
            _mockMapper.Setup(m => m.Map<Country>(request)).Returns(country);
            _mockRepo.Setup(r => r.Create(country)).ReturnsAsync(country);
            _mockMapper.Setup(m => m.Map<CountryResponse>(country)).Returns(countryResponse);

            var result = await _countryService.CreateCountryAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CountryName, Is.EqualTo("India"));
        }

        [Test]
        public async Task UpdateCountry_CountryNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Country?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _countryService.UpdateCountryAsync(1, new UpdateCountryRequest()));
        }

        [Test]
        public async Task DeleteCountry_CountryNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Country?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _countryService.DeleteCountryAsync(1));
        }
    }
}
