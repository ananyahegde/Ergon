using AutoMapper;
using Ergon.DTOs.ReviewCycle;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class ReviewCycleServiceTests
    {
        private Mock<IRepository<Guid, ReviewCycle>> _mockRepo = null!;
        private Mock<IReviewCycleRepository> _mockReviewCycleRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private ReviewCycleService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Guid, ReviewCycle>>();
            _mockReviewCycleRepo = new Mock<IReviewCycleRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ReviewCycleService(_mockRepo.Object, _mockReviewCycleRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task CreateReviewCycle_ValidRequest_ReturnsResponse()
        {
            var request = new CreateReviewCycleRequest
            {
                ReviewName = "Q1 Review",
                StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30))
            };
            _mockMapper.Setup(m => m.Map<ReviewCycle>(request)).Returns(new ReviewCycle());
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>())).Returns(new ReviewCycleResponse());

            var result = await _service.CreateReviewCycleAsync(request);

            Assert.That(result, Is.Not.Null);
            _mockRepo.Verify(r => r.Create(It.IsAny<ReviewCycle>()), Times.Once);
        }

        [Test]
        public async Task CreateReviewCycle_EndDateBeforeStartDate_ThrowsBadRequest()
        {
            var request = new CreateReviewCycleRequest
            {
                ReviewName = "Q1 Review",
                StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewCycleAsync(request));
        }

        [Test]
        public async Task CreateReviewCycle_CycleShorterThan14Days_ThrowsBadRequest()
        {
            var request = new CreateReviewCycleRequest
            {
                ReviewName = "Q1 Review",
                StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
            };

            Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewCycleAsync(request));
        }

        [Test]
        public async Task StartReviewCycle_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.StartReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task StartReviewCycle_NotDraft_ThrowsBadRequest()
        {
            var reviewCycle = new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(reviewCycle);

            Assert.ThrowsAsync<BadRequestException>(() => _service.StartReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task StartReviewCycle_ValidDraft_CreatesDetailsAndActivates()
        {
            var reviewCycleId = Guid.NewGuid();
            var reviewCycle = new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewCycleStatus = ReviewCycleStatusEnum.Draft };
            var activeEmployees = new List<Employee>
            {
                new Employee { EmployeeId = Guid.NewGuid() },
                new Employee { EmployeeId = Guid.NewGuid() }
            };

            _mockRepo.Setup(r => r.Get(reviewCycleId)).ReturnsAsync(reviewCycle);
            _mockReviewCycleRepo.Setup(r => r.GetActiveEmployeesAsync()).ReturnsAsync(activeEmployees);
            _mockReviewCycleRepo.Setup(r => r.GetExistingDetailEmployeeIdsAsync(reviewCycleId)).ReturnsAsync(new List<Guid>());
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(reviewCycle)).Returns(new ReviewCycleResponse());

            var result = await _service.StartReviewCycleAsync(reviewCycleId);

            _mockReviewCycleRepo.Verify(r => r.AddReviewCycleDetailsRangeAsync(It.Is<List<ReviewCycleDetails>>(d => d.Count == 2)), Times.Once);
            _mockReviewCycleRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.That(reviewCycle.ReviewCycleStatus, Is.EqualTo(ReviewCycleStatusEnum.Active));
        }

        [Test]
        public async Task CloseReviewCycle_AlreadyClosed_ThrowsBadRequest()
        {
            var reviewCycle = new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Closed };
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(reviewCycle);

            Assert.ThrowsAsync<BadRequestException>(() => _service.CloseReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CloseReviewCycle_Draft_ThrowsBadRequest()
        {
            var reviewCycle = new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Draft };
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(reviewCycle);

            Assert.ThrowsAsync<BadRequestException>(() => _service.CloseReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteReviewCycle_Active_ThrowsBadRequest()
        {
            var reviewCycle = new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(reviewCycle);

            Assert.ThrowsAsync<BadRequestException>(() => _service.DeleteReviewCycleAsync(Guid.NewGuid()));
        }
    }
}
