// `Designation` Master Table.

namespace Ergon.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
