using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.City
{
    public class UpdateCityRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CityName { get; set; } = string.Empty;
    }
}
