namespace Ergon.DTOs.Auth
{
    public class CreateLoginRequest
    {
        public string WorkEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
