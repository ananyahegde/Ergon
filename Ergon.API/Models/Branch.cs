// `Branch` Master Table.

namespace Ergon.Models
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
