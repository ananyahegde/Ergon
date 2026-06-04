namespace Ergon.DTOs.Attendance
{
    public class AttendanceTodaySummaryResponse
    {
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalOnLeave { get; set; }
        public int TotalHalfDay { get; set; }
        public int TotalIncomplete { get; set; }
    }
}
