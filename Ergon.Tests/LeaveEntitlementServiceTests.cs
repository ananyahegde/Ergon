using AutoMapper;
using Ergon.DTOs.LeaveEntitlement;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class LeaveEntitlementServiceTests
    {
        private Mock<IRepository<int, LeaveEntitlement>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private LeaveEntitlementService _leaveEntitlementService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, LeaveEntitlement>>();
            _mockMapper = new Mock<IMapper>();
            _leaveEntitlementService = new LeaveEntitlementService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetLeaveEntitlementById_LeaveEntitlementExists_ReturnsLeaveEntitlementResponse()
        {
            var leaveEntitlement = new LeaveEntitlement { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard Entitlement" };
            var leaveEntitlementResponse = new LeaveEntitlementResponse { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard Entitlement" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(leaveEntitlement);
            _mockMapper.Setup(m => m.Map<LeaveEntitlementResponse>(leaveEntitlement)).Returns(leaveEntitlementResponse);

            var result = await _leaveEntitlementService.GetLeaveEntitlementByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LeaveEntitlementName, Is.EqualTo("Standard Entitlement"));
        }

        [Test]
        public async Task GetLeaveEntitlementById_LeaveEntitlementNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlement?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementService.GetLeaveEntitlementByIdAsync(1));
        }

        [Test]
        public async Task CreateLeaveEntitlement_ValidRequest_ReturnsLeaveEntitlementResponse()
        {
            var request = new CreateLeaveEntitlementRequest { LeaveEntitlementName = "Standard Entitlement" };
            var leaveEntitlement = new LeaveEntitlement { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard Entitlement" };
            var leaveEntitlementResponse = new LeaveEntitlementResponse { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard Entitlement" };
            _mockMapper.Setup(m => m.Map<LeaveEntitlement>(request)).Returns(leaveEntitlement);
            _mockRepo.Setup(r => r.Create(leaveEntitlement)).ReturnsAsync(leaveEntitlement);
            _mockMapper.Setup(m => m.Map<LeaveEntitlementResponse>(leaveEntitlement)).Returns(leaveEntitlementResponse);

            var result = await _leaveEntitlementService.CreateLeaveEntitlementAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LeaveEntitlementName, Is.EqualTo("Standard Entitlement"));
        }

        [Test]
        public async Task UpdateLeaveEntitlement_LeaveEntitlementNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlement?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementService.UpdateLeaveEntitlementAsync(1, new UpdateLeaveEntitlementRequest()));
        }

        [Test]
        public async Task DeleteLeaveEntitlement_LeaveEntitlementNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlement?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementService.DeleteLeaveEntitlementAsync(1));
        }
    }
}
