using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.Employee;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Ergon.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Guid, Employee> _repository;
        private readonly ErgonContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public EmployeeService(
                IRepository<Guid, Employee> repository,
                ErgonContext context,
                IMapper mapper,
                INotificationService notificationService)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }


        public async Task<PagedEmployeeResponse> GetAllEmployeesAsync(GetAllEmployeesRequest request)
        {
            var q = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .AsQueryable();

            // filters
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                q = q.Where(e => (e.FirstName + " " + e.LastName).Contains(request.Search));
            }

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

            // sorting
            var asc = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
            q = asc
                ? q.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                : q.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName);

            // pagination
            var totalCount = await q.CountAsync();
            var pageSize = Math.Max(1, request.PageSize);
            var pageNumber = Math.Max(1, request.PageNumber);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var employees = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedEmployeeResponse
            {
                Items = _mapper.Map<List<EmployeeListResponse>>(employees),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


        public async Task<EmployeeDetailResponse> GetEmployeeByIdAsync(Guid id)
        {
            _context.ChangeTracker.Clear();

            var employee = await _context.Employees
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
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                throw new NotFoundException("Employee not found.");

            return _mapper.Map<EmployeeDetailResponse>(employee);
        }


        public async Task<IEnumerable<EmployeeListResponse>> GetMyTeamAsync(Guid managerId)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Where(e => e.ReportsTo == managerId)
                .ToListAsync();
            return _mapper.Map<List<EmployeeListResponse>>(employees);
        }


        public async Task<EmployeeDetailResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var employee = _mapper.Map<Employee>(request);
            employee.EmployeeId = Guid.NewGuid();
            employee.EmploymentStatus = EmploymentStatusEnum.Active;
            employee.CreatedAt = DateTime.Now;
            employee.UpdatedAt = DateTime.Now;

            var tempPassword = Guid.NewGuid().ToString()[..8];
            employee.PasswordHash = PasswordHasher.HashPassword(tempPassword);

            var createdEmployee = await _repository.Create(employee);

            // send the password to email - not implemented yet
            // await EmailHelper.SendTempPasswordAsync(employee.WorkEmail, tempPassword);

            var response = await GetEmployeeByIdAsync(createdEmployee.EmployeeId);
            response.TempPassword = tempPassword;
            return response;
        }


        public async Task<EmployeeDetailResponse> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request)
        {
            var employee = await _repository.Get(id);
            if (employee == null) throw new NotFoundException("Employee not found.");
            _mapper.Map(request, employee);
            employee.UpdatedAt = DateTime.Now;
            await _repository.Update(id, employee);
            return await GetEmployeeByIdAsync(id);
        }


        public async Task<EmployeeDetailResponse> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _repository.Get(id);
            if (employee == null) throw new NotFoundException("Employee not found.");
            await _repository.Delete(id);
            return _mapper.Map<EmployeeDetailResponse>(employee);
        }


        public async Task<EmployeeDetailResponse> UpdateEmployeeStatusAsync(Guid id, UpdateEmployeeStatusRequest request)
        {
            var employee = await _repository.Get(id);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var inactiveStatuses = new[]
            {
                EmploymentStatusEnum.OnNoticePeriod,
                EmploymentStatusEnum.Resigned,
                EmploymentStatusEnum.Terminated,
                EmploymentStatusEnum.Suspended
            };

            if (inactiveStatuses.Contains(request.EmploymentStatus))
            {
                var hasDirectReports = await _context.Employees
                    .AnyAsync(e => e.ReportsTo == id);

                if (hasDirectReports)
                    throw new BadRequestException("Cannot update status: this employee has direct reports. Reassign them first.");
            }

            employee.EmploymentStatus = request.EmploymentStatus;
            employee.UpdatedAt = DateTime.Now;

            await _repository.Update(id, employee);
            await _notificationService.CreateNotificationAsync(id, "Employment Status Update", $"Your employment status has been updated to {request.EmploymentStatus}.");

            return await GetEmployeeByIdAsync(id);
        }


        public async Task<EmployeeDetailResponse> UpdateEmployeePfpAsync(Guid id, IFormFile pfp)
        {
            var employee = await _repository.Get(id);
            if (employee == null) throw new NotFoundException("Employee not found.");

            var fileName = $"{id}_pfp.jpg";
            var filePath = Path.Combine("uploads", "pfp", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var image = await Image.LoadAsync(pfp.OpenReadStream());

            var size = Math.Min(image.Width, image.Height);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(512, 512),
                Mode = ResizeMode.Max
            }));

            await image.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 80 });

            employee.Pfp = filePath;
            employee.UpdatedAt = DateTime.Now;
            await _repository.Update(id, employee);
            return await GetEmployeeByIdAsync(id);
        }
    }
}
