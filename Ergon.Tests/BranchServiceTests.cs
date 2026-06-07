using Moq;
using AutoMapper;
using Ergon.Services;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.DTOs.Branch;
using Ergon.Exceptions;

namespace Ergon.Tests
{
    public class BranchServiceTests
    {
        private Mock<IRepository<int, Branch>> _mockRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private BranchService _branchService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<int, Branch>>();
            _mockMapper = new Mock<IMapper>();
            _branchService = new BranchService(_mockRepo.Object, _mockMapper.Object);
        }


        [Test]
        public async Task CreateBranch_ValidRequest_ReturnsBranchResponse()
        {
            var request = new CreateBranchRequest { BranchName = "TestBranch" };
            var branch = new Branch { BranchId = 1, BranchName = "TestBranch" };
            var branchResponse = new BranchResponse { BranchId = 1, BranchName = "TestBranch" };

            _mockMapper.Setup(m => m.Map<Branch>(request)).Returns(branch);
            _mockRepo.Setup(r => r.Create(branch)).ReturnsAsync(branch);
            _mockMapper.Setup(m => m.Map<BranchResponse>(branch)).Returns(branchResponse);

            var result = await _branchService.CreateBranchAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BranchName, Is.EqualTo("TestBranch"));
        }


        [Test]
        public async Task GetBranchById_BranchExists_ReturnsBranchResponse()
        {
            var branch = new Branch { BranchId = 1, BranchName = "TestBranch" };
            var branchResponse = new BranchResponse { BranchId = 1, BranchName = "TestBranch" };

            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync(branch);
            _mockMapper.Setup(m => m.Map<BranchResponse>(branch)).Returns(branchResponse);

            var result = await _branchService.GetBranchByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BranchName, Is.EqualTo("TestBranch"));
        }


        [Test]
        public async Task GetBranchById_BranchNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Branch?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _branchService.GetBranchByIdAsync(1));
        }


        [Test]
        public async Task UpdateBranch_BranchNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Branch?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _branchService.UpdateBranchAsync(1, new UpdateBranchRequest()));
        }

        [Test]
        public async Task DeleteBranch_BranchNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(1)).ReturnsAsync((Branch?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _branchService.DeleteBranchAsync(1));
        }
    }
}
