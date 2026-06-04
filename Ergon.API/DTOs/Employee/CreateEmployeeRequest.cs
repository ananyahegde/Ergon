using Ergon.Models;

namespace Ergon.DTOs.Employee
{
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }
        public IFormFile? Pfp { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public int? CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public DateOnly DateOfJoining { get; set; }
        public EmploymentTypeEnum EmploymentType { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public int DesignationId { get; set; }
        public int ShiftId { get; set; }
        public int SalaryStructureId { get; set; }
        public Guid? ReportsTo { get; set; }
    }
}
