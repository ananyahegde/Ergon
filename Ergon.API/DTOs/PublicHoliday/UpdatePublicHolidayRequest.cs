namespace Ergon.DTOs.PublicHoliday
{
    public class UpdatePublicHolidayRequest
    {
        public string PublicHolidayName { get; set; } = string.Empty;
        public DateOnly PublicHolidayDate { get; set; }
    }
}
