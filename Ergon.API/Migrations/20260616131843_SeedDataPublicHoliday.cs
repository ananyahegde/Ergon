using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataPublicHoliday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PublicHolidays",
                columns: new[] { "PublicHolidayId", "PublicHolidayDate", "PublicHolidayName" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 1, 1), "New Year's Day" },
                    { 2, new DateOnly(2026, 1, 26), "Republic Day" },
                    { 3, new DateOnly(2026, 8, 15), "Independence Day" },
                    { 5, new DateOnly(2026, 12, 25), "Christmas Day" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PublicHolidays",
                keyColumn: "PublicHolidayId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PublicHolidays",
                keyColumn: "PublicHolidayId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PublicHolidays",
                keyColumn: "PublicHolidayId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PublicHolidays",
                keyColumn: "PublicHolidayId",
                keyValue: 5);
        }
    }
}
