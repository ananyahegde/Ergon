namespace Ergon.DTOs.Dashboard
{
    public class TodayAttendanceSnapshot
    {
        public int Present { get; set; }
        public int Absent { get; set; }
        public int NotClockedIn { get; set; }
        public int OnLeave { get; set; }
    }
}
