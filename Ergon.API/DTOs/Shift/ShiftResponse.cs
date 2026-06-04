namespace Ergon.DTOs.Shift
{
    public class ShiftResponse
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
