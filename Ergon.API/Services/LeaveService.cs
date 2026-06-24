using AutoMapper;
using Ergon.DTOs.Leave;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly IRepository<Guid, Leave> _repository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        private static readonly int[] HrRoleIds = { 1, 2 };

        public LeaveService(
            IRepository<Guid, Leave> repository,
            ILeaveRepository leaveRepository,
            INotificationService notificationService,
            IMapper mapper)
        {
            _repository = repository;
            _leaveRepository = leaveRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<PagedLeaveResponse> GetAllLeavesAsync(GetAllLeavesRequest request)
        {
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var (leaves, totalCount) = await _leaveRepository.GetAllAsync(request);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalCount > 0 && pageNumber > totalPages)
            {
                request.PageNumber = totalPages;
                (leaves, _) = await _leaveRepository.GetAllAsync(request);
            }

            return new PagedLeaveResponse
            {
                Items = _mapper.Map<List<LeaveResponse>>(leaves),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveResponse> GetLeaveByIdAsync(Guid leaveId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new NotFoundException("Leave not found.");
            return _mapper.Map<LeaveResponse>(leave);
        }

        public async Task<LeaveResponse> ApplyLeaveAsync(Guid employeeId, CreateLeaveRequest request)
        {
            var isActive = await _leaveRepository.EmployeeExistsAndActiveAsync(employeeId);
            if (!isActive)
                throw new BadRequestException("Only active employees can apply for leave.");

            if (request.ToDate < request.FromDate)
                throw new BadRequestException("To date cannot be before from date.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (request.FromDate < today)
                throw new BadRequestException("Cannot apply leave for past dates.");

            if (request.IsHalfDay)
            {
                if (request.FromDate != request.ToDate)
                    throw new BadRequestException("Half day leave must be for a single day only.");

                var halfDayCount = await _leaveRepository.CountHalfDaysOnDateAsync(employeeId, request.FromDate);
                if (halfDayCount >= 2)
                    throw new ConflictException("You already have two half day leaves on this date. Please contact HR for further changes.");

                if (halfDayCount == 1)
                {
                    if (request.FromDate <= today)
                        throw new BadRequestException("Cannot apply another half day for a date that has already started. Please contact HR.");
                }
            }
            else
            {
                var overlapping = await _leaveRepository.HasOverlappingLeaveAsync(employeeId, request.FromDate, request.ToDate);
                if (overlapping)
                    throw new ConflictException("You already have a leave request for these dates.");
            }

            var entitlement = await _leaveRepository.GetEntitlementDaysAsync(
                (await GetEmployeeLeaveEntitlementIdAsync(employeeId)),
                request.LeaveTypeId);

            var used = await _leaveRepository.GetUsedLeaveDaysAsync(employeeId, request.LeaveTypeId);
            var requested = request.IsHalfDay ? 0.5m : (request.ToDate.DayNumber - request.FromDate.DayNumber + 1);

            if (used + requested > entitlement)
                throw new BadRequestException($"Insufficient leave balance. You have {entitlement - used} day(s) remaining for this leave type.");

            var leave = _mapper.Map<Leave>(request);
            leave.LeaveId = Guid.NewGuid();
            leave.EmployeeId = employeeId;
            leave.Status = LeaveStatusEnum.Open;
            leave.AppliedAt = DateTime.UtcNow;
            leave.CreatedAt = DateTime.Now;
            leave.UpdatedAt = DateTime.Now;

            await _repository.Create(leave);
            return await GetLeaveByIdAsync(leave.LeaveId);
        }

        public async Task<LeaveResponse> ActionLeaveAsync(Guid leaveId, Guid actionedBy, LeaveActionRequest request)
        {
            if (request.Action != LeaveStatusEnum.Approved && request.Action != LeaveStatusEnum.Rejected)
                throw new BadRequestException("Invalid action. Only Approved or Rejected are allowed.");

            var actor = await _leaveRepository.GetActioningEmployeeAsync(actionedBy);
            if (actor == null)
                throw new NotFoundException("Actioning employee not found.");

            if (!HrRoleIds.Contains(actor.RoleId))
                throw new ForbiddenException("Only HR or HR Admin can action leave requests.");

            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new NotFoundException("Leave not found.");

            if (leave.Status != LeaveStatusEnum.Open)
                throw new BadRequestException("Only open leave requests can be actioned.");

            leave.Status = request.Action;
            leave.ActionedBy = actionedBy;
            leave.UpdatedAt = DateTime.Now;

            await _repository.Update(leaveId, leave);

            await _notificationService.CreateNotificationAsync(
                leave.EmployeeId,
                "Leave Request Update",
                $"Your leave request from {leave.FromDate} to {leave.ToDate} has been {leave.Status}."
            );

            return await GetLeaveByIdAsync(leaveId);
        }

        public async Task<LeaveResponse> CancelLeaveAsync(Guid leaveId, Guid employeeId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new NotFoundException("Leave not found.");

            if (leave.EmployeeId != employeeId)
                throw new ForbiddenException("You can only cancel your own leave requests.");

            if (leave.Status == LeaveStatusEnum.Rejected || leave.Status == LeaveStatusEnum.Cancelled || leave.Status == LeaveStatusEnum.Approved)
                throw new BadRequestException("This leave request cannot be cancelled. Please contact HR.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (leave.FromDate <= today)
                throw new BadRequestException("Cannot cancel a leave that has already started. Please contact HR.");

            leave.Status = LeaveStatusEnum.Cancelled;
            leave.UpdatedAt = DateTime.Now;

            await _repository.Update(leaveId, leave);
            return await GetLeaveByIdAsync(leaveId);
        }

        public async Task<IEnumerable<LeaveBalanceResponse>> GetLeaveBalancesAsync()
        {
            return await _leaveRepository.GetLeaveBalancesAsync();
        }

        private async Task<int> GetEmployeeLeaveEntitlementIdAsync(Guid employeeId)
        {
            var entitlementId = await _leaveRepository.GetEmployeeLeaveEntitlementIdAsync(employeeId);
            if (entitlementId == null)
                throw new NotFoundException("Leave entitlement not found for this employee.");
            return entitlementId.Value;
        }
    }
}
