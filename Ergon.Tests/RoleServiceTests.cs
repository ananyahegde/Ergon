using AutoMapper;
using Ergon.DTOs.Role;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class RoleServiceTests
    {
        private Mock<IRepository<int, Role>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private RoleService _roleService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Role>>();
            _mockMapper = new Mock<IMapper>();
            _roleService = new RoleService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetRoleById_RoleExists_ReturnsRoleResponse()
        {
            var role = new Role { RoleId = 1, RoleName = "HR Admin" };
            var roleResponse = new RoleResponse { RoleId = 1, RoleName = "HR Admin" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(role);
            _mockMapper.Setup(m => m.Map<RoleResponse>(role)).Returns(roleResponse);

            var result = await _roleService.GetRoleByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleName, Is.EqualTo("HR Admin"));
        }

        [Test]
        public async Task GetRoleById_RoleNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Role?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _roleService.GetRoleByIdAsync(1));
        }

        [Test]
        public async Task CreateRole_ValidRequest_ReturnsRoleResponse()
        {
            var request = new CreateRoleRequest { RoleName = "HR Admin" };
            var role = new Role { RoleId = 1, RoleName = "HR Admin" };
            var roleResponse = new RoleResponse { RoleId = 1, RoleName = "HR Admin" };
            _mockMapper.Setup(m => m.Map<Role>(request)).Returns(role);
            _mockRepo.Setup(r => r.Create(role)).ReturnsAsync(role);
            _mockMapper.Setup(m => m.Map<RoleResponse>(role)).Returns(roleResponse);

            var result = await _roleService.CreateRoleAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleName, Is.EqualTo("HR Admin"));
        }

        [Test]
        public async Task UpdateRole_RoleNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Role?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _roleService.UpdateRoleAsync(1, new UpdateRoleRequest()));
        }

        [Test]
        public async Task DeleteRole_RoleNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Role?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _roleService.DeleteRoleAsync(1));
        }
    }
}
