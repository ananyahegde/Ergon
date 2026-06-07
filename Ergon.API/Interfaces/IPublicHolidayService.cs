using Ergon.DTOs.PublicHoliday;

namespace Ergon.Interfaces
{
    public interface IPublicHolidayService
    {
        Task<PublicHolidayResponse> GetPublicHolidayByIdAsync(int id);
        Task<IEnumerable<PublicHolidayResponse>> GetAllPublicHolidaysAsync();
        Task<PublicHolidayResponse> CreatePublicHolidayAsync(CreatePublicHolidayRequest request);
        Task<PublicHolidayResponse> UpdatePublicHolidayAsync(int id, UpdatePublicHolidayRequest request);
        Task<PublicHolidayResponse> DeletePublicHolidayAsync(int id);
    }
}
