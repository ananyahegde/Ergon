using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Auth
{
    public class CreateLoginRequest
    {
        [Required(ErrorMessage = "Work email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string WorkEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
