using AutoMapper;
using Ergon.DTOs.LeaveEntitlementComponent;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class LeaveEntitlementComponentServiceTests
    {
        private Mock<IRepository<int, LeaveEntitlementComponent>> _mockRepo = null!;
        private Mock<IRepository<int, LeaveEntitlement>> _mockLeaveEntitlementRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private LeaveEntitlementComponentService _leaveEntitlementComponentService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, LeaveEntitlementComponent>>();
            _mockLeaveEntitlementRepo = new Mock<IRepository<int, LeaveEntitlement>>();
            _mockMapper = new Mock<IMapper>();
            _leaveEntitlementComponentService = new LeaveEntitlementComponentService(_mockRepo.Object, _mockLeaveEntitlementRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetLeaveEntitlementComponentById_Exists_ReturnsResponse()
        {
            var lec = new LeaveEntitlementComponent { LeaveEntitlementComponentId = 1, TotalDays = 12 };
            var lecResponse = new LeaveEntitlementComponentResponse { LeaveEntitlementComponentId = 1, TotalDays = 12 };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(lec);
            _mockMapper.Setup(m => m.Map<LeaveEntitlementComponentResponse>(lec)).Returns(lecResponse);

            var result = await _leaveEntitlementComponentService.GetLeaveEntitlementComponentByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalDays, Is.EqualTo(12));
        }

        [Test]
        public async Task GetLeaveEntitlementComponentById_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlementComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementComponentService.GetLeaveEntitlementComponentByIdAsync(1));
        }

        [Test]
        public async Task CreateLeaveEntitlementComponent_LeaveEntitlementNotFound_ThrowsNotFoundException()
        {
            _mockLeaveEntitlementRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlement?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementComponentService.CreateLeaveEntitlementComponentAsync(1, new CreateLeaveEntitlementComponentRequest()));
        }

        [Test]
        public async Task UpdateLeaveEntitlementComponent_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlementComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementComponentService.UpdateLeaveEntitlementComponentAsync(1, new UpdateLeaveEntitlementComponentRequest()));
        }

        [Test]
        public async Task DeleteLeaveEntitlementComponent_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveEntitlementComponent?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveEntitlementComponentService.DeleteLeaveEntitlementComponentAsync(1));
        }
    }
}
