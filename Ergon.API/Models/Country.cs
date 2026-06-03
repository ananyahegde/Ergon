namespace Ergon.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
