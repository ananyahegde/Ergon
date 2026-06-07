using AutoMapper;
using Ergon.DTOs.Designation;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class DesignationServiceTests
    {
        private Mock<IRepository<int, Designation>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private DesignationService _designationService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Designation>>();
            _mockMapper = new Mock<IMapper>();
            _designationService = new DesignationService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetDesignationById_DesignationExists_ReturnsDesignationResponse()
        {
            var designation = new Designation { DesignationId = 1, DesignationName = "Software Engineer" };
            var designationResponse = new DesignationResponse { DesignationId = 1, DesignationName = "Software Engineer" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(designation);
            _mockMapper.Setup(m => m.Map<DesignationResponse>(designation)).Returns(designationResponse);

            var result = await _designationService.GetDesignationByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DesignationName, Is.EqualTo("Software Engineer"));
        }

        [Test]
        public async Task GetDesignationById_DesignationNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Designation?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _designationService.GetDesignationByIdAsync(1));
        }

        [Test]
        public async Task CreateDesignation_ValidRequest_ReturnsDesignationResponse()
        {
            var request = new CreateDesignationRequest { DesignationName = "Software Engineer" };
            var designation = new Designation { DesignationId = 1, DesignationName = "Software Engineer" };
            var designationResponse = new DesignationResponse { DesignationId = 1, DesignationName = "Software Engineer" };
            _mockMapper.Setup(m => m.Map<Designation>(request)).Returns(designation);
            _mockRepo.Setup(r => r.Create(designation)).ReturnsAsync(designation);
            _mockMapper.Setup(m => m.Map<DesignationResponse>(designation)).Returns(designationResponse);

            var result = await _designationService.CreateDesignationAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DesignationName, Is.EqualTo("Software Engineer"));
        }

        [Test]
        public async Task UpdateDesignation_DesignationNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Designation?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _designationService.UpdateDesignationAsync(1, new UpdateDesignationRequest()));
        }

        [Test]
        public async Task DeleteDesignation_DesignationNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Designation?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _designationService.DeleteDesignationAsync(1));
        }
    }
}
