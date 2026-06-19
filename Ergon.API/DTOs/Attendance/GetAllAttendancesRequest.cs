using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.Attendance
{
    public class GetAllAttendancesRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1.")]
        public int PageSize { get; set; } = 10;

        public Guid? EmployeeId { get; set; }

        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int? Month { get; set; }

        [Range(1900, int.MaxValue, ErrorMessage = "Year must be 1900 or later.")]
        public int? Year { get; set; }

        public List<string>? Status { get; set; }
    }
}
