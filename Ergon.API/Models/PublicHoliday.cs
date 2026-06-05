namespace Ergon.Models
{
    public class PublicHoliday
    {
        public int PublicHolidayId { get; set; }
        public string PublicHolidayName { get; set; } = string.Empty;
        public DateOnly PublicHolidayDate { get; set; }
    }
}
