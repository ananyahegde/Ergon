using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsMasterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_States_StateName",
                table: "States",
                column: "StateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ShiftName",
                table: "Shifts",
                column: "ShiftName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructures_SalaryStructureName",
                table: "SalaryStructures",
                column: "SalaryStructureName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_LeaveTypeName",
                table: "LeaveTypes",
                column: "LeaveTypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_DesignationName",
                table: "Designations",
                column: "DesignationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentName",
                table: "Departments",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CountryName",
                table: "Countries",
                column: "CountryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CityName",
                table: "Cities",
                column: "CityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BranchName",
                table: "Branches",
                column: "BranchName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_States_StateName",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_ShiftName",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_SalaryStructures_SalaryStructureName",
                table: "SalaryStructures");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleName",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypes_LeaveTypeName",
                table: "LeaveTypes");

            migrationBuilder.DropIndex(
                name: "IX_Designations_DesignationName",
                table: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentName",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Countries_CountryName",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CityName",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Branches_BranchName",
                table: "Branches");
        }
    }
}
