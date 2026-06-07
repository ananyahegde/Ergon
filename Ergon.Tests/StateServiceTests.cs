using AutoMapper;
using Ergon.DTOs.State;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Moq;

namespace Ergon.Tests
{
    public class StateServiceTests
    {
        private Mock<IRepository<int, State>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private StateService _stateService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, State>>();
            _mockMapper = new Mock<IMapper>();
            _stateService = new StateService(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetStateById_StateExists_ReturnsStateResponse()
        {
            var state = new State { StateId = 1, StateName = "Karnataka" };
            var stateResponse = new StateResponse { StateId = 1, StateName = "Karnataka" };
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(state);
            _mockMapper.Setup(m => m.Map<StateResponse>(state)).Returns(stateResponse);

            var result = await _stateService.GetStateByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StateName, Is.EqualTo("Karnataka"));
        }

        [Test]
        public async Task GetStateById_StateNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((State?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _stateService.GetStateByIdAsync(1));
        }

        [Test]
        public async Task CreateState_ValidRequest_ReturnsStateResponse()
        {
            var request = new CreateStateRequest { StateName = "Karnataka" };
            var state = new State { StateId = 1, StateName = "Karnataka" };
            var stateResponse = new StateResponse { StateId = 1, StateName = "Karnataka" };
            _mockMapper.Setup(m => m.Map<State>(request)).Returns(state);
            _mockRepo.Setup(r => r.Create(state)).ReturnsAsync(state);
            _mockMapper.Setup(m => m.Map<StateResponse>(state)).Returns(stateResponse);

            var result = await _stateService.CreateStateAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StateName, Is.EqualTo("Karnataka"));
        }

        [Test]
        public async Task UpdateState_StateNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((State?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _stateService.UpdateStateAsync(1, new UpdateStateRequest()));
        }

        [Test]
        public async Task DeleteState_StateNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((State?)null);
            Assert.ThrowsAsync<NotFoundException>(() => _stateService.DeleteStateAsync(1));
        }
    }
}
