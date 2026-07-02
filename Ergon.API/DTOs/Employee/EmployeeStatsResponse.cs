namespace Ergon.DTOs.Employee
{
    public class EmployeeStatsResponse
    {
        public int TotalEmployees { get; set; }
        public EmployeeStatusStats ByStatus { get; set; } = new();
        public EmployeeTypeStats ByType { get; set; } = new();
    }

    public class EmployeeStatusStats
    {
        public int Active { get; set; }
        public int OnNoticePeriod { get; set; }
        public int Resigned { get; set; }
        public int Terminated { get; set; }
        public int Suspended { get; set; }
    }

    public class EmployeeTypeStats
    {
        public int FullTime { get; set; }
        public int Intern { get; set; }
        public int PartTime { get; set; }
        public int Contract { get; set; }
    }
}
