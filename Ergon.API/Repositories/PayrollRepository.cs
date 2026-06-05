using Microsoft.EntityFrameworkCore;
using Ergon.Interfaces;
using Ergon.Contexts;
using Ergon.Models;

namespace Ergon.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        protected ErgonContext _context;

        public PayrollRepository(ErgonContext context)
        {
            _context = context;
        }

        public async Task BulkApprovePayrollsAsync(int month, int year, Guid approvedBy)
        {
            await _context.Payrolls
              .Where(p => p.Month == month && p.Year == year)
              .ExecuteUpdateAsync(setters => setters
                  .SetProperty(p => p.PayrollStatus, PayrollStatusEnum.Approved)
                  .SetProperty(p => p.ApprovedBy, approvedBy)
                  .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
        }
    }
}
