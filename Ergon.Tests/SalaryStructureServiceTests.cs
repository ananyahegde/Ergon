using AutoMapper;
using Ergon.DTOs.SalaryStructure;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class SalaryStructureServiceTests
    {
        private Mock<IRepository<int, SalaryStructure>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private SalaryStructureService _salaryStructureService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, SalaryStructure>>();
            _mockMapper = new Mock<IMapper>();
            _salaryStructureService = new SalaryStructureService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetSalaryStructureById_SalaryStructureExists_ReturnsSalaryStructureResponse()
        {
            var salaryStructure = new SalaryStructure { SalaryStructureId = 1, SalaryStructureName = "Structure A" };
            var salaryStructureResponse = new SalaryStructureResponse { SalaryStructureId = 1, SalaryStructureName = "Structure A" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(salaryStructure);
            _mockMapper.Setup(m => m.Map<SalaryStructureResponse>(salaryStructure)).Returns(salaryStructureResponse);

            var result = await _salaryStructureService.GetSalaryStructureByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SalaryStructureName, Is.EqualTo("Structure A"));
        }

        [Test]
        public async Task GetSalaryStructureById_SalaryStructureNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryStructure?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryStructureService.GetSalaryStructureByIdAsync(1));
        }

        [Test]
        public async Task CreateSalaryStructure_ValidRequest_ReturnsSalaryStructureResponse()
        {
            var request = new CreateSalaryStructureRequest { SalaryStructureName = "Structure A" };
            var salaryStructure = new SalaryStructure { SalaryStructureId = 1, SalaryStructureName = "Structure A" };
            var salaryStructureResponse = new SalaryStructureResponse { SalaryStructureId = 1, SalaryStructureName = "Structure A" };
            _mockMapper.Setup(m => m.Map<SalaryStructure>(request)).Returns(salaryStructure);
            _mockRepo.Setup(r => r.Create(salaryStructure)).ReturnsAsync(salaryStructure);
            _mockMapper.Setup(m => m.Map<SalaryStructureResponse>(salaryStructure)).Returns(salaryStructureResponse);

            var result = await _salaryStructureService.CreateSalaryStructureAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SalaryStructureName, Is.EqualTo("Structure A"));
        }

        [Test]
        public async Task UpdateSalaryStructure_SalaryStructureNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryStructure?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryStructureService.UpdateSalaryStructureAsync(1, new UpdateSalaryStructureRequest()));
        }

        [Test]
        public async Task DeleteSalaryStructure_SalaryStructureNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryStructure?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryStructureService.DeleteSalaryStructureAsync(1));
        }
    }
}
