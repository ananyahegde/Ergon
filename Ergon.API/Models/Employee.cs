namespace Ergon.Models
{
    public enum GenderEnum
    {
        Male,
        Female,
        Other
    }

    public enum EmploymentTypeEnum
    {
        Intern,
        FullTime,
        PartTime,
        Contract
    }

    public enum EmploymentStatusEnum
    {
        Active,
        OnNoticePeriod,
        Resigned,
        Terminated,
        Suspended
    }


    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }
        public string Pfp { get; set; } = string.Empty; // path

        // permanent address
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public DateOnly DateOfJoining { get; set; }
        public EmploymentTypeEnum EmploymentType { get; set; }
        public EmploymentStatusEnum EmploymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public int DesignationId { get; set; }
        public int ShiftId { get; set; }
        public int SalaryStructureId { get; set; }
        public Guid? ReportsTo { get; set; }

        // navigation properties
        public Role Role { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
        public Designation Designation { get; set; } = null!;
        public Shift Shift { get; set; } = null!;
        public SalaryStructure SalaryStructure { get; set; } = null!;
        public Employee? Manager { get; set; }

        public ICollection<EmployeeDocument> EmployeeDocuments { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<Leave> Leaves { get; set; }
        public ICollection<ReviewCycleDetails> ReviewCycleDetails { get; set; }
        public ICollection<BankAccount> BankAccounts { get; set; }
        public ICollection<Payroll> Payrolls { get; set; }
        public ICollection<Payroll> ApprovedPayrolls { get; set; }
        public ICollection<Employee> Subordinates { get; set; }
    }
}
