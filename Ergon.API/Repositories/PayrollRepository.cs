using Ergon.Contexts;
using Ergon.DTOs.Payroll;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        protected ErgonContext _context;

        public PayrollRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeWithSalaryAsync(Guid employeeId)
        {
            return await _context.Employees
                .Include(e => e.SalaryStructure)
                    .ThenInclude(ss => ss.SalaryComponents)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<List<Employee>> GetActiveEmployeesWithSalaryAsync()
        {
            return await _context.Employees
                .Include(e => e.SalaryStructure)
                    .ThenInclude(ss => ss.SalaryComponents)
                .Where(e => e.EmploymentStatus == EmploymentStatusEnum.Active)
                .ToListAsync();
        }

        public async Task<bool> PayrollExistsAsync(Guid employeeId, int month, int year)
        {
            return await _context.Payrolls
                .AnyAsync(p => p.EmployeeId == employeeId && p.Month == month && p.Year == year);
        }

        public async Task<(List<Payroll> Items, int TotalCount)> GetPagedPayrollsAsync(GetAllPayrollsRequest request)
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

            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PayrollStatusEnum>(request.Status, out var status))
                q = q.Where(p => p.PayrollStatus == status);

            q = q.OrderByDescending(p => p.Year).ThenByDescending(p => p.Month);

            var totalCount = await q.CountAsync();

            var items = await q
                .Skip((Math.Max(1, request.PageNumber) - 1) * Math.Max(1, request.PageSize))
                .Take(Math.Max(1, request.PageSize))
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Payroll?> GetPayrollByIdAsync(Guid payrollId)
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);
        }

        public async Task<List<Payroll>> GetUnapprovedPayrollsAsync()
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.ApprovedByEmployee)
                .Include(p => p.PayrollComponents)
                .Where(p => p.PayrollStatus == PayrollStatusEnum.ApprovalPending)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Payroll>> GetPayrollsByMonthYearAsync(int month, int year, PayrollStatusEnum? status = null)
        {
            var q = _context.Payrolls
                .Where(p => p.Month == month && p.Year == year)
                .AsQueryable();

            if (status.HasValue)
                q = q.Where(p => p.PayrollStatus == status.Value);

            return await q.ToListAsync();
        }

        public async Task<List<TaxSlab>> GetTaxSlabsAsync()
        {
            return await _context.TaxSlabs
                .OrderBy(t => t.MinIncome)
                .ToListAsync();
        }

        public async Task<List<Leave>> GetUnpaidLeavesAsync(Guid? employeeId, int month, int year)
        {
            var q = _context.Leaves
                .Where(l => l.Status == LeaveStatusEnum.Approved
                    && l.LeaveTypeId == 4
                    && l.FromDate.Month == month
                    && l.FromDate.Year == year)
                .AsQueryable();

            if (employeeId.HasValue)
                q = q.Where(l => l.EmployeeId == employeeId.Value);

            return await q.ToListAsync();
        }

        public async Task AddPayrollAsync(Payroll payroll)
        {
            await _context.Payrolls.AddAsync(payroll);
        }

        public async Task AddPayrollComponentsAsync(List<PayrollComponent> components)
        {
            await _context.PayrollComponents.AddRangeAsync(components);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BulkApprovePayrollsAsync(int month, int year, Guid approvedBy)
        {
            await _context.Payrolls
                .Where(p => p.Month == month && p.Year == year && p.PayrollStatus == PayrollStatusEnum.ApprovalPending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.PayrollStatus, PayrollStatusEnum.Approved)
                    .SetProperty(p => p.ApprovedBy, approvedBy)
                    .SetProperty(p => p.UpdatedAt, DateTime.Now));
        }
    }
}
