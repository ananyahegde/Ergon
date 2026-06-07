using Asp.Versioning;
using Ergon.DTOs.State;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/states")]
    [EnableRateLimiting("Fixed")]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStateById(int id)
        {
            var state = await _stateService.GetStateByIdAsync(id);
            return Ok(state);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStates()
        {
            var states = await _stateService.GetAllStatesAsync();
            return Ok(states);
        }

        [HttpPost]
        public async Task<IActionResult> CreateState([FromBody] CreateStateRequest request)
        {
            var createdState = await _stateService.CreateStateAsync(request);
            return Created("", createdState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateState(int id, [FromBody] UpdateStateRequest request)
        {
            var updatedState = await _stateService.UpdateStateAsync(id, request);
            return Ok(updatedState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteState(int id)
        {
            var deletedState = await _stateService.DeleteStateAsync(id);
            return Ok(deletedState);
        }
    }
}
