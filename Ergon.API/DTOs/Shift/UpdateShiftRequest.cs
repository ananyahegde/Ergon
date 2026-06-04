namespace Ergon.DTOs.Shift
{
    public class UpdateShiftRequest
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
