using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "CityId", "CityName" },
                values: new object[,]
                {
                    { 1, "Bangalore" },
                    { 2, "Chennai" },
                    { 3, "Mumbai" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[] { 1, "India" });

            migrationBuilder.InsertData(
                table: "LeaveEntitlements",
                columns: new[] { "LeaveEntitlementId", "LeaveEntitlementName" },
                values: new object[] { 1, "Standard Entitlement" });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "StateId", "StateName" },
                values: new object[,]
                {
                    { 1, "Karnataka" },
                    { 2, "Tamil Nadu" },
                    { 3, "Maharashtra" }
                });

            migrationBuilder.InsertData(
                table: "LeaveEntitlementComponents",
                columns: new[] { "LeaveEntitlementComponentId", "LeaveEntitlementId", "LeaveTypeId", "TotalDays" },
                values: new object[,]
                {
                    { 1, 1, 1, 12 },
                    { 2, 1, 2, 6 },
                    { 3, 1, 3, 15 },
                    { 4, 1, 4, 0 },
                    { 5, 1, 5, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "CityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "CityId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "CityId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlementComponents",
                keyColumn: "LeaveEntitlementComponentId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "States",
                keyColumn: "StateId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "States",
                keyColumn: "StateId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "States",
                keyColumn: "StateId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveEntitlements",
                keyColumn: "LeaveEntitlementId",
                keyValue: 1);
        }
    }
}
