// `Role` Master Table.
// Fixed set of roles as far as this application goes - HR Admin, HR, Manager, Employee

namespace Ergon.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
