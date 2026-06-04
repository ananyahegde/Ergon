using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BranchName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch", x => x.BranchId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CityName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_city", x => x.CityId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_country", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    DesignationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DesignationName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_designation", x => x.DesignationId);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    LeaveTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeaveTypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leavetype", x => x.LeaveTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PublicHolidays",
                columns: table => new
                {
                    PublicHolidayId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicHolidayName = table.Column<string>(type: "text", nullable: false),
                    PublicHolidayDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publicholiday", x => x.PublicHolidayId);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCycles",
                columns: table => new
                {
                    ReviewCycleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewName = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReviewCycleStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviewcycle", x => x.ReviewCycleId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructures",
                columns: table => new
                {
                    SalaryStructureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalaryStructureName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_salarystructure", x => x.SalaryStructureId);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShiftName = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift", x => x.ShiftId);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StateName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_state", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "TaxSlabs",
                columns: table => new
                {
                    TaxSlabId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_taxslab", x => x.TaxSlabId);
                });

            migrationBuilder.CreateTable(
                name: "SalaryComponents",
                columns: table => new
                {
                    SalaryComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentName = table.Column<string>(type: "text", nullable: false),
                    ComponentType = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SalaryStructureId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_salarycomponent", x => x.SalaryComponentId);
                    table.ForeignKey(
                        name: "fk_salarycomponent_salarystructure",
                        column: x => x.SalaryStructureId,
                        principalTable: "SalaryStructures",
                        principalColumn: "SalaryStructureId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    WorkEmail = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PersonalEmail = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Pfp = table.Column<string>(type: "text", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    StateId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    DateOfJoining = table.Column<DateOnly>(type: "date", nullable: false),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    EmploymentStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    BranchId = table.Column<int>(type: "integer", nullable: false),
                    DesignationId = table.Column<int>(type: "integer", nullable: false),
                    ShiftId = table.Column<int>(type: "integer", nullable: false),
                    SalaryStructureId = table.Column<int>(type: "integer", nullable: false),
                    ReportsTo = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_branch",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_department",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_designation",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "DesignationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_employee",
                        column: x => x.ReportsTo,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_role",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_salarystructure",
                        column: x => x.SalaryStructureId,
                        principalTable: "SalaryStructures",
                        principalColumn: "SalaryStructureId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_shift",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ClockInTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ClockOutTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    AttendanceStatus = table.Column<string>(type: "text", nullable: false),
                    LateEntry = table.Column<bool>(type: "boolean", nullable: false),
                    LateExit = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attendance", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "fk_attendance_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    BankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    IfscCode = table.Column<string>(type: "text", nullable: false),
                    AccountHolderName = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bankaccount", x => x.BankAccountId);
                    table.ForeignKey(
                        name: "fk_bankaccount_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentName = table.Column<string>(type: "text", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employeedocument", x => x.DocumentId);
                    table.ForeignKey(
                        name: "fk_employeedocument_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leaves",
                columns: table => new
                {
                    LeaveId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActionedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leave", x => x.LeaveId);
                    table.ForeignKey(
                        name: "fk_leave_actionedby_employee",
                        column: x => x.ActionedBy,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_leave_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_leave_leavetype",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "LeaveTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "fk_notification_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    PayrollId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    PayrollStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payroll", x => x.PayrollId);
                    table.ForeignKey(
                        name: "fk_payroll_approvedby_employee",
                        column: x => x.ApprovedBy,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payroll_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCycleDetails",
                columns: table => new
                {
                    ReviewCycleDetailsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelfScore = table.Column<decimal>(type: "numeric", nullable: false),
                    FeedbackScore = table.Column<decimal>(type: "numeric", nullable: false),
                    ManagerComments = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewCycleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviewcycledetails", x => x.ReviewCycleDetailsId);
                    table.ForeignKey(
                        name: "fk_reviewcycledetails_employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviewcycledetails_reviewcycle",
                        column: x => x.ReviewCycleId,
                        principalTable: "ReviewCycles",
                        principalColumn: "ReviewCycleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollComponents",
                columns: table => new
                {
                    PayrollComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayrollComponentName = table.Column<string>(type: "text", nullable: false),
                    PayrollComponentType = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PayrollId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payrollcomponent", x => x.PayrollComponentId);
                    table.ForeignKey(
                        name: "fk_payrollcomponent_payroll",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "PayrollId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "BranchId", "BranchName" },
                values: new object[,]
                {
                    { 1, "Bangalore" },
                    { 2, "Chennai" },
                    { 3, "Mumbai" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "Engineering" },
                    { 2, "Human Resources" },
                    { 3, "Finance" },
                    { 4, "Sales" },
                    { 5, "Operations" }
                });

            migrationBuilder.InsertData(
                table: "Designations",
                columns: new[] { "DesignationId", "DesignationName" },
                values: new object[,]
                {
                    { 1, "Software Engineer" },
                    { 2, "Senior Software Engineer" },
                    { 3, "HR Executive" },
                    { 4, "HR Manager" },
                    { 5, "Finance Executive" },
                    { 6, "Sales Executive" },
                    { 7, "Operations Manager" }
                });

            migrationBuilder.InsertData(
                table: "LeaveTypes",
                columns: new[] { "LeaveTypeId", "LeaveTypeName" },
                values: new object[,]
                {
                    { 1, "Casual Leave" },
                    { 2, "Sick Leave" },
                    { 3, "Privilege Leave" },
                    { 4, "Leave Without Pay" },
                    { 5, "Compensatory Off" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "HR Admin" },
                    { 2, "HR" },
                    { 3, "Manager" },
                    { 4, "Employee" }
                });

            migrationBuilder.InsertData(
                table: "SalaryStructures",
                columns: new[] { "SalaryStructureId", "SalaryStructureName" },
                values: new object[,]
                {
                    { 1, "Structure A" },
                    { 2, "Structure B" },
                    { 3, "Structure C" }
                });

            migrationBuilder.InsertData(
                table: "Shifts",
                columns: new[] { "ShiftId", "EndTime", "ShiftName", "StartTime" },
                values: new object[,]
                {
                    { 1, new TimeOnly(18, 0, 0), "Morning", new TimeOnly(9, 0, 0) },
                    { 2, new TimeOnly(23, 0, 0), "Evening", new TimeOnly(14, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId",
                table: "Attendances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_EmployeeId",
                table: "BankAccounts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeId",
                table: "EmployeeDocuments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchId",
                table: "Employees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CityId",
                table: "Employees",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CountryId",
                table: "Employees",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ReportsTo",
                table: "Employees",
                column: "ReportsTo");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId",
                table: "Employees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SalaryStructureId",
                table: "Employees",
                column: "SalaryStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ShiftId",
                table: "Employees",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StateId",
                table: "Employees",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_ActionedBy",
                table: "Leaves",
                column: "ActionedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_EmployeeId",
                table: "Leaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_LeaveTypeId",
                table: "Leaves",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EmployeeId",
                table: "Notifications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComponents_PayrollId",
                table: "PayrollComponents",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_ApprovedBy",
                table: "Payrolls",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCycleDetails_EmployeeId",
                table: "ReviewCycleDetails",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCycleDetails_ReviewCycleId",
                table: "ReviewCycleDetails",
                column: "ReviewCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryComponents_SalaryStructureId",
                table: "SalaryComponents",
                column: "SalaryStructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "Leaves");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PayrollComponents");

            migrationBuilder.DropTable(
                name: "PublicHolidays");

            migrationBuilder.DropTable(
                name: "ReviewCycleDetails");

            migrationBuilder.DropTable(
                name: "SalaryComponents");

            migrationBuilder.DropTable(
                name: "TaxSlabs");

            migrationBuilder.DropTable(
                name: "LeaveTypes");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropTable(
                name: "ReviewCycles");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SalaryStructures");

            migrationBuilder.DropTable(
                name: "Shifts");
        }
    }
}
