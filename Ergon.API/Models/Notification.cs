namespace Ergon.Models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // foreign key
        public Guid EmployeeId { get; set; }

        // navigation
        public Employee Employee { get; set; } = null!;
    }
}
