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
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<LeaveEntitlement> LeaveEntitlements { get; set; }
        public DbSet<LeaveEntitlementComponent> LeaveEntitlementComponents { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId).HasName("pk_role");
            modelBuilder.Entity<Department>().HasKey(d => d.DepartmentId).HasName("pk_department");
            modelBuilder.Entity<Branch>().HasKey(b => b.BranchId).HasName("pk_branch");
            modelBuilder.Entity<Designation>().HasKey(d => d.DesignationId).HasName("pk_designation");
            modelBuilder.Entity<Shift>().HasKey(s => s.ShiftId).HasName("pk_shift");
            modelBuilder.Entity<LeaveType>().HasKey(lt => lt.LeaveTypeId).HasName("pk_leavetype");
            modelBuilder.Entity<SalaryStructure>().HasKey(ss => ss.SalaryStructureId).HasName("pk_salarystructure");
            modelBuilder.Entity<PublicHoliday>().HasKey(ph => ph.PublicHolidayId).HasName("pk_publicholiday");
            modelBuilder.Entity<TaxSlab>().HasKey(ts => ts.TaxSlabId).HasName("pk_taxslab");
            modelBuilder.Entity<Country>().HasKey(c => c.CountryId).HasName("pk_country");
            modelBuilder.Entity<State>().HasKey(s => s.StateId).HasName("pk_state");
            modelBuilder.Entity<City>().HasKey(c => c.CityId).HasName("pk_city");


            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName).IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.DepartmentName).IsUnique();

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.BranchName).IsUnique();

            modelBuilder.Entity<Designation>()
                .HasIndex(d => d.DesignationName).IsUnique();

            modelBuilder.Entity<Shift>()
                .HasIndex(s => s.ShiftName).IsUnique();

            modelBuilder.Entity<LeaveType>()
                .HasIndex(lt => lt.LeaveTypeName).IsUnique();

            modelBuilder.Entity<SalaryStructure>()
                .HasIndex(ss => ss.SalaryStructureName).IsUnique();

            modelBuilder.Entity<Country>()
                .HasIndex(c => c.CountryName).IsUnique();

            modelBuilder.Entity<State>()
                .HasIndex(s => s.StateName).IsUnique();

            modelBuilder.Entity<City>()
                .HasIndex(c => c.CityName).IsUnique();


            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "HR Admin" },
                new Role { RoleId = 2, RoleName = "HR" },
                new Role { RoleId = 3, RoleName = "Manager" },
                new Role { RoleId = 4, RoleName = "Employee" }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "Engineering" },
                new Department { DepartmentId = 2, DepartmentName = "Human Resources" },
                new Department { DepartmentId = 3, DepartmentName = "Finance" },
                new Department { DepartmentId = 4, DepartmentName = "Sales" },
                new Department { DepartmentId = 5, DepartmentName = "Operations" }
            );

            modelBuilder.Entity<Branch>().HasData(
                new Branch { BranchId = 1, BranchName = "Bangalore" },
                new Branch { BranchId = 2, BranchName = "Chennai" },
                new Branch { BranchId = 3, BranchName = "Mumbai" }
            );

            modelBuilder.Entity<Designation>().HasData(
                new Designation { DesignationId = 1, DesignationName = "Software Engineer" },
                new Designation { DesignationId = 2, DesignationName = "Senior Software Engineer" },
                new Designation { DesignationId = 3, DesignationName = "HR Executive" },
                new Designation { DesignationId = 4, DesignationName = "HR Manager" },
                new Designation { DesignationId = 5, DesignationName = "Finance Executive" },
                new Designation { DesignationId = 6, DesignationName = "Sales Executive" },
                new Designation { DesignationId = 7, DesignationName = "Operations Manager" }
            );

            modelBuilder.Entity<Shift>().HasData(
                new Shift { ShiftId = 1, ShiftName = "Morning", StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(18, 0) },
                new Shift { ShiftId = 2, ShiftName = "Evening", StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(23, 0) }
            );

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Casual Leave" },
                new LeaveType { LeaveTypeId = 2, LeaveTypeName = "Sick Leave" },
                new LeaveType { LeaveTypeId = 3, LeaveTypeName = "Privilege Leave" },
                new LeaveType { LeaveTypeId = 4, LeaveTypeName = "Leave Without Pay" },
                new LeaveType { LeaveTypeId = 5, LeaveTypeName = "Compensatory Off" }
            );

            modelBuilder.Entity<SalaryStructure>().HasData(
                new SalaryStructure { SalaryStructureId = 1, SalaryStructureName = "Structure A" },
                new SalaryStructure { SalaryStructureId = 2, SalaryStructureName = "Structure B" },
                new SalaryStructure { SalaryStructureId = 3, SalaryStructureName = "Structure C" }
            );

            modelBuilder.Entity<Country>().HasData(
                new Country { CountryId = 1, CountryName = "India" }
            );

            modelBuilder.Entity<State>().HasData(
                new State { StateId = 1, StateName = "Karnataka" },
                new State { StateId = 2, StateName = "Tamil Nadu" },
                new State { StateId = 3, StateName = "Maharashtra" }
            );

            modelBuilder.Entity<City>().HasData(
                new City { CityId = 1, CityName = "Bangalore" },
                new City { CityId = 2, CityName = "Chennai" },
                new City { CityId = 3, CityName = "Mumbai" }
            );

            modelBuilder.Entity<LeaveEntitlement>().HasData(
                new LeaveEntitlement { LeaveEntitlementId = 1, LeaveEntitlementName = "Standard Entitlement" }
            );

            modelBuilder.Entity<LeaveEntitlementComponent>().HasData(
                new LeaveEntitlementComponent { LeaveEntitlementComponentId = 1, LeaveEntitlementId = 1, LeaveTypeId = 1, TotalDays = 12 },
                new LeaveEntitlementComponent { LeaveEntitlementComponentId = 2, LeaveEntitlementId = 1, LeaveTypeId = 2, TotalDays = 6 },
                new LeaveEntitlementComponent { LeaveEntitlementComponentId = 3, LeaveEntitlementId = 1, LeaveTypeId = 3, TotalDays = 15 },
                new LeaveEntitlementComponent { LeaveEntitlementComponentId = 4, LeaveEntitlementId = 1, LeaveTypeId = 4, TotalDays = 0 },
                new LeaveEntitlementComponent { LeaveEntitlementComponentId = 5, LeaveEntitlementId = 1, LeaveTypeId = 5, TotalDays = 0 }
            );

            modelBuilder.Entity<PublicHoliday>().HasData(
                new PublicHoliday { PublicHolidayId = 1, PublicHolidayName = "New Year's Day", PublicHolidayDate = new DateOnly(2026, 1, 1) },
                new PublicHoliday { PublicHolidayId = 2, PublicHolidayName = "Republic Day", PublicHolidayDate = new DateOnly(2026, 1, 26) },
                new PublicHoliday { PublicHolidayId = 3, PublicHolidayName = "Independence Day", PublicHolidayDate = new DateOnly(2026, 8, 15) },
                new PublicHoliday { PublicHolidayId = 5, PublicHolidayName = "Christmas Day", PublicHolidayDate = new DateOnly(2026, 12, 25) }
            );


            // Employee
            modelBuilder.Entity<Employee>(emp =>
            {
                emp.HasKey(emp => emp.EmployeeId).HasName("pk_employee");

                emp.Property(emp => emp.Gender).HasConversion<string>();
                emp.Property(emp => emp.EmploymentType).HasConversion<string>();
                emp.Property(emp => emp.EmploymentStatus).HasConversion<string>();

                emp.Property(emp => emp.DateOfBirth).HasColumnType("date");
                emp.Property(emp => emp.DateOfJoining).HasColumnType("date");
                emp.Property(emp => emp.CreatedAt).HasColumnType("timestamp without time zone");
                emp.Property(emp => emp.UpdatedAt).HasColumnType("timestamp without time zone");

                emp.HasOne(emp => emp.Manager)
                   .WithMany(emp => emp.Subordinates)
                   .HasForeignKey(emp => emp.ReportsTo)
                   .HasConstraintName("fk_employee_employee")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.Role)
                   .WithMany(r => r.Employees)
                   .HasForeignKey(emp => emp.RoleId)
                   .HasConstraintName("fk_employee_role")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.Department)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(emp => emp.DepartmentId)
                   .HasConstraintName("fk_employee_department")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.Branch)
                   .WithMany(b => b.Employees)
                   .HasForeignKey(emp => emp.BranchId)
                   .HasConstraintName("fk_employee_branch")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.Designation)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(emp => emp.DesignationId)
                   .HasConstraintName("fk_employee_designation")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.Shift)
                   .WithMany(s => s.Employees)
                   .HasForeignKey(emp => emp.ShiftId)
                   .HasConstraintName("fk_employee_shift")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasOne(emp => emp.SalaryStructure)
                   .WithMany(s => s.Employees)
                   .HasForeignKey(emp => emp.SalaryStructureId)
                   .HasConstraintName("fk_employee_salarystructure")
                   .OnDelete(DeleteBehavior.Restrict);

                emp.HasIndex(e => e.WorkEmail).IsUnique().HasDatabaseName("uq_employee_workemail");
                emp.HasIndex(e => e.PersonalEmail).IsUnique().HasDatabaseName("uq_employee_personalemail");
                emp.HasIndex(e => e.Phone).IsUnique().HasDatabaseName("uq_employee_phone");
            });

            // Attendance
            modelBuilder.Entity<Attendance>(att =>
            {
                att.HasKey(att => att.AttendanceId).HasName("pk_attendance");

                att.Property(att => att.AttendanceStatus).HasConversion<string>();

                att.Property(att => att.Date).HasColumnType("date");
                att.Property(att => att.ClockInTime).HasColumnType("time");
                att.Property(att => att.ClockOutTime).HasColumnType("time");
                att.Property(att => att.CreatedAt).HasColumnType("timestamp without time zone");
                att.Property(att => att.UpdatedAt).HasColumnType("timestamp without time zone");

                att.HasOne(att => att.Employee)
                   .WithMany(emp => emp.Attendances)
                   .HasForeignKey(att => att.EmployeeId)
                   .HasConstraintName("fk_attendance_employee")
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // BankAccount
            modelBuilder.Entity<BankAccount>(ba =>
            {
                ba.HasKey(ba => ba.BankAccountId).HasName("pk_bankaccount");

                ba.Property(ba => ba.CreatedAt).HasColumnType("timestamp without time zone");
                ba.Property(ba => ba.UpdatedAt).HasColumnType("timestamp without time zone");

                ba.HasOne(ba => ba.Employee)
                  .WithMany(emp => emp.BankAccounts)
                  .HasForeignKey(ba => ba.EmployeeId)
                  .HasConstraintName("fk_bankaccount_employee")
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // EmployeeDocument
            modelBuilder.Entity<EmployeeDocument>(doc =>
            {
                doc.HasKey(doc => doc.DocumentId).HasName("pk_employeedocument");

                doc.Property(doc => doc.DocumentType).HasConversion<string>();
                doc.Property(doc => doc.CreatedAt).HasColumnType("timestamp without time zone");
                doc.Property(doc => doc.UpdatedAt).HasColumnType("timestamp without time zone");

                doc.HasOne(doc => doc.Employee)
                   .WithMany(emp => emp.EmployeeDocuments)
                   .HasForeignKey(doc => doc.EmployeeId)
                   .HasConstraintName("fk_employeedocument_employee")
                   .OnDelete(DeleteBehavior.Restrict);

                doc.HasIndex(doc => new { doc.EmployeeId, doc.DocumentType }).IsUnique().HasDatabaseName("ix_employeedocument_employeeid_documenttype");
            });

            // Leave
            modelBuilder.Entity<Leave>(lev =>
            {
                lev.HasKey(lev => lev.LeaveId).HasName("pk_leave");

                lev.Property(lev => lev.Status).HasConversion<string>();
                lev.Property(lev => lev.FromDate).HasColumnType("date");
                lev.Property(lev => lev.ToDate).HasColumnType("date");
                lev.Property(lev => lev.CreatedAt).HasColumnType("timestamp without time zone");
                lev.Property(lev => lev.UpdatedAt).HasColumnType("timestamp without time zone");

                lev.HasOne(lev => lev.Employee)
                   .WithMany(emp => emp.Leaves)
                   .HasForeignKey(lev => lev.EmployeeId)
                   .HasConstraintName("fk_leave_employee")
                   .OnDelete(DeleteBehavior.Restrict);

                lev.HasOne(lev => lev.LeaveType)
                   .WithMany(lt => lt.Leaves)
                   .HasForeignKey(lev => lev.LeaveTypeId)
                   .HasConstraintName("fk_leave_leavetype")
                   .OnDelete(DeleteBehavior.Restrict);

                lev.HasOne(lev => lev.ActionedByEmployee)
                   .WithMany(emp => emp.ActionedLeaves)
                   .HasForeignKey(lev => lev.ActionedBy)
                   .HasConstraintName("fk_leave_actionedby_employee")
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // Payroll
            modelBuilder.Entity<Payroll>(pay =>
            {
                pay.HasKey(pay => pay.PayrollId).HasName("pk_payroll");

                pay.Property(pay => pay.PayrollStatus).HasConversion<string>();
                pay.Property(pay => pay.CreatedAt).HasColumnType("timestamp without time zone");
                pay.Property(pay => pay.UpdatedAt).HasColumnType("timestamp without time zone");

                pay.HasOne(pay => pay.Employee)
                   .WithMany(emp => emp.Payrolls)
                   .HasForeignKey(pay => pay.EmployeeId)
                   .HasConstraintName("fk_payroll_employee")
                   .OnDelete(DeleteBehavior.Restrict);

                pay.HasOne(pay => pay.ApprovedByEmployee)
                   .WithMany(emp => emp.ApprovedPayrolls)
                   .HasForeignKey(pay => pay.ApprovedBy)
                   .HasConstraintName("fk_payroll_approvedby_employee")
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // PayrollComponent
            modelBuilder.Entity<PayrollComponent>(pc =>
            {
                pc.HasKey(pc => pc.PayrollComponentId).HasName("pk_payrollcomponent");

                pc.Property(pc => pc.PayrollComponentType).HasConversion<string>();
                pc.Property(pc => pc.CreatedAt).HasColumnType("timestamp without time zone");
                pc.Property(pc => pc.UpdatedAt).HasColumnType("timestamp without time zone");

                pc.HasOne(pc => pc.Payroll)
                  .WithMany(pay => pay.PayrollComponents)
                  .HasForeignKey(pc => pc.PayrollId)
                  .HasConstraintName("fk_payrollcomponent_payroll")
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // SalaryComponent
            modelBuilder.Entity<SalaryComponent>(sc =>
            {
                sc.HasKey(sc => sc.SalaryComponentId).HasName("pk_salarycomponent");

                sc.Property(sc => sc.ComponentType).HasConversion<string>();
                sc.Property(sc => sc.CreatedAt).HasColumnType("timestamp without time zone");
                sc.Property(sc => sc.UpdatedAt).HasColumnType("timestamp without time zone");

                sc.HasOne(sc => sc.SalaryStructure)
                  .WithMany(ss => ss.SalaryComponents)
                  .HasForeignKey(sc => sc.SalaryStructureId)
                  .HasConstraintName("fk_salarycomponent_salarystructure")
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // ReviewCycle
            modelBuilder.Entity<ReviewCycle>(rc =>
            {
                rc.HasKey(rc => rc.ReviewCycleId).HasName("pk_reviewcycle");

                rc.Property(rc => rc.ReviewCycleStatus).HasConversion<string>();
                rc.Property(rc => rc.StartDate).HasColumnType("date");
                rc.Property(rc => rc.EndDate).HasColumnType("date");
                rc.Property(rc => rc.CreatedAt).HasColumnType("timestamp without time zone");
                rc.Property(rc => rc.UpdatedAt).HasColumnType("timestamp without time zone");
            });

            // ReviewCycleDetails
            modelBuilder.Entity<ReviewCycleDetails>(rcd =>
            {
                rcd.HasKey(rcd => rcd.ReviewCycleDetailsId).HasName("pk_reviewcycledetails");

                rcd.Property(rcd => rcd.CreatedAt).HasColumnType("timestamp without time zone");
                rcd.Property(rcd => rcd.UpdatedAt).HasColumnType("timestamp without time zone");

                rcd.HasOne(rcd => rcd.Employee)
                   .WithMany(emp => emp.ReviewCycleDetails)
                   .HasForeignKey(rcd => rcd.EmployeeId)
                   .HasConstraintName("fk_reviewcycledetails_employee")
                   .OnDelete(DeleteBehavior.Restrict);

                rcd.HasOne(rcd => rcd.ReviewCycle)
                   .WithMany(rc => rc.ReviewCycleDetails)
                   .HasForeignKey(rcd => rcd.ReviewCycleId)
                   .HasConstraintName("fk_reviewcycledetails_reviewcycle")
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification
            modelBuilder.Entity<Notification>(n =>
            {
                n.HasKey(n => n.NotificationId).HasName("pk_notification");

                n.Property(n => n.CreatedAt).HasColumnType("timestamp without time zone");

                n.HasOne(n => n.Employee)
                 .WithMany(e => e.Notifications)
                 .HasForeignKey(n => n.EmployeeId)
                 .HasConstraintName("fk_notification_employee")
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // LeaveEntitlement
            modelBuilder.Entity<LeaveEntitlement>(le =>
            {
                le.HasKey(le => le.LeaveEntitlementId).HasName("pk_leaveentitlement");
            });

            modelBuilder.Entity<LeaveEntitlementComponent>(lec =>
            {
                lec.HasKey(lec => lec.LeaveEntitlementComponentId).HasName("pk_leaveentitlementcomponent");

                lec.HasOne(lec => lec.LeaveEntitlement)
                   .WithMany(le => le.LeaveEntitlementComponents)
                   .HasForeignKey(lec => lec.LeaveEntitlementId)
                   .HasConstraintName("fk_leaveentitlementcomponent_leaveentitlement")
                   .OnDelete(DeleteBehavior.Restrict);

                lec.HasOne(lec => lec.LeaveType)
                   .WithMany()
                   .HasForeignKey(lec => lec.LeaveTypeId)
                   .HasConstraintName("fk_leaveentitlementcomponent_leavetype")
                   .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.LeaveEntitlement)
                .WithMany(le => le.Employees)
                .HasForeignKey(e => e.LeaveEntitlementId)
                .HasConstraintName("fk_employee_leaveentitlement")
                .OnDelete(DeleteBehavior.Restrict);

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(rt =>
            {
                rt.HasKey(rt => rt.RefreshTokenId).HasName("pk_refreshtoken");

                rt.Property(rt => rt.Expiry).HasColumnType("timestamp without time zone");
                rt.Property(rt => rt.CreatedAt).HasColumnType("timestamp without time zone");

                rt.HasOne(rt => rt.Employee)
                  .WithMany(e => e.RefreshTokens)
                  .HasForeignKey(rt => rt.EmployeeId)
                  .HasConstraintName("fk_refreshtoken_employee")
                  .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
