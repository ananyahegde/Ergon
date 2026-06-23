namespace Ergon.DTOs.Dashboard
{
    public class TeamAttendanceSummary
    {
        public int TotalTeamMembers { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public int OnLeaveToday { get; set; }
    }
}
