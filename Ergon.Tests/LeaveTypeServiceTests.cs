using AutoMapper;
using Ergon.DTOs.LeaveType;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class LeaveTypeServiceTests
    {
        private Mock<IRepository<int, LeaveType>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private LeaveTypeService _leaveTypeService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, LeaveType>>();
            _mockMapper = new Mock<IMapper>();
            _leaveTypeService = new LeaveTypeService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetLeaveTypeById_LeaveTypeExists_ReturnsLeaveTypeResponse()
        {
            var leaveType = new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" };
            var leaveTypeResponse = new LeaveTypeResponse { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(leaveType);
            _mockMapper.Setup(m => m.Map<LeaveTypeResponse>(leaveType)).Returns(leaveTypeResponse);

            var result = await _leaveTypeService.GetLeaveTypeByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LeaveTypeName, Is.EqualTo("Casual Leave"));
        }

        [Test]
        public async Task GetLeaveTypeById_LeaveTypeNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveType?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveTypeService.GetLeaveTypeByIdAsync(1));
        }

        [Test]
        public async Task CreateLeaveType_ValidRequest_ReturnsLeaveTypeResponse()
        {
            var request = new CreateLeaveTypeRequest { LeaveTypeName = "Casual Leave" };
            var leaveType = new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" };
            var leaveTypeResponse = new LeaveTypeResponse { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" };
            _mockMapper.Setup(m => m.Map<LeaveType>(request)).Returns(leaveType);
            _mockRepo.Setup(r => r.Create(leaveType)).ReturnsAsync(leaveType);
            _mockMapper.Setup(m => m.Map<LeaveTypeResponse>(leaveType)).Returns(leaveTypeResponse);

            var result = await _leaveTypeService.CreateLeaveTypeAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LeaveTypeName, Is.EqualTo("Casual Leave"));
        }

        [Test]
        public async Task UpdateLeaveType_LeaveTypeNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveType?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveTypeService.UpdateLeaveTypeAsync(1, new UpdateLeaveTypeRequest()));
        }

        [Test]
        public async Task DeleteLeaveType_LeaveTypeNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((LeaveType?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _leaveTypeService.DeleteLeaveTypeAsync(1));
        }
    }
}
