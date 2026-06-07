using Ergon.DTOs.Shift;

namespace Ergon.Interfaces
{
    public interface IShiftService
    {
        Task<ShiftResponse> GetShiftByIdAsync(int id);
        Task<IEnumerable<ShiftResponse>> GetAllShiftsAsync();
        Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request);
        Task<ShiftResponse> UpdateShiftAsync(int id, UpdateShiftRequest request);
        Task<ShiftResponse> DeleteShiftAsync(int id);
    }
}
