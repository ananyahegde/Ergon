namespace Ergon.DTOs.PublicHoliday
{
    public class CreatePublicHolidayRequest
    {
        public string PublicHolidayName { get; set; } = string.Empty;
        public DateOnly PublicHolidayDate { get; set; }
    }
}
