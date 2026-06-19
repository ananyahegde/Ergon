using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Leave
{
    public class GetAllLeavesRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1.")]
        public int PageSize { get; set; } = 10;

        public Guid? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }

        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int? Month { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int? Year { get; set; }

        public List<string>? Statuses { get; set; }
    }
}
