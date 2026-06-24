using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexEmployeeDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocuments_EmployeeId",
                table: "EmployeeDocuments");

            migrationBuilder.CreateIndex(
                name: "ix_employeedocument_employeeid_documenttype",
                table: "EmployeeDocuments",
                columns: new[] { "EmployeeId", "DocumentType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employeedocument_employeeid_documenttype",
                table: "EmployeeDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeId",
                table: "EmployeeDocuments",
                column: "EmployeeId");
        }
    }
}
