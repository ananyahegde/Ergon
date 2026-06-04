namespace Ergon.DTOs.Shift
{
    public class CreateShiftRequest
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
