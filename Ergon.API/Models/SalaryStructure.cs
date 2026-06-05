namespace Ergon.Models
{
    public class SalaryStructure
    {
        public int SalaryStructureId { get; set; }
        public string SalaryStructureName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
        public ICollection<SalaryComponent> SalaryComponents { get; set; }
    }
}
