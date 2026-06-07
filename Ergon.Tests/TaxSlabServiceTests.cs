using AutoMapper;
using Ergon.DTOs.TaxSlab;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class TaxSlabServiceTests
    {
        private Mock<IRepository<int, TaxSlab>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private TaxSlabService _taxSlabService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, TaxSlab>>();
            _mockMapper = new Mock<IMapper>();
            _taxSlabService = new TaxSlabService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetTaxSlabById_TaxSlabExists_ReturnsTaxSlabResponse()
        {
            var taxSlab = new TaxSlab { TaxSlabId = 1, MinIncome = 0, MaxIncome = 250000, TaxPercentage = 0 };
            var taxSlabResponse = new TaxSlabResponse { TaxSlabId = 1, MinIncome = 0, MaxIncome = 250000, TaxPercentage = 0 };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(taxSlab);
            _mockMapper.Setup(m => m.Map<TaxSlabResponse>(taxSlab)).Returns(taxSlabResponse);

            var result = await _taxSlabService.GetTaxSlabByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TaxSlabId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetTaxSlabById_TaxSlabNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((TaxSlab?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _taxSlabService.GetTaxSlabByIdAsync(1));
        }

        [Test]
        public async Task CreateTaxSlab_ValidRequest_ReturnsTaxSlabResponse()
        {
            var request = new CreateTaxSlabRequest { MinIncome = 0, MaxIncome = 250000, TaxPercentage = 0 };
            var taxSlab = new TaxSlab { TaxSlabId = 1, MinIncome = 0, MaxIncome = 250000, TaxPercentage = 0 };
            var taxSlabResponse = new TaxSlabResponse { TaxSlabId = 1, MinIncome = 0, MaxIncome = 250000, TaxPercentage = 0 };
            _mockMapper.Setup(m => m.Map<TaxSlab>(request)).Returns(taxSlab);
            _mockRepo.Setup(r => r.Create(taxSlab)).ReturnsAsync(taxSlab);
            _mockMapper.Setup(m => m.Map<TaxSlabResponse>(taxSlab)).Returns(taxSlabResponse);

            var result = await _taxSlabService.CreateTaxSlabAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TaxSlabId, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateTaxSlab_TaxSlabNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((TaxSlab?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _taxSlabService.UpdateTaxSlabAsync(1, new UpdateTaxSlabRequest()));
        }

        [Test]
        public async Task DeleteTaxSlab_TaxSlabNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((TaxSlab?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _taxSlabService.DeleteTaxSlabAsync(1));
        }
    }
}
