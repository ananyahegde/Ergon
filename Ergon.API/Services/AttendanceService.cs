// AttendanceService.cs
using AutoMapper;
using Ergon.DTOs.Attendance;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IMapper _mapper;

        public AttendanceService(IAttendanceRepository attendanceRepository, IMapper mapper)
        {
            _attendanceRepository = attendanceRepository;
            _mapper = mapper;
        }

        public async Task<PagedAttendanceResponse> GetAllAttendancesAsync(GetAllAttendancesRequest request)
        {
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var (items, totalCount) = await _attendanceRepository.GetPagedAttendancesAsync(request);

            return new PagedAttendanceResponse
            {
                Items = _mapper.Map<List<AttendanceResponse>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<AttendanceResponse> GetAttendanceByIdAsync(Guid attendanceId)
        {
            var attendance = await _attendanceRepository.GetAttendanceByIdAsync(attendanceId);
            if (attendance == null) throw new NotFoundException("Attendance record not found.");
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceResponse> ClockInAsync(Guid employeeId)
        {
            var employee = await _attendanceRepository.GetEmployeeWithShiftAsync(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            if (employee.EmploymentStatus != EmploymentStatusEnum.Active)
                throw new BadRequestException("Only active employees can clock in.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            var existingRecord = await _attendanceRepository.GetAttendanceForDateAsync(employeeId, today);
            if (existingRecord != null) throw new ConflictException("Already clocked in today.");

            var clockInTime = TimeOnly.FromDateTime(DateTime.Now);
            var isLateEntry = clockInTime > employee.Shift.StartTime;

            var attendance = new Attendance
            {
                AttendanceId = Guid.NewGuid(),
                EmployeeId = employeeId,
                Date = today,
                ClockInTime = clockInTime,
                AttendanceStatus = AttendanceStatusEnum.Incomplete,
                LateEntry = isLateEntry,
                LateExit = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _attendanceRepository.AddAttendanceAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceResponse> ClockOutAsync(Guid attendanceId, Guid employeeId)
        {
            var attendance = await _attendanceRepository.GetAttendanceWithShiftAsync(attendanceId);

            if (attendance == null) throw new NotFoundException("Attendance record not found.");
            if (attendance.EmployeeId != employeeId) throw new ForbiddenException("You can only clock out your own attendance.");
            if (attendance.ClockOutTime != null) throw new BadRequestException("Already clocked out for this record.");

            var clockOutTime = TimeOnly.FromDateTime(DateTime.Now);
            if (clockOutTime.ToTimeSpan() < attendance.ClockInTime.ToTimeSpan())
                throw new BadRequestException("Clock out time cannot be before clock in time.");

            var totalHours = (clockOutTime.ToTimeSpan() - attendance.ClockInTime.ToTimeSpan()).TotalHours;

            attendance.ClockOutTime = clockOutTime;
            attendance.LateExit = clockOutTime < attendance.Employee.Shift.EndTime;
            attendance.AttendanceStatus = totalHours < 1
                ? AttendanceStatusEnum.Absent
                : totalHours < 4
                    ? AttendanceStatusEnum.HalfDay
                    : AttendanceStatusEnum.Present;
            attendance.UpdatedAt = DateTime.Now;

            await _attendanceRepository.SaveChangesAsync();
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync()
        {
            return await _attendanceRepository.GetTodaySummaryAsync();
        }
    }
}
