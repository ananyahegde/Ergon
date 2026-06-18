using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class LeaveEntitlementDecimalDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDays",
                table: "LeaveEntitlementComponents",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 1,
                column: "TotalDays",
                value: 12m);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 2,
                column: "TotalDays",
                value: 6m);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 3,
                column: "TotalDays",
                value: 15m);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 4,
                column: "TotalDays",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 5,
                column: "TotalDays",
                value: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalDays",
                table: "LeaveEntitlementComponents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 1,
                column: "TotalDays",
                value: 12);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 2,
                column: "TotalDays",
                value: 6);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 3,
                column: "TotalDays",
                value: 15);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 4,
                column: "TotalDays",
                value: 0);

            migrationBuilder.UpdateData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 5,
                column: "TotalDays",
                value: 0);
        }
    }
}
