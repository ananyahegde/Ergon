using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Notification;
using Ergon.Exceptions;
using Ergon.Models;
using Ergon.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class NotificationServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IMapper> _mockMapper = null!;
        private NotificationService _notificationService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockMapper = new Mock<IMapper>();
            _notificationService = new NotificationService(_context, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateNotification_ValidRequest_SavesNotification()
        {
            var employeeId = Guid.NewGuid();
            await _notificationService.CreateNotificationAsync(employeeId, "Test", "Test message");
            var count = _context.Notifications.Count();
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetNotifications_ReturnsEmployeeNotifications()
        {
            var employeeId = Guid.NewGuid();
            _context.Notifications.AddRange(
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "A", Message = "B", CreatedAt = DateTime.Now },
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), Title = "C", Message = "D", CreatedAt = DateTime.Now }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<NotificationResponse>>(It.IsAny<List<Notification>>()))
                .Returns(new List<NotificationResponse> { new() });

            var result = await _notificationService.GetNotificationsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task MarkAsRead_NotificationExists_SetsIsReadTrue()
        {
            var notificationId = Guid.NewGuid();
            _context.Notifications.Add(new Notification { NotificationId = notificationId, EmployeeId = Guid.NewGuid(), Title = "A", Message = "B", IsRead = false, CreatedAt = DateTime.Now });
            await _context.SaveChangesAsync();

            await _notificationService.MarkAsReadAsync(notificationId);

            var notification = await _context.Notifications.FindAsync(notificationId);
            Assert.That(notification!.IsRead, Is.True);
        }

        [Test]
        public async Task MarkAsRead_NotificationNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _notificationService.MarkAsReadAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task MarkAllAsRead_SetsAllUnreadToRead()
        {
            var employeeId = Guid.NewGuid();
            _context.Notifications.AddRange(
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "A", Message = "B", IsRead = false, CreatedAt = DateTime.Now },
                new Notification { NotificationId = Guid.NewGuid(), EmployeeId = employeeId, Title = "C", Message = "D", IsRead = false, CreatedAt = DateTime.Now }
            );
            await _context.SaveChangesAsync();

            await _notificationService.MarkAllAsReadAsync(employeeId);

            var unread = _context.Notifications.Count(n => !n.IsRead);
            Assert.That(unread, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteNotification_NotificationExists_DeletesIt()
        {
            var notificationId = Guid.NewGuid();
            _context.Notifications.Add(new Notification { NotificationId = notificationId, EmployeeId = Guid.NewGuid(), Title = "A", Message = "B", CreatedAt = DateTime.Now });
            await _context.SaveChangesAsync();

            await _notificationService.DeleteNotificationAsync(notificationId);

            var count = _context.Notifications.Count();
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteNotification_NotificationNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _notificationService.DeleteNotificationAsync(Guid.NewGuid()));
        }
    }
}
