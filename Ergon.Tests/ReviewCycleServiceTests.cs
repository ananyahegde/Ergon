using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.ReviewCycle;
using Ergon.Exceptions;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class ReviewCycleServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IMapper> _mockMapper = null!;
        private ReviewCycleService _reviewCycleService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockMapper = new Mock<IMapper>();
            _reviewCycleService = new ReviewCycleService(_context, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetAllReviewCycles_ReturnsPagedResult()
        {
            _context.ReviewCycles.AddRange(
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active },
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle2", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<ReviewCycleResponse>>(It.IsAny<List<ReviewCycle>>()))
                .Returns(new List<ReviewCycleResponse> { new(), new() });

            var request = new GetAllReviewCyclesRequest { PageNumber = 1, PageSize = 10 };
            var result = await _reviewCycleService.GetAllReviewCyclesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllReviewCycles_StatusFilter_ReturnsMatchingCycles()
        {
            _context.ReviewCycles.AddRange(
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active },
                new ReviewCycle { ReviewCycleId = Guid.NewGuid(), ReviewName = "Cycle2", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<ReviewCycleResponse>>(It.IsAny<List<ReviewCycle>>()))
                .Returns(new List<ReviewCycleResponse> { new() });

            var request = new GetAllReviewCyclesRequest { PageNumber = 1, PageSize = 10, Status = "Active" };
            var result = await _reviewCycleService.GetAllReviewCyclesAsync(request);

            Assert.That(result.TotalCount, Is.EqualTo(1));
        }


        [Test]
        public async Task GetReviewCycleById_Exists_ReturnsResponse()
        {
            var reviewCycleId = Guid.NewGuid();
            _context.ReviewCycles.Add(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            var result = await _reviewCycleService.GetReviewCycleByIdAsync(reviewCycleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReviewCycleId, Is.EqualTo(reviewCycleId));
        }

        [Test]
        public async Task GetReviewCycleById_NotFound_ThrowsNotFoundException()
        {
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
            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.UpdateReviewCycleAsync(Guid.NewGuid(), new UpdateReviewCycleRequest()));
        }

        [Test]
        public async Task UpdateReviewCycle_ClosedCycle_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _context.ReviewCycles.Add(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.UpdateReviewCycleAsync(reviewCycleId, new UpdateReviewCycleRequest()));
        }

        [Test]
        public async Task UpdateReviewCycle_ActiveCycle_UpdatesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            var reviewCycle = new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _context.ReviewCycles.Add(reviewCycle);
            await _context.SaveChangesAsync();

            var request = new UpdateReviewCycleRequest { ReviewName = "Updated Cycle" };
            _mockMapper.Setup(m => m.Map(request, reviewCycle));
            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            var result = await _reviewCycleService.UpdateReviewCycleAsync(reviewCycleId, request);

            Assert.That(result, Is.Not.Null);
        }


        [Test]
        public async Task DeleteReviewCycle_NotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.DeleteReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteReviewCycle_ActiveCycle_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _context.ReviewCycles.Add(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.DeleteReviewCycleAsync(reviewCycleId));
        }

        [Test]
        public async Task DeleteReviewCycle_ClosedCycle_DeletesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            _context.ReviewCycles.Add(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId });

            await _reviewCycleService.DeleteReviewCycleAsync(reviewCycleId);

            var deleted = await _context.ReviewCycles.FindAsync(reviewCycleId);
            Assert.That(deleted, Is.Null);
        }


        [Test]
        public async Task CloseReviewCycle_NotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _reviewCycleService.CloseReviewCycleAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CloseReviewCycle_AlreadyClosed_ThrowsBadRequestException()
        {
            var reviewCycleId = Guid.NewGuid();
            _context.ReviewCycles.Add(new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Closed });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<BadRequestException>(() =>
                _reviewCycleService.CloseReviewCycleAsync(reviewCycleId));
        }

        [Test]
        public async Task CloseReviewCycle_Active_ClosesSuccessfully()
        {
            var reviewCycleId = Guid.NewGuid();
            var reviewCycle = new ReviewCycle { ReviewCycleId = reviewCycleId, ReviewName = "Cycle1", StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)), ReviewCycleStatus = ReviewCycleStatusEnum.Active };
            _context.ReviewCycles.Add(reviewCycle);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<ReviewCycleResponse>(It.IsAny<ReviewCycle>()))
                .Returns(new ReviewCycleResponse { ReviewCycleId = reviewCycleId, ReviewCycleStatus = ReviewCycleStatusEnum.Closed });

            var result = await _reviewCycleService.CloseReviewCycleAsync(reviewCycleId);

            Assert.That(result, Is.Not.Null);
            var updated = await _context.ReviewCycles.FindAsync(reviewCycleId);
            Assert.That(updated!.ReviewCycleStatus, Is.EqualTo(ReviewCycleStatusEnum.Closed));
        }
    }
}
