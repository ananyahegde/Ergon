using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
