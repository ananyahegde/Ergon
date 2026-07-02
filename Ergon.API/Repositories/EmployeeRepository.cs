using Ergon.Contexts;
using Ergon.DTOs.Employee;
using Ergon.Interfaces;
using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ErgonContext _context;

        public EmployeeRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(GetAllEmployeesRequest request)
        {
            var q = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                q = q.Where(e => (e.FirstName + " " + e.LastName).ToLower().Contains(request.Search.ToLower()));

            if (request.DepartmentIds != null && request.DepartmentIds.Any())
                q = q.Where(e => request.DepartmentIds.Contains(e.DepartmentId));

            if (request.DesignationIds != null && request.DesignationIds.Any())
                q = q.Where(e => request.DesignationIds.Contains(e.DesignationId));

            if (request.BranchIds != null && request.BranchIds.Any())
                q = q.Where(e => request.BranchIds.Contains(e.BranchId));

            if (request.EmploymentStatuses != null && request.EmploymentStatuses.Any())
            {
                var statuses = request.EmploymentStatuses
                    .Select(s => Enum.TryParse<EmploymentStatusEnum>(s, out var status) ? status : (EmploymentStatusEnum?)null)
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                q = q.Where(e => statuses.Contains(e.EmploymentStatus));
            }

            var asc = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
            q = asc
                ? q.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                : q.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName);

            var totalCount = await q.CountAsync();

            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);

            var employees = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (employees, totalCount);
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Department)
                .Include(e => e.Branch)
                .Include(e => e.Designation)
                .Include(e => e.Shift)
                .Include(e => e.SalaryStructure)
                .Include(e => e.Manager)
                .Include(e => e.City)
                .Include(e => e.State)
                .Include(e => e.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<IEnumerable<Employee>> GetTeamAsync(Guid managerId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Where(e => e.ReportsTo == managerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasDirectReportsAsync(Guid managerId)
        {
            return await _context.Employees.AnyAsync(e => e.ReportsTo == managerId);
        }

        public async Task<bool> ExistsByWorkEmailAsync(string workEmail, Guid? excludeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.WorkEmail == workEmail &&
                (excludeId == null || e.EmployeeId != excludeId));
        }

        public async Task<bool> ExistsByPersonalEmailAsync(string personalEmail, Guid? excludeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.PersonalEmail == personalEmail &&
                (excludeId == null || e.EmployeeId != excludeId));
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, Guid? excludeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.Phone == phone &&
                (excludeId == null || e.EmployeeId != excludeId));
        }

        public async Task<Employee?> GetManagerAsync(Guid managerId)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == managerId);
        }

        public async Task<EmployeeStatsResponse> GetEmployeeStatsAsync()
        {
            var totalEmployees = await _context.Employees.CountAsync();

            var statusCounts = await _context.Employees
                .GroupBy(e => e.EmploymentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var typeCounts = await _context.Employees
                .GroupBy(e => e.EmploymentType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            return new EmployeeStatsResponse
            {
                TotalEmployees = totalEmployees,
                ByStatus = new EmployeeStatusStats
                {
                    Active = statusCounts.FirstOrDefault(s => s.Status == EmploymentStatusEnum.Active)?.Count ?? 0,
                    OnNoticePeriod = statusCounts.FirstOrDefault(s => s.Status == EmploymentStatusEnum.OnNoticePeriod)?.Count ?? 0,
                    Resigned = statusCounts.FirstOrDefault(s => s.Status == EmploymentStatusEnum.Resigned)?.Count ?? 0,
                    Terminated = statusCounts.FirstOrDefault(s => s.Status == EmploymentStatusEnum.Terminated)?.Count ?? 0,
                    Suspended = statusCounts.FirstOrDefault(s => s.Status == EmploymentStatusEnum.Suspended)?.Count ?? 0
                },

                ByType = new EmployeeTypeStats
                {
                    FullTime = typeCounts.FirstOrDefault(t => t.Type == EmploymentTypeEnum.FullTime)?.Count ?? 0,
                    Intern = typeCounts.FirstOrDefault(t => t.Type == EmploymentTypeEnum.Intern)?.Count ?? 0,
                    PartTime = typeCounts.FirstOrDefault(t => t.Type == EmploymentTypeEnum.PartTime)?.Count ?? 0,
                    Contract = typeCounts.FirstOrDefault(t => t.Type == EmploymentTypeEnum.Contract)?.Count ?? 0
                }
            };
        }
    }
}
