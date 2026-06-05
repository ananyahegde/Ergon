namespace Ergon.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
