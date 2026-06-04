namespace Ergon.DTOs.PublicHoliday
{
    public class PublicHolidayResponse
    {
        public int PublicHolidayId { get; set; }
        public string PublicHolidayName { get; set; } = string.Empty;
        public DateOnly PublicHolidayDate { get; set; }
    }
}
