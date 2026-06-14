using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Leave;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ErgonContext _context;
        private readonly ILeaveRepository _leaveRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public LeaveService(ErgonContext context, ILeaveRepository leaveRepository, INotificationService notificationService, IMapper mapper)
        {
            _context = context;
            _leaveRepository = leaveRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<PagedLeaveResponse> GetAllLeavesAsync(GetAllLeavesRequest request)
        {
            var q = _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.ActionedByEmployee)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(l => l.EmployeeId == request.EmployeeId.Value);

            if (request.LeaveTypeId.HasValue)
                q = q.Where(l => l.LeaveTypeId == request.LeaveTypeId.Value);

            if (request.Month.HasValue)
                q = q.Where(l => l.FromDate.Month == request.Month.Value);

            if (request.Year.HasValue)
                q = q.Where(l => l.FromDate.Year == request.Year.Value);

            if (request.Statuses != null && request.Statuses.Any())
            {
                var statuses = request.Statuses
                    .Select(s => Enum.TryParse<LeaveStatusEnum>(s, out var status) ? status : (LeaveStatusEnum?)null)
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                q = q.Where(l => statuses.Contains(l.Status));
            }

            q = q.OrderByDescending(l => l.AppliedAt);

            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var leaves = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedLeaveResponse
            {
                Items = _mapper.Map<List<LeaveResponse>>(leaves),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveResponse> GetLeaveByIdAsync(Guid leaveId)
        {
            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.ActionedByEmployee)
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
            if (leave == null) throw new NotFoundException("Leave not found.");
            return _mapper.Map<LeaveResponse>(leave);
        }

        public async Task<LeaveResponse> ApplyLeaveAsync(Guid employeeId, CreateLeaveRequest request)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            if (request.FromDate < DateOnly.FromDateTime(DateTime.Now))
                throw new BadRequestException("Cannot apply leave for past dates.");

            if (request.ToDate < request.FromDate)
                throw new BadRequestException("To date cannot be before from date.");

            var overlapping = await _context.Leaves
                .AnyAsync(l => l.EmployeeId == employeeId
                    && l.Status != LeaveStatusEnum.Rejected
                    && l.Status != LeaveStatusEnum.Cancelled
                    && l.FromDate <= request.ToDate
                    && l.ToDate >= request.FromDate);

            if (overlapping) throw new ConflictException("You already have a leave request for these dates.");

            var leave = _mapper.Map<Leave>(request);
            leave.LeaveId = Guid.NewGuid();
            leave.EmployeeId = employeeId;
            leave.Status = LeaveStatusEnum.Open;
            leave.AppliedAt = DateTime.UtcNow;
            leave.CreatedAt = DateTime.Now;
            leave.UpdatedAt = DateTime.Now;

            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
            return await GetLeaveByIdAsync(leave.LeaveId);
        }

        public async Task<LeaveResponse> ActionLeaveAsync(Guid leaveId, Guid actionedBy, LeaveActionRequest request)
        {
            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
            if (leave == null) throw new NotFoundException("Leave not found.");

            if (leave.Status != LeaveStatusEnum.Open)
                throw new BadRequestException("Only open leave requests can be actioned.");

            leave.Status = request.Action;
            leave.ActionedBy = actionedBy;
            leave.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                leave.EmployeeId,
                "Leave Request Update",
                $"Your leave request from {leave.FromDate} to {leave.ToDate} has been {leave.Status}."
            );

            return await GetLeaveByIdAsync(leaveId);
        }

        public async Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync()
        {
            return await _leaveRepository.GetLeaveBalancesAsync();
        }

        public async Task<LeaveResponse> CancelLeaveAsync(Guid leaveId, Guid employeeId)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null) throw new NotFoundException("Leave not found.");

            if (leave.EmployeeId != employeeId)
                throw new ForbiddenException("You can only cancel your own leave requests.");

            if (leave.Status == LeaveStatusEnum.Rejected || leave.Status == LeaveStatusEnum.Cancelled)
                throw new BadRequestException("This leave request cannot be cancelled.");

            leave.Status = LeaveStatusEnum.Cancelled;
            leave.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await GetLeaveByIdAsync(leaveId);
        }
    }
}
