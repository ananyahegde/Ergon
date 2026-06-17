using AutoMapper;
using Ergon.DTOs.Notification;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Guid, Notification> _repository;
        private readonly IMapper _mapper;

        public NotificationService(IRepository<Guid, Notification> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task CreateNotificationAsync(Guid employeeId, string title, string message)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.Create(notification);
        }

        public async Task<IEnumerable<NotificationResponse>> GetNotificationsAsync(Guid employeeId)
        {
            var all = await _repository.GetAll();
            var notifications = all
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToList();
            return _mapper.Map<List<NotificationResponse>>(notifications);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _repository.Get(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found.");
            notification.IsRead = true;
            await _repository.Update(notificationId, notification);
        }

        public async Task MarkAllAsReadAsync(Guid employeeId)
        {
            var all = await _repository.GetAll();
            var unread = all.Where(n => n.EmployeeId == employeeId && !n.IsRead).ToList();
            foreach (var notification in unread)
            {
                notification.IsRead = true;
                await _repository.Update(notification.NotificationId, notification);
            }
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _repository.Get(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found.");
            await _repository.Delete(notificationId);
        }
    }
}
