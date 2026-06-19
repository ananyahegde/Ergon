using AutoMapper;
using Ergon.DTOs.PublicHoliday;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;

namespace Ergon.Services
{
    public class PublicHolidayService : IPublicHolidayService
    {
        private readonly IRepository<int, PublicHoliday> _repository;
        private readonly IMapper _mapper;

        public PublicHolidayService(IRepository<int, PublicHoliday> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PublicHolidayResponse> GetPublicHolidayByIdAsync(int id)
        {
            var publicHoliday = await _repository.Get(id);
            if (publicHoliday == null) throw new NotFoundException("Public holiday not found.");
            return _mapper.Map<PublicHolidayResponse>(publicHoliday);
        }

        public async Task<IEnumerable<PublicHolidayResponse>> GetAllPublicHolidaysAsync()
        {
            var publicHolidays = await _repository.GetAll();
            return _mapper.Map<List<PublicHolidayResponse>>(publicHolidays);
        }

        public async Task<PublicHolidayResponse> CreatePublicHolidayAsync(CreatePublicHolidayRequest request)
        {
            request.PublicHolidayName = request.PublicHolidayName.ToPascalCase();

            var all = await _repository.GetAll();
            if (all.Any(h => h.PublicHolidayDate == request.PublicHolidayDate))
                throw new ConflictException("A public holiday already exists on this date.");

            var publicHoliday = _mapper.Map<PublicHoliday>(request);
            var createdPublicHoliday = await _repository.Create(publicHoliday);
            return _mapper.Map<PublicHolidayResponse>(createdPublicHoliday);
        }

        public async Task<PublicHolidayResponse> UpdatePublicHolidayAsync(int id, UpdatePublicHolidayRequest request)
        {
            var publicHoliday = await _repository.Get(id);
            if (publicHoliday == null) throw new NotFoundException("Public holiday not found.");

            request.PublicHolidayName = request.PublicHolidayName.ToPascalCase();

            var all = await _repository.GetAll();
            if (all.Any(h => h.PublicHolidayDate == request.PublicHolidayDate && h.PublicHolidayId != id))
                throw new ConflictException("A public holiday already exists on this date.");

            _mapper.Map(request, publicHoliday);
            var updatedPublicHoliday = await _repository.Update(id, publicHoliday);
            return _mapper.Map<PublicHolidayResponse>(updatedPublicHoliday);
        }

        public async Task<PublicHolidayResponse> DeletePublicHolidayAsync(int id)
        {
            var publicHoliday = await _repository.Get(id);
            if (publicHoliday == null) throw new NotFoundException("Public holiday not found.");
            var deletedPublicHoliday = await _repository.Delete(id);
            return _mapper.Map<PublicHolidayResponse>(deletedPublicHoliday);
        }
    }
}
