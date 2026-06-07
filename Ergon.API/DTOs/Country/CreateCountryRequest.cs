using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Country
{
    public class CreateCountryRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]

        public string CountryName { get; set; } = string.Empty;
    }
}
