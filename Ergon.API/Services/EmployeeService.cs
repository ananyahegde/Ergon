using AutoMapper;
using Ergon.DTOs.Employee;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Ergon.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Guid, Employee> _repository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public EmployeeService(
            IRepository<Guid, Employee> repository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            INotificationService notificationService)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        private static void ValidateDateOfBirth(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;

            if (age < 18)
                throw new BadRequestException("Employee must be at least 18 years old.");
            if (age > 80)
                throw new BadRequestException("Date of birth is not valid.");
        }

        private static void ValidateDateOfJoining(DateOnly dateOfJoining)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (dateOfJoining < today.AddYears(-80))
                throw new BadRequestException("Date of joining cannot be more than 80 years in the past.");
            if (dateOfJoining > today.AddMonths(6))
                throw new BadRequestException("Date of joining cannot be more than 6 months in the future.");
        }

        private async Task ValidateManagerAsync(Guid managerId, Guid? employeeId = null)
        {
            if (employeeId.HasValue && managerId == employeeId.Value)
                throw new BadRequestException("An employee cannot be their own manager.");

            var manager = await _employeeRepository.GetManagerAsync(managerId);
            if (manager == null)
                throw new NotFoundException("Specified manager does not exist.");

            var invalidStatuses = new[]
            {
                EmploymentStatusEnum.Resigned,
                EmploymentStatusEnum.Terminated,
                EmploymentStatusEnum.Suspended,
                EmploymentStatusEnum.OnNoticePeriod
            };

            if (invalidStatuses.Contains(manager.EmploymentStatus))
                throw new BadRequestException($"Cannot assign this employee as manager. Their current status is {manager.EmploymentStatus}.");
        }


        public async Task<PagedEmployeeResponse> GetAllEmployeesAsync(GetAllEmployeesRequest request)
        {
            request.PageSize = Math.Max(1, request.PageSize);
            request.PageNumber = Math.Max(1, request.PageNumber);

            var (employees, totalCount) = await _employeeRepository.GetAllAsync(request);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            if (totalCount > 0 && request.PageNumber > totalPages)
            {
                request.PageNumber = totalPages;
                (employees, _) = await _employeeRepository.GetAllAsync(request);
            }

            return new PagedEmployeeResponse
            {
                Items = _mapper.Map<List<EmployeeListResponse>>(employees),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<EmployeeDetailResponse> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException("Employee not found.");
            return _mapper.Map<EmployeeDetailResponse>(employee);
        }

        public async Task<IEnumerable<EmployeeListResponse>> GetMyTeamAsync(Guid managerId)
        {
            var employees = await _employeeRepository.GetTeamAsync(managerId);
            return _mapper.Map<List<EmployeeListResponse>>(employees);
        }

        public async Task<EmployeeDetailResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            ValidateDateOfBirth(request.DateOfBirth);
            ValidateDateOfJoining(request.DateOfJoining);

            if (!request.WorkEmail.EndsWith("@ergon.com", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Work email must be a valid @ergon.com address.");

            if (request.WorkEmail.Equals(request.PersonalEmail, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Work email and personal email must be different.");

            if (await _employeeRepository.ExistsByWorkEmailAsync(request.WorkEmail))
                throw new ConflictException("An employee with this work email already exists.");

            if (await _employeeRepository.ExistsByPersonalEmailAsync(request.PersonalEmail))
                throw new ConflictException("An employee with this personal email already exists.");

            if (await _employeeRepository.ExistsByPhoneAsync(request.Phone))
                throw new ConflictException("An employee with this phone number already exists.");

            if (request.ReportsTo.HasValue)
                await ValidateManagerAsync(request.ReportsTo.Value);

            var employee = _mapper.Map<Employee>(request);
            employee.EmployeeId = Guid.NewGuid();
            employee.EmploymentStatus = EmploymentStatusEnum.Active;
            employee.CreatedAt = DateTime.Now;
            employee.UpdatedAt = DateTime.Now;

            var tempPassword = Guid.NewGuid().ToString()[..8];
            employee.PasswordHash = PasswordHasher.HashPassword(tempPassword);

            var createdEmployee = await _repository.Create(employee);

            // await EmailHelper.SendTempPasswordAsync(employee.WorkEmail, tempPassword);

            var response = await GetEmployeeByIdAsync(createdEmployee.EmployeeId);
            response.TempPassword = tempPassword;
            return response;
        }

        public async Task<EmployeeDetailResponse> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request)
        {
            var employee = await _repository.Get(id);
            if (employee == null)
                throw new NotFoundException("Employee not found.");

            if (request.PersonalEmail.Equals(employee.WorkEmail, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Personal email must be different from the work email.");

            if (await _employeeRepository.ExistsByPersonalEmailAsync(request.PersonalEmail, excludeId: id))
                throw new ConflictException("An employee with this personal email already exists.");

            if (await _employeeRepository.ExistsByPhoneAsync(request.Phone, excludeId: id))
                throw new ConflictException("An employee with this phone number already exists.");

            if (request.ReportsTo.HasValue)
                await ValidateManagerAsync(request.ReportsTo.Value, employeeId: id);

            _mapper.Map(request, employee);
            employee.UpdatedAt = DateTime.Now;

            await _repository.Update(id, employee);
            return await GetEmployeeByIdAsync(id);
        }

        public async Task<EmployeeDetailResponse> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _repository.Get(id);
            if (employee == null)
                throw new NotFoundException("Employee not found.");
            await _repository.Delete(id);
            return _mapper.Map<EmployeeDetailResponse>(employee);
        }

        public async Task<EmployeeDetailResponse> UpdateEmployeeStatusAsync(Guid id, UpdateEmployeeStatusRequest request)
        {
            var employee = await _repository.Get(id);
            if (employee == null)
                throw new NotFoundException("Employee not found.");

            var inactiveStatuses = new[]
            {
                EmploymentStatusEnum.OnNoticePeriod,
                EmploymentStatusEnum.Resigned,
                EmploymentStatusEnum.Terminated,
                EmploymentStatusEnum.Suspended
            };

            if (inactiveStatuses.Contains(request.EmploymentStatus))
            {
                if (await _employeeRepository.HasDirectReportsAsync(id))
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
            if (employee == null)
                throw new NotFoundException("Employee not found.");

            var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedMimeTypes.Contains(pfp.ContentType.ToLower()))
                throw new BadRequestException("Profile picture must be JPG or PNG.");

            if (pfp.Length > 5 * 1024 * 1024)
                throw new BadRequestException("File size must not exceed 5MB.");

            var fileName = $"{id}_pfp.jpg";
            var filePath = Path.Combine("uploads", "pfp", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var image = await Image.LoadAsync(pfp.OpenReadStream());
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
