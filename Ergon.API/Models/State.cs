namespace Ergon.Models
{
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
