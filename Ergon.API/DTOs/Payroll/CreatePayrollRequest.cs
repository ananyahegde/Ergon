namespace Ergon.DTOs.Payroll
{
    public class CreatePayrollRequest
    {
        public Guid EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
