using AutoMapper;
using Ergon.DTOs.Department;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class DepartmentServiceTests
    {
        private Mock<IRepository<int, Department>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private DepartmentService _departmentService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Department>>();
            _mockMapper = new Mock<IMapper>();
            _departmentService = new DepartmentService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetDepartmentById_DepartmentExists_ReturnsDepartmentResponse()
        {
            var department = new Department { DepartmentId = 1, DepartmentName = "Engineering" };
            var departmentResponse = new DepartmentResponse { DepartmentId = 1, DepartmentName = "Engineering" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentResponse>(department)).Returns(departmentResponse);

            var result = await _departmentService.GetDepartmentByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DepartmentName, Is.EqualTo("Engineering"));
        }

        [Test]
        public async Task GetDepartmentById_DepartmentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Department?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _departmentService.GetDepartmentByIdAsync(1));
        }

        [Test]
        public async Task CreateDepartment_ValidRequest_ReturnsDepartmentResponse()
        {
            var request = new CreateDepartmentRequest { DepartmentName = "Engineering" };
            var department = new Department { DepartmentId = 1, DepartmentName = "Engineering" };
            var departmentResponse = new DepartmentResponse { DepartmentId = 1, DepartmentName = "Engineering" };
            _mockMapper.Setup(m => m.Map<Department>(request)).Returns(department);
            _mockRepo.Setup(r => r.Create(department)).ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentResponse>(department)).Returns(departmentResponse);

            var result = await _departmentService.CreateDepartmentAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DepartmentName, Is.EqualTo("Engineering"));
        }

        [Test]
        public async Task UpdateDepartment_DepartmentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Department?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _departmentService.UpdateDepartmentAsync(1, new UpdateDepartmentRequest()));
        }

        [Test]
        public async Task DeleteDepartment_DepartmentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Department?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _departmentService.DeleteDepartmentAsync(1));
        }
    }
}
