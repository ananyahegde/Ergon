using Ergon.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Contexts
{
    public class ErgonContext : DbContext
    {
        public ErgonContext(DbContextOptions<ErgonContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<SalaryStructure> SalaryStructures { get; set; }
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PayrollComponent> PayrollComponents { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<ReviewCycle> ReviewCycles { get; set; }
        public DbSet<ReviewCycleDetails> ReviewCycleDetails { get; set; }
        public DbSet<PublicHoliday> PublicHolidays { get; set; }
        public DbSet<TaxSlab> TaxSlabs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee
            modelBuilder.Entity<Employee>(e =>
            {
                // store enum types as strings instead of int
                e.Property(e => e.Gender).HasConversion<string>();
                e.Property(e => e.EmploymentType).HasConversion<string>();
                e.Property(e => e.EmploymentStatus).HasConversion<string>();

                e.Property(e => e.DateOfBirth).HasColumnType("date");
                e.Property(e => e.DateOfJoining).HasColumnType("date");
                e.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                e.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                e.HasOne(e => e.Manager)
                 .WithMany(e => e.Subordinates)
                 .HasForeignKey(e => e.ReportsTo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Role)
                 .WithMany(r => r.Employees)
                 .HasForeignKey(e => e.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Department)
                 .WithMany(d => d.Employees)
                 .HasForeignKey(e => e.DepartmentId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Branch)
                 .WithMany(b => b.Employees)
                 .HasForeignKey(e => e.BranchId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Designation)
                 .WithMany(d => d.Employees)
                 .HasForeignKey(e => e.DesignationId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Shift)
                 .WithMany(s => s.Employees)
                 .HasForeignKey(e => e.ShiftId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.SalaryStructure)
                 .WithMany(s => s.Employees)
                 .HasForeignKey(e => e.SalaryStructureId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance
            modelBuilder.Entity<Attendance>(a =>
            {
                a.Property(a => a.AttendanceStatus).HasConversion<string>();

                a.Property(a => a.Date).HasColumnType("date");
                a.Property(a => a.ClockInTime).HasColumnType("time");
                a.Property(a => a.ClockOutTime).HasColumnType("time");
                a.Property(a => a.CreatedAt).HasColumnType("timestamp without time zone");
                a.Property(a => a.UpdatedAt).HasColumnType("timestamp without time zone");

                a.HasOne(a => a.Employee)
                 .WithMany(e => e.Attendances)
                 .HasForeignKey(a => a.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // BankAccount
            modelBuilder.Entity<BankAccount>(b =>
            {
                b.Property(b => b.CreatedAt).HasColumnType("timestamp without time zone");
                b.Property(b => b.UpdatedAt).HasColumnType("timestamp without time zone");

                b.HasOne(b => b.Employee)
                .WithMany(e => e.BankAccounts)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // EmployeeDocument
            modelBuilder.Entity<EmployeeDocument>(d =>
            {
                d.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                d.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                d.Property(d => d.DocumentType).HasConversion<string>();

                d.HasOne(d => d.Employee)
                 .WithMany(e => e.EmployeeDocuments)
                 .HasForeignKey(d => d.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Leave
            modelBuilder.Entity<Leave>(l =>
            {
                l.Property(l => l.Status).HasConversion<string>();
                l.Property(e => e.FromDate).HasColumnType("date");
                l.Property(e => e.ToDate).HasColumnType("date");
                l.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                l.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                l.HasOne(l => l.Employee)
                 .WithMany(e => e.Leaves)
                 .HasForeignKey(l => l.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                l.HasOne(l => l.LeaveType)
                 .WithMany(le => le.Leaves)
                 .HasForeignKey(l => l.LeaveTypeId)
                 .OnDelete(DeleteBehavior.Restrict);

                l.HasOne(l => l.ActionedByEmployee)
                 .WithMany(e => e.Leaves)
                 .HasForeignKey(l => l.ActionedBy)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Payroll
            modelBuilder.Entity<Payroll>(p =>
            {
                p.Property(p => p.PayrollStatus).HasConversion<string>();
                p.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                p.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                p.HasOne(p => p.Employee)
                 .WithMany(e => e.Payrolls)
                 .HasForeignKey(p => p.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                p.HasOne(p => p.ApprovedByEmployee)
                 .WithMany(e => e.ApprovedPayrolls)
                 .HasForeignKey(p => p.ApprovedBy)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // PayrollComponent
            modelBuilder.Entity<PayrollComponent>(p =>
            {
                p.Property(p => p.PayrollComponentType).HasConversion<string>();
                p.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                p.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                p.HasOne(p => p.Payroll)
                 .WithMany(pa => pa.PayrollComponents)
                 .HasForeignKey(p => p.PayrollId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // SalaryComponent
            modelBuilder.Entity<SalaryComponent>(s =>
            {
                s.Property(s => s.ComponentType).HasConversion<string>();
                s.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                s.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                s.HasOne(s => s.SalaryStructure)
                 .WithMany(ss => ss.SalaryComponents)
                 .HasForeignKey(s => s.SalaryStructureId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ReviewCycle
            modelBuilder.Entity<ReviewCycle>(r =>
            {
                r.Property(r => r.ReviewCycleStatus).HasConversion<string>();

                r.Property(a => a.StartDate).HasColumnType("date");
                r.Property(a => a.EndDate).HasColumnType("time");
                r.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                r.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");
            });

            // ReviewCycleDetails
            modelBuilder.Entity<ReviewCycleDetails>(r =>
            {
                r.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                r.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                r.HasOne(r => r.Employee)
                     .WithMany(e => e.ReviewCycleDetails)
                     .HasForeignKey(r => r.EmployeeId)
                     .OnDelete(DeleteBehavior.Restrict);

                r.HasOne(r => r.ReviewCycle)
                 .WithMany(rc => rc.ReviewCycleDetails)
                 .HasForeignKey(r => r.ReviewCycleId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
