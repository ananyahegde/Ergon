namespace Ergon.DTOs.Employee
{
    public class GetAllEmployeesRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortDirection { get; set; } = "asc";
        public string? Search { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<int>? DesignationIds { get; set; }
        public List<int>? BranchIds { get; set; }
        public List<string>? EmploymentStatuses { get; set; }
    }
}

