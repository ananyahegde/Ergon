using AutoMapper;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class ReviewCycleDetailsServiceTests
    {
        private Mock<IReviewCycleDetailsRepository> _mockDetailsRepo = null!;
        private Mock<IRepository<Guid, ReviewCycleDetails>> _mockGenericRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private ReviewCycleDetailsService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockDetailsRepo = new Mock<IReviewCycleDetailsRepository>();
            _mockGenericRepo = new Mock<IRepository<Guid, ReviewCycleDetails>>();
            _mockMapper = new Mock<IMapper>();
            _service = new ReviewCycleDetailsService(_mockDetailsRepo.Object, _mockGenericRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task CreateReviewCycleDetails_ReviewCycleNotFound_ThrowsNotFoundException()
        {
            _mockDetailsRepo.Setup(r => r.GetReviewCycleAsync(It.IsAny<Guid>())).ReturnsAsync((ReviewCycle?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReviewCycleDetailsAsync(Guid.NewGuid(), new CreateReviewCycleDetailsRequest()));
        }

        [Test]
        public async Task CreateReviewCycleDetails_ClosedCycle_ThrowsBadRequest()
        {
            _mockDetailsRepo.Setup(r => r.GetReviewCycleAsync(It.IsAny<Guid>())).ReturnsAsync(new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Closed });

            Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewCycleDetailsAsync(Guid.NewGuid(), new CreateReviewCycleDetailsRequest()));
        }

        [Test]
        public async Task CreateReviewCycleDetails_DraftCycle_ThrowsBadRequest()
        {
            _mockDetailsRepo.Setup(r => r.GetReviewCycleAsync(It.IsAny<Guid>())).ReturnsAsync(new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Draft });

            Assert.ThrowsAsync<BadRequestException>(() => _service.CreateReviewCycleDetailsAsync(Guid.NewGuid(), new CreateReviewCycleDetailsRequest()));
        }

        [Test]
        public async Task CreateReviewCycleDetails_EmployeeNotFound_ThrowsNotFoundException()
        {
            _mockDetailsRepo.Setup(r => r.GetReviewCycleAsync(It.IsAny<Guid>())).ReturnsAsync(new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Active });
            _mockDetailsRepo.Setup(r => r.GetEmployeeAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReviewCycleDetailsAsync(Guid.NewGuid(), new CreateReviewCycleDetailsRequest { EmployeeId = Guid.NewGuid() }));
        }

        [Test]
        public async Task CreateReviewCycleDetails_DuplicateEntry_ThrowsConflict()
        {
            _mockDetailsRepo.Setup(r => r.GetReviewCycleAsync(It.IsAny<Guid>())).ReturnsAsync(new ReviewCycle { ReviewCycleStatus = ReviewCycleStatusEnum.Active });
            _mockDetailsRepo.Setup(r => r.GetEmployeeAsync(It.IsAny<Guid>())).ReturnsAsync(new Employee { EmploymentStatus = EmploymentStatusEnum.Active });
            _mockDetailsRepo.Setup(r => r.DetailsExistAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            Assert.ThrowsAsync<ConflictException>(() => _service.CreateReviewCycleDetailsAsync(Guid.NewGuid(), new CreateReviewCycleDetailsRequest { EmployeeId = Guid.NewGuid() }));
        }

        [Test]
        public async Task UpdateSelfScore_ScoreOutOfRange_ThrowsBadRequest()
        {
            Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateSelfScoreAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateSelfScoreRequest { SelfScore = 11 }));
        }

        [Test]
        public async Task UpdateSelfScore_WrongEmployee_ThrowsForbidden()
        {
            var detailsId = Guid.NewGuid();
            var details = new ReviewCycleDetails
            {
                EmployeeId = Guid.NewGuid(),
                ReviewCycle = new ReviewCycle
                {
                    ReviewCycleStatus = ReviewCycleStatusEnum.Active,
                    EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
                }
            };
            _mockDetailsRepo.Setup(r => r.GetDetailsWithCycleAsync(detailsId)).ReturnsAsync(details);

            Assert.ThrowsAsync<ForbiddenException>(() => _service.UpdateSelfScoreAsync(detailsId, Guid.NewGuid(), new UpdateSelfScoreRequest { SelfScore = 8 }));
        }

        [Test]
        public async Task UpdateFeedback_ScoreOutOfRange_ThrowsBadRequest()
        {
            Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateFeedbackAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateFeedbackRequest { FeedbackScore = 0 }));
        }

        [Test]
        public async Task UpdateFeedback_NotSubordinate_ThrowsForbidden()
        {
            var detailsId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var details = new ReviewCycleDetails
            {
                Employee = new Employee { ReportsTo = Guid.NewGuid() },
                ReviewCycle = new ReviewCycle
                {
                    ReviewCycleStatus = ReviewCycleStatusEnum.Active,
                    EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
                }
            };
            _mockDetailsRepo.Setup(r => r.GetDetailsWithCycleAndEmployeeAsync(detailsId)).ReturnsAsync(details);

            Assert.ThrowsAsync<ForbiddenException>(() => _service.UpdateFeedbackAsync(detailsId, managerId, new UpdateFeedbackRequest { FeedbackScore = 8 }));
        }

        [Test]
        public async Task DeleteReviewCycleDetails_NotFound_ThrowsNotFoundException()
        {
            _mockDetailsRepo.Setup(r => r.GetDetailsByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ReviewCycleDetails?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteReviewCycleDetailsAsync(Guid.NewGuid()));
        }
    }
}
