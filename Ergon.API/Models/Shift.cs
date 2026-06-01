// `Shift` Master Table.

namespace Ergon.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
