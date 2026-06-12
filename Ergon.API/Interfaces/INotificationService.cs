using Ergon.DTOs.Notification;

namespace Ergon.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(Guid employeeId, string title, string message);
        Task<IEnumerable<NotificationResponse>> GetNotificationsAsync(Guid employeeId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid employeeId);
        Task DeleteNotificationAsync(Guid notificationId);
    }
}
