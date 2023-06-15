using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trains_Number",
                table: "Trains",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationName",
                table: "Stations",
                column: "StationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNum",
                table: "Invoices",
                column: "InvoiceNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarNumber",
                table: "Cars",
                column: "CarNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trains_Number",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Stations_StationName",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNum",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarNumber",
                table: "Cars");
        }
    }
}
