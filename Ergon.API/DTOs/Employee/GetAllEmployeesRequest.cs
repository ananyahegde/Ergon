using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Employee
{
    public class GetAllEmployeesRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1.")]
        public int PageSize { get; set; } = 10;

        public string? SortDirection { get; set; } = "asc";
        public string? Search { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<int>? DesignationIds { get; set; }
        public List<int>? BranchIds { get; set; }
        public List<string>? EmploymentStatuses { get; set; }
    }
}
