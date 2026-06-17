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
        private Mock<IMapper> _mockMapper = null!;
        private ReviewCycleService _reviewCycleService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Guid, ReviewCycle>>();
            _mockMapper = new Mock<IMapper>();
            _reviewCycleService = new ReviewCycleService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAllReviewCycles_ReturnsPagedResult()
        {
            var cycles = new List<ReviewCycle>
            {
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active },
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle2", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(cycles);
            _mockMapper.Setup(m => m.Map<List<ReviewCycleResponse>>(It.IsAny<List<ReviewCycle>>()))
                .Returns(new List<ReviewCycleResponse> { new(), new() });

            var result = await _reviewCycleService.GetAllReviewCyclesAsync(new GetAllReviewCyclesRequest { PageNumber = 1, PageSize = 10 });

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllReviewCycles_StatusFilter_ReturnsMatchingCycles()
        {
            var cycles = new List<ReviewCycle>
            {
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active },
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle2", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(cycles);
            _mockMapper.Setup(m => m.Map<List<ReviewCycleResponse>>(It.IsAny<List<ReviewCycle>>()))
                .Returns(new List<ReviewCycleResponse> { new() });

            var result = await _reviewCycleService.GetAllReviewCyclesAsync(new GetAllReviewCyclesRequest { PageNumber = 1, PageSize = 10, Status = "Active" });

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetReviewCycleById_Exists_ReturnsResponse()
        {
            var reviewCycleId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(reviewCycleId))
                .ReturnsAsync(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active });
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            var result = await _reviewCycleService.GetReviewCycleByIdAsync(reviewCycleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReviewCycleId, Is.EqualTo(reviewCycleId));
        }

        [Test]
        public async Task GetReviewCycleById_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.GetReviewCycleByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreateReviewCycle_ValidRequest_ReturnsResponse()
        {
            var request = new CreateReviewCycleRequest
            {
                ReviewName = "Cycle1",
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
            };
            var reviewCycle = new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle1", StartDate = request.StartDate, EndDate = request.EndDate };
            _mockMapper.Setup(m => m.Map<ReviewCycle>(request)).Returns(reviewCycle);
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycle.ReviewCycleId });

            var result = await _reviewCycleService.CreateReviewCycleAsync(request);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateReviewCycle_EndDateBeforeStartDate_ThrowsBadRequestException()
        {
            var request = new CreateReviewCycleRequest
            {
                ReviewName = "Cycle1",
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.CreateReviewCycleAsync(request));
        }

        [Test]
        public async Task UpdateReviewCycle_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.UpdateReviewCycleAsync(Guid.NewGuid(), new UpdateReviewCycleRequest()));
        }

        [Test]
        public async Task UpdateReviewCycle_ClosedCycle_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(reviewCycleId))
                .ReturnsAsync(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.UpdateReviewCycleAsync(reviewCycleId, new UpdateReviewCycleRequest()));
        }

        [Test]
        public async Task UpdateReviewCycle_ActiveCycle_UpdatesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            var reviewCycle = new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _mockRepo.Setup(r => r.Get(reviewCycleId)).ReturnsAsync(reviewCycle);
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            var result = await _reviewCycleService.UpdateReviewCycleAsync(reviewCycleId, new UpdateReviewCycleRequest { ReviewName = "Updated Cycle" });

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task DeleteReviewCycle_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.DeleteReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteReviewCycle_ActiveCycle_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(reviewCycleId))
                .ReturnsAsync(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active });

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.DeleteReviewCycleAsync(reviewCycleId));
        }

        [Test]
        public async Task DeleteReviewCycle_ClosedCycle_DeletesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(reviewCycleId))
                .ReturnsAsync(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            await _reviewCycleService.DeleteReviewCycleAsync(reviewCycleId);

            _mockRepo.Verify(r => r.Delete(reviewCycleId), Times.Once);
        }

        [Test]
        public async Task CloseReviewCycle_NotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.CloseReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CloseReviewCycle_AlreadyClosed_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(reviewCycleId))
                .ReturnsAsync(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.CloseReviewCycleAsync(reviewCycleId));
        }

        [Test]
        public async Task CloseReviewCycle_Active_ClosesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            var reviewCycle = new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _mockRepo.Setup(r => r.Get(reviewCycleId)).ReturnsAsync(reviewCycle);
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId, ReviewCycleStatus = ReviewCycleStatusEnum.Closed });

            var result = await _reviewCycleService.CloseReviewCycleAsync(reviewCycleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(reviewCycle.ReviewCycleStatus, Is.EqualTo(ReviewCycleStatusEnum.Closed));
        }
    }
}
