using Asp.Versioning;
using Ergon.DTOs.Country;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/countries")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            return Ok(country);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpPost]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryRequest request)
        {
            var createdCountry = await _countryService.CreateCountryAsync(request);
            return Created("", createdCountry);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryRequest request)
        {
            var updatedCountry = await _countryService.UpdateCountryAsync(id, request);
            return Ok(updatedCountry);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HRAdminOnly")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var deletedCountry = await _countryService.DeleteCountryAsync(id);
            return Ok(deletedCountry);
        }
    }
}
