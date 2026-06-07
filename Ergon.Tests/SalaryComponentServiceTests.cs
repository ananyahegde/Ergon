using AutoMapper;
using Ergon.DTOs.SalaryComponent;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class SalaryComponentServiceTests
    {
        private Mock<IRepository<int, SalaryComponent>> _mockRepo = null!;
        private Mock<IRepository<int, SalaryStructure>> _mockSalaryStructureRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private SalaryComponentService _salaryComponentService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, SalaryComponent>>();
            _mockSalaryStructureRepo = new Mock<IRepository<int, SalaryStructure>>();
            _mockMapper = new Mock<IMapper>();
            _salaryComponentService = new SalaryComponentService(_mockRepo.Object, _mockSalaryStructureRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetSalaryComponentById_SalaryComponentExists_ReturnsSalaryComponentResponse()
        {
            var salaryComponent = new SalaryComponent { SalaryComponentId = 1, ComponentName = "Basic" };
            var salaryComponentResponse = new SalaryComponentResponse { SalaryComponentId = 1, ComponentName = "Basic" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(salaryComponent);
            _mockMapper.Setup(m => m.Map<SalaryComponentResponse>(salaryComponent)).Returns(salaryComponentResponse);

            var result = await _salaryComponentService.GetSalaryComponentByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ComponentName, Is.EqualTo("Basic"));
        }

        [Test]
        public async Task GetSalaryComponentById_SalaryComponentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryComponentService.GetSalaryComponentByIdAsync(1));
        }

        [Test]
        public async Task CreateSalaryComponent_SalaryStructureNotFound_ThrowsNotFoundException()
        {
            _mockSalaryStructureRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryStructure?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryComponentService.CreateSalaryComponentAsync(1, new CreateSalaryComponentRequest()));
        }

        [Test]
        public async Task UpdateSalaryComponent_SalaryComponentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryComponentService.UpdateSalaryComponentAsync(1, new UpdateSalaryComponentRequest()));
        }

        [Test]
        public async Task DeleteSalaryComponent_SalaryComponentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((SalaryComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _salaryComponentService.DeleteSalaryComponentAsync(1));
        }
    }
}
