using AutoMapper;
using Ergon.DTOs.Notification;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class NotificationServiceTests
    {
        private Mock<IRepository<Guid, Notification>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private NotificationService _notificationService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Guid, Notification>>();
            _mockMapper = new Mock<IMapper>();
            _notificationService = new NotificationService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task CreateNotification_ValidRequest_SavesNotification()
        {
            var employeeId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Create(It.IsAny<Notification>())).ReturnsAsync(new Notification());

            await _notificationService.CreateNotificationAsync(employeeId, "Test", "Test message");

            _mockRepo.Verify(r => r.Create(It.IsAny<Notification>()), Times.Once);
        }

        [Test]
        public async Task GetNotifications_ReturnsEmployeeNotifications()
        {
            var employeeId = Guid.NewGuid();
            var notifications = new List<Notification>
            {
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "A", Message = "B", CreatedAt = DateTime.Now },
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), Title = "C", Message = "D", CreatedAt = DateTime.Now }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(notifications);
            _mockMapper.Setup(m => m.Map<List<NotificationResponse>>(It.IsAny<List<Notification>>()))
                .Returns(new List<NotificationResponse> { new() });

            var result = await _notificationService.GetNotificationsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task MarkAsRead_NotificationExists_SetsIsReadTrue()
        {
            var notificationId = Guid.NewGuid();
            var notification = new Notification { NotificationId = notificationId, EmployeeId = Guid.NewGuid(), Title = "A", Message = "B", IsRead = false, CreatedAt = DateTime.Now };
            _mockRepo.Setup(r => r.Get(notificationId)).ReturnsAsync(notification);

            await _notificationService.MarkAsReadAsync(notificationId);

            Assert.That(notification.IsRead, Is.True);
        }

        [Test]
        public async Task MarkAsRead_NotificationNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Notification?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _notificationService.MarkAsReadAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task MarkAllAsRead_SetsAllUnreadToRead()
        {
            var employeeId = Guid.NewGuid();
            var notifications = new List<Notification>
            {
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "A", Message = "B", IsRead = false, CreatedAt = DateTime.Now },
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "C", Message = "D", IsRead = false, CreatedAt = DateTime.Now }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(notifications);

            await _notificationService.MarkAllAsReadAsync(employeeId);

            Assert.That(notifications.All(n => n.IsRead), Is.True);
        }

        [Test]
        public async Task DeleteNotification_NotificationExists_DeletesIt()
        {
            var notificationId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(notificationId))
                .ReturnsAsync(new Notification { NotificationId = notificationId, EmployeeId = Guid.NewGuid(), Title = "A", Message = "B", CreatedAt = DateTime.Now });

            await _notificationService.DeleteNotificationAsync(notificationId);

            _mockRepo.Verify(r => r.Delete(notificationId), Times.Once);
        }

        [Test]
        public async Task DeleteNotification_NotificationNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Notification?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _notificationService.DeleteNotificationAsync(Guid.NewGuid()));
        }
    }
}
