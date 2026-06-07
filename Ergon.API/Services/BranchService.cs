using Ergon.Interfaces;
using Ergon.Models;
using Ergon.DTOs.Branch;
using AutoMapper;
using Ergon.Exceptions;

namespace Ergon.Services
{
    public class BranchService : IBranchService
    {
        private readonly IRepository<int, Branch> _repository;
        private readonly IMapper _mapper;

        public BranchService(IRepository<int, Branch> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request)
        {
            var branch = _mapper.Map<Branch>(request);
            var createdBranch = await _repository.Create(branch);
            return _mapper.Map<BranchResponse>(createdBranch);
        }

        public async Task<BranchResponse> GetBranchByIdAsync(int id)
        {
            var branch = await _repository.Get(id);
            if (branch == null)
                throw new NotFoundException("Branch not found.");

            return _mapper.Map<BranchResponse>(branch);
        }

        public async Task<IEnumerable<BranchResponse>> GetAllBranchesAsync()
        {
            var branches = await _repository.GetAll();
            return _mapper.Map<List<BranchResponse>>(branches);
        }

        public async Task<BranchResponse> UpdateBranchAsync(int id, UpdateBranchRequest request)
        {
            var branch = await _repository.Get(id);

            if (branch == null)
                throw new NotFoundException("Branch not found.");

            if (string.IsNullOrEmpty(request.BranchName))
                throw new BadRequestException("Branch Name cannot be empty.");

            _mapper.Map(request, branch);
            var updatedBranch = await _repository.Update(id, branch);
            return _mapper.Map<BranchResponse>(updatedBranch);
        }

        public async Task<BranchResponse> DeleteBranchAsync(int id)
        {
            var branch = await _repository.Get(id);
            if (branch == null)
                throw new NotFoundException("Branch not found.");

            var deletedBranch = await _repository.Delete(id);

            return _mapper.Map<BranchResponse>(deletedBranch);
        }
    }
}
