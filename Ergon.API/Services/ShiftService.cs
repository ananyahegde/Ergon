using AutoMapper;
using Ergon.DTOs.Shift;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IRepository<int, Shift> _repository;
        private readonly IMapper _mapper;

        public ShiftService(IRepository<int, Shift> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ShiftResponse> GetShiftByIdAsync(int id)
        {
            var shift = await _repository.Get(id);
            if (shift == null) throw new NotFoundException("Shift not found.");
            return _mapper.Map<ShiftResponse>(shift);
        }

        public async Task<IEnumerable<ShiftResponse>> GetAllShiftsAsync()
        {
            var shifts = await _repository.GetAll();
            return _mapper.Map<List<ShiftResponse>>(shifts);
        }

        public async Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request)
        {
            request.ShiftName = request.ShiftName.ToPascalCase();
            var shift = _mapper.Map<Shift>(request);
            var createdShift = await _repository.Create(shift);
            return _mapper.Map<ShiftResponse>(createdShift);
        }

        public async Task<ShiftResponse> UpdateShiftAsync(int id, UpdateShiftRequest request)
        {
            var shift = await _repository.Get(id);
            if (shift == null) throw new NotFoundException("Shift not found.");

            request.ShiftName = request.ShiftName.ToPascalCase();
            _mapper.Map(request, shift);
            var updatedShift = await _repository.Update(id, shift);
            return _mapper.Map<ShiftResponse>(updatedShift);
        }

        public async Task<ShiftResponse> DeleteShiftAsync(int id)
        {
            var shift = await _repository.Get(id);
            if (shift == null) throw new NotFoundException("Shift not found.");
            var deletedShift = await _repository.Delete(id);
            return _mapper.Map<ShiftResponse>(deletedShift);
        }
    }
}
