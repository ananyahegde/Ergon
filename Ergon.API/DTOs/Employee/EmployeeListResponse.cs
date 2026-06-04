namespace Ergon.DTOs.Employee
{
    public class EmployeeListResponse
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public string Pfp { get; set; } = string.Empty;
    }
}
