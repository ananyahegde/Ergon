using AutoMapper;
using Ergon.DTOs.Payroll;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;

namespace Ergon.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IRepository<Guid, Payroll> _genericRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public PayrollService(IPayrollRepository payrollRepository, IRepository<Guid, Payroll> genericRepository, INotificationService notificationService, IMapper mapper)
        {
            _payrollRepository = payrollRepository;
            _genericRepository = genericRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        private static void ValidateMonthYear(int month, int year)
        {
            if (month < 1 || month > 12)
                throw new BadRequestException("Month must be between 1 and 12.");
            if (year < 2000 || year > 2100)
                throw new BadRequestException("Year must be between 2000 and 2100.");
        }

        private static (decimal earnings, decimal deductions) CalculateBaseComponents(Employee employee)
        {
            if (employee.SalaryStructure?.SalaryComponents == null || !employee.SalaryStructure.SalaryComponents.Any())
                throw new BadRequestException($"Employee {employee.EmployeeId} has no salary structure configured.");

            var earnings = employee.SalaryStructure.SalaryComponents
                .Where(sc => sc.ComponentType == SalaryComponentEnum.Earning)
                .Sum(sc => sc.Amount);

            if (earnings <= 0)
                throw new BadRequestException($"Employee {employee.EmployeeId} has no earning components in their salary structure.");

            var deductions = employee.SalaryStructure.SalaryComponents
                .Where(sc => sc.ComponentType == SalaryComponentEnum.Deduction)
                .Sum(sc => sc.Amount);

            return (earnings, deductions);
        }

        private static (decimal monthlyTDS, decimal unpaidDeduction) CalculateDynamicDeductions(
            Employee employee, decimal earnings, List<TaxSlab> taxSlabs, List<Leave> leaves)
        {
            var unpaidDays = leaves
                .Where(l => l.EmployeeId == employee.EmployeeId)
                .Sum(l => l.IsHalfDay ? 0.5m : 1m);
            var unpaidDeduction = unpaidDays * (earnings / 30);

            var annualSalary = earnings * 12;
            var slab = taxSlabs.LastOrDefault(t => annualSalary >= t.MinIncome);
            var monthlyTDS = slab != null ? (annualSalary * (slab.TaxPercentage / 100m)) / 12 : 0;

            return (monthlyTDS, unpaidDeduction);
        }

        private async Task<Payroll> BuildPayrollAsync(Employee employee, int month, int year, List<TaxSlab> taxSlabs, List<Leave> leaves)
        {
            var (earnings, deductions) = CalculateBaseComponents(employee);
            var (monthlyTDS, unpaidDeduction) = CalculateDynamicDeductions(employee, earnings, taxSlabs, leaves);

            var netSalary = earnings - deductions - unpaidDeduction - monthlyTDS;
            if (netSalary < 0)
                throw new BadRequestException($"Computed net salary for employee {employee.EmployeeId} is negative. Review salary structure and deductions.");

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

            await _payrollRepository.AddPayrollAsync(payroll);
            await _payrollRepository.AddPayrollComponentsAsync(payrollComponents);

            return payroll;
        }

        public async Task<PagedPayrollResponse> GetAllPayrollsAsync(GetAllPayrollsRequest request)
        {
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var (items, totalCount) = await _payrollRepository.GetPagedPayrollsAsync(request);

            return new PagedPayrollResponse
            {
                Items = _mapper.Map<List<PayrollResponse>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PayrollResponse> GetPayrollByIdAsync(Guid payrollId)
        {
            var payroll = await _payrollRepository.GetPayrollByIdAsync(payrollId);
            if (payroll == null) throw new NotFoundException("Payroll not found.");
            return _mapper.Map<PayrollResponse>(payroll);
        }

        public async Task<PayrollResponse> CreatePayrollAsync(CreatePayrollRequest request)
        {
            ValidateMonthYear(request.Month, request.Year);

            var employee = await _payrollRepository.GetEmployeeWithSalaryAsync(request.EmployeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");
            if (employee.EmploymentStatus != EmploymentStatusEnum.Active)
                throw new BadRequestException("Payroll can only be created for active employees.");

            var exists = await _payrollRepository.PayrollExistsAsync(request.EmployeeId, request.Month, request.Year);
            if (exists) throw new ConflictException("Payroll already exists for this employee for this month.");

            var taxSlabs = await _payrollRepository.GetTaxSlabsAsync();
            var leaves = await _payrollRepository.GetUnpaidLeavesAsync(request.EmployeeId, request.Month, request.Year);

            var payroll = await BuildPayrollAsync(employee, request.Month, request.Year, taxSlabs, leaves);
            await _payrollRepository.SaveChangesAsync();

            return _mapper.Map<PayrollResponse>(payroll);
        }

        public async Task<IEnumerable<PayrollResponse>> GetUnapprovedPayrollsAsync()
        {
            var payrolls = await _payrollRepository.GetUnapprovedPayrollsAsync();
            return _mapper.Map<List<PayrollResponse>>(payrolls);
        }

        public async Task RunPayrollAsync(Guid createdBy, int month, int year)
        {
            ValidateMonthYear(month, year);

            var now = DateTime.Now;
            if (year > now.Year || (year == now.Year && month > now.Month))
                throw new BadRequestException("Cannot run payroll for a future month.");

            var employees = await _payrollRepository.GetActiveEmployeesWithSalaryAsync();
            if (!employees.Any()) throw new NotFoundException("No active employees found.");

            var taxSlabs = await _payrollRepository.GetTaxSlabsAsync();
            var allLeaves = await _payrollRepository.GetUnpaidLeavesAsync(null, month, year);

            foreach (var employee in employees)
            {
                if (employee.SalaryStructure?.SalaryComponents == null || !employee.SalaryStructure.SalaryComponents.Any())
                    continue;

                var exists = await _payrollRepository.PayrollExistsAsync(employee.EmployeeId, month, year);
                if (exists) continue;

                await BuildPayrollAsync(employee, month, year, taxSlabs, allLeaves);
            }

            await _payrollRepository.SaveChangesAsync();
        }

        public async Task BulkApprovePayrollsAsync(Guid approvedBy, BulkApprovePayrollRequest request)
        {
            ValidateMonthYear(request.Month, request.Year);

            var now = DateTime.Now;
            if (request.Year > now.Year || (request.Year == now.Year && request.Month > now.Month))
                throw new BadRequestException("Cannot approve payroll for a future month.");

            var payrolls = await _payrollRepository.GetPayrollsByMonthYearAsync(
                request.Month, request.Year, PayrollStatusEnum.ApprovalPending);

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
            var payroll = await _payrollRepository.GetPayrollByIdAsync(payrollId);
            if (payroll == null) throw new NotFoundException("Payroll not found.");
            if (payroll.PayrollStatus == PayrollStatusEnum.Approved)
                throw new BadRequestException("Cannot delete an approved payroll.");

            await _genericRepository.Delete(payrollId);
            await _payrollRepository.SaveChangesAsync();
            return _mapper.Map<PayrollResponse>(payroll);
        }
    }
}
