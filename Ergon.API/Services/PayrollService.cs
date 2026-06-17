using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Payroll;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly ErgonContext _context;
        private readonly IPayrollRepository _payrollRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public PayrollService(ErgonContext context, IPayrollRepository payrollRepository, INotificationService notificationService, IMapper mapper)
        {
            _context = context;
            _payrollRepository = payrollRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        private async Task CreatePayrollForEmployeeAsync(Employee employee, int month, int year, List<TaxSlab> taxSlabs, List<Leave> leaves)
        {
            var earnings = employee.SalaryStructure.SalaryComponents
                .Where(sc => sc.ComponentType == SalaryComponentEnum.Earning)
                .Sum(sc => sc.Amount);

            var deductions = employee.SalaryStructure.SalaryComponents
                .Where(sc => sc.ComponentType == SalaryComponentEnum.Deduction)
                .Sum(sc => sc.Amount);

            // Unpaid leave deduction — filter in memory
            var unpaidDays = leaves
                .Where(l => l.EmployeeId == employee.EmployeeId)
                .Sum(l => l.IsHalfDay ? 0.5m : 1m);
            var dailySalary = earnings / 30;
            var unpaidDeduction = unpaidDays * dailySalary;

            // TDS
            var annualSalary = earnings * 12;
            var slab = taxSlabs.LastOrDefault(t => annualSalary >= t.MinIncome);
            var annualTax = slab != null ? annualSalary * (slab.TaxPercentage / 100m) : 0;
            var monthlyTDS = annualTax / 12;

            var netSalary = earnings - deductions - unpaidDeduction - monthlyTDS;

            var payroll = new Payroll
            {
                PayrollId = Guid.NewGuid(),
                EmployeeId = employee.EmployeeId,
                Month = month,
                Year = year,
                NetSalary = netSalary,
                PayrollStatus = PayrollStatusEnum.ApprovalPending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _context.Payrolls.AddAsync(payroll);

            var payrollComponents = employee.SalaryStructure.SalaryComponents.Select(sc => new PayrollComponent
            {
                PayrollComponentId = Guid.NewGuid(),
                PayrollId = payroll.PayrollId,
                PayrollComponentName = sc.ComponentName,
                PayrollComponentType = sc.ComponentType == SalaryComponentEnum.Earning
                    ? PayrollComponentEnum.Earning
                    : PayrollComponentEnum.Deduction,
                Amount = sc.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();

            if (unpaidDeduction > 0)
            {
                payrollComponents.Add(new PayrollComponent
                {
                    PayrollComponentId = Guid.NewGuid(),
                    PayrollId = payroll.PayrollId,
                    PayrollComponentName = "Unpaid Leave Deduction",
                    PayrollComponentType = PayrollComponentEnum.Deduction,
                    Amount = unpaidDeduction,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            payrollComponents.Add(new PayrollComponent
            {
                PayrollComponentId = Guid.NewGuid(),
                PayrollId = payroll.PayrollId,
                PayrollComponentName = "TDS",
                PayrollComponentType = PayrollComponentEnum.Deduction,
                Amount = monthlyTDS,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            await _context.PayrollComponents.AddRangeAsync(payrollComponents);
        }


        public async Task<PagedPayrollResponse> GetAllPayrollsAsync(GetAllPayrollsRequest request)
        {
            var q = _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                q = q.Where(p => p.EmployeeId == request.EmployeeId.Value);

            if (request.Month.HasValue)
                q = q.Where(p => p.Month == request.Month.Value);

            if (request.Year.HasValue)
                q = q.Where(p => p.Year == request.Year.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<PayrollStatusEnum>(request.Status, out var status))
                    q = q.Where(p => p.PayrollStatus == status);
            }

            q = q.OrderByDescending(p => p.Year).ThenByDescending(p => p.Month);

            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var payrolls = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedPayrollResponse
            {
                Items = _mapper.Map<List<PayrollResponse>>(payrolls),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<PayrollResponse> GetPayrollByIdAsync(Guid payrollId)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);

            if (payroll == null) throw new NotFoundException("Payroll not found.");

            return _mapper.Map<PayrollResponse>(payroll);
        }

        public async Task<PayrollResponse> CreatePayrollAsync(CreatePayrollRequest request)
        {
            var employee = await _context.Employees
                .Include(e => e.SalaryStructure)
                    .ThenInclude(ss => ss.SalaryComponents)
                .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var existing = await _context.Payrolls
                .AnyAsync(p => p.EmployeeId == request.EmployeeId && p.Month == request.Month && p.Year == request.Year);
            if (existing) throw new ConflictException("Payroll already exists for this employee for this month.");

            var taxSlabs = await _context.TaxSlabs.OrderBy(t => t.MinIncome).ToListAsync();

            var leaves = await _context.Leaves
                .Where(l => l.EmployeeId == request.EmployeeId
                    && l.Status == LeaveStatusEnum.Approved
                    && l.LeaveTypeId == 4
                    && l.FromDate.Month == request.Month
                    && l.FromDate.Year == request.Year)
                .ToListAsync();

            await CreatePayrollForEmployeeAsync(employee, request.Month, request.Year, taxSlabs, leaves);
            await _context.SaveChangesAsync();

            var payroll = await _context.Payrolls
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync(p => p.EmployeeId == request.EmployeeId);

            return await GetPayrollByIdAsync(payroll.PayrollId);
        }

        public async Task<IEnumerable<PayrollResponse>> GetUnapprovedPayrollsAsync()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .Where(p => p.PayrollStatus == PayrollStatusEnum.ApprovalPending)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<List<PayrollResponse>>(payrolls);
        }

        public async Task RunPayrollAsync(Guid createdBy, int month, int year)
        {
            var employees = await _context.Employees
                .Include(e => e.SalaryStructure)
                    .ThenInclude(ss => ss.SalaryComponents)
                .Where(e => e.EmploymentStatus == EmploymentStatusEnum.Active)
                .ToListAsync();

            var taxSlabs = await _context.TaxSlabs.OrderBy(t => t.MinIncome).ToListAsync();

            var allLeaves = await _context.Leaves
                .Where(l => l.Status == LeaveStatusEnum.Approved
                    && l.LeaveTypeId == 4
                    && l.FromDate.Month == month
                    && l.FromDate.Year == year)
                .ToListAsync();

            foreach (var employee in employees)
            {
                var existing = await _context.Payrolls
                    .AnyAsync(p => p.EmployeeId == employee.EmployeeId && p.Month == month && p.Year == year);
                if (existing) continue;

                await CreatePayrollForEmployeeAsync(employee, month, year, taxSlabs, allLeaves);
            }

            await _context.SaveChangesAsync();
        }

        public async Task BulkApprovePayrollsAsync(Guid approvedBy, BulkApprovePayrollRequest request)
        {
            var payrolls = await _context.Payrolls
                .Where(p => p.Month == request.Month && p.Year == request.Year && p.PayrollStatus == PayrollStatusEnum.ApprovalPending)
                .ToListAsync();

            if (!payrolls.Any()) throw new NotFoundException("No unapproved payrolls found for this month.");

            await _payrollRepository.BulkApprovePayrollsAsync(request.Month, request.Year, approvedBy);

            foreach (var payroll in payrolls)
            {
                await _notificationService.CreateNotificationAsync(
                    payroll.EmployeeId,
                    "Payroll Approved",
                    $"Your payroll for {request.Month}/{request.Year} has been approved."
                );
            }
        }

        public async Task<PayrollResponse> DeletePayrollAsync(Guid payrollId)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);
            if (payroll == null) throw new NotFoundException("Payroll not found.");
            if (payroll.PayrollStatus == PayrollStatusEnum.Approved)
                throw new BadRequestException("Cannot delete an approved payroll.");
            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();
            return _mapper.Map<PayrollResponse>(payroll);
        }
    }
}
