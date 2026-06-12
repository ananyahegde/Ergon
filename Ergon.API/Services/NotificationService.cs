using Ergon.Contexts;
using Ergon.DTOs.Notification;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ErgonContext _context;
        private readonly IMapper _mapper;

        public NotificationService(ErgonContext context, IMapper mapper)
        {
            _context = context;
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
                CreatedAt = DateTime.Now
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationResponse>> GetNotificationsAsync(Guid employeeId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return _mapper.Map<List<NotificationResponse>>(notifications);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found.");
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid employeeId)
        {
            await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) throw new NotFoundException("Notification not found.");
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
