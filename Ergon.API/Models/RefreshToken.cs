namespace Ergon.Models
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // foreign key
        public Guid EmployeeId { get; set; }

        // navigation
        public Employee Employee { get; set; } = null!;
    }
}
