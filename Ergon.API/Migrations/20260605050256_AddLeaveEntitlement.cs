using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveEntitlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaveEntitlementId",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LeaveEntitlements",
                columns: table => new
                {
                    LeaveEntitlementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeaveEntitlementName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leaveentitlement", x => x.LeaveEntitlementId);
                });

            migrationBuilder.CreateTable(
                name: "LeaveEntitlementComponents",
                columns: table => new
                {
                    LeaveEntitlementComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalDays = table.Column<int>(type: "integer", nullable: false),
                    LeaveEntitlementId = table.Column<int>(type: "integer", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leaveentitlementcomponent", x => x.LeaveEntitlementComponentId);
                    table.ForeignKey(
                        name: "fk_leaveentitlementcomponent_leaveentitlement",
                        column: x => x.LeaveEntitlementId,
                        principalTable: "LeaveEntitlements",
                        principalColumn: "LeaveEntitlementId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_leaveentitlementcomponent_leavetype",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "LeaveTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LeaveEntitlementId",
                table: "Employees",
                column: "LeaveEntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEntitlementComponents_LeaveEntitlementId",
                table: "LeaveEntitlementComponents",
                column: "LeaveEntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveEntitlementComponents_LeaveTypeId",
                table: "LeaveEntitlementComponents",
                column: "LeaveTypeId");

            migrationBuilder.AddForeignKey(
                name: "fk_employee_leaveentitlement",
                table: "Employees",
                column: "LeaveEntitlementId",
                principalTable: "LeaveEntitlements",
                principalColumn: "LeaveEntitlementId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_leaveentitlement",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "LeaveEntitlementComponents");

            migrationBuilder.DropTable(
                name: "LeaveEntitlements");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LeaveEntitlementId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LeaveEntitlementId",
                table: "Employees");
        }
    }
}
