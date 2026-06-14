using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Attendance;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ErgonContext _context;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IMapper _mapper;

        public AttendanceService(ErgonContext context, IAttendanceRepository attendanceRepository, IMapper mapper)
        {
            _context = context;
            _attendanceRepository = attendanceRepository;
            _mapper = mapper;
        }

        public async Task<PagedAttendanceResponse> GetAllAttendancesAsync(GetAllAttendancesRequest request)
        {
            var q = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(a => a.EmployeeId == request.EmployeeId.Value);

            if (request.Month.HasValue)
                q = q.Where(a => a.Date.Month == request.Month.Value);

            if (request.Year.HasValue)
                q = q.Where(a => a.Date.Year == request.Year.Value);

            if (request.Status != null && request.Status.Any())
            {
                var statuses = request.Status
                    .Select(s => Enum.TryParse<AttendanceStatusEnum>(s, out var status) ? status : (AttendanceStatusEnum?)null)
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                q = q.Where(a => statuses.Contains(a.AttendanceStatus));
            }

            q = q.OrderByDescending(a => a.Date);

            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var attendances = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedAttendanceResponse
            {
                Items = _mapper.Map<List<AttendanceResponse>>(attendances),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<AttendanceResponse> GetAttendanceByIdAsync(Guid attendanceId)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
            if (attendance == null) throw new NotFoundException("Attendance record not found.");
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceResponse> ClockInAsync(Guid employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            var existingRecord = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);
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

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceResponse> ClockOutAsync(Guid attendanceId, Guid employeeId)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Shift)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);

            if (attendance == null) throw new NotFoundException("Attendance record not found.");
            if (attendance.EmployeeId != employeeId) throw new ForbiddenException("You can only clock out your own attendance.");

            var clockOutTime = TimeOnly.FromDateTime(DateTime.Now);
            var totalHours = (clockOutTime.ToTimeSpan() - attendance.ClockInTime.ToTimeSpan()).TotalHours;

            attendance.ClockOutTime = clockOutTime;
            attendance.LateExit = clockOutTime < attendance.Employee.Shift.EndTime;
            attendance.AttendanceStatus = totalHours >= 4 && totalHours < 8
                ? AttendanceStatusEnum.HalfDay
                : AttendanceStatusEnum.Present;
            attendance.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return _mapper.Map<AttendanceResponse>(attendance);
        }

        public async Task<AttendanceTodaySummaryResponse> GetTodaySummaryAsync()
        {
            return await _attendanceRepository.GetTodaySummaryAsync();
        }
    }
}
