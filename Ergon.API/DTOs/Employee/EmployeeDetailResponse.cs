namespace Ergon.DTOs.Employee
{
    public class EmployeeDetailResponse
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string? TempPassword { get; set; }
        public string PersonalEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Pfp { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public DateOnly DateOfJoining { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string SalaryStructureName { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
