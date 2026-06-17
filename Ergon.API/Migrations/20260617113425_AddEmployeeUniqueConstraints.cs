using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Cities_CityId",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Employees",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "uq_employee_personalemail",
                table: "Employees",
                column: "PersonalEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_employee_phone",
                table: "Employees",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_employee_workemail",
                table: "Employees",
                column: "WorkEmail",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Cities_CityId",
                table: "Employees",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Cities_CityId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "uq_employee_personalemail",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "uq_employee_phone",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "uq_employee_workemail",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Cities_CityId",
                table: "Employees",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
