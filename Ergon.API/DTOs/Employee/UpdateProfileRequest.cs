using System.ComponentModel.DataAnnotations;
namespace Ergon.DTOs.Employee
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string PersonalEmail { get; set; } = string.Empty;
        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? AddressLine2 { get; set; }
        public int? CityId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CountryId { get; set; }
    }
}
