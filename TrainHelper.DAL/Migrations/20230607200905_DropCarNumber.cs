using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DropCarNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Invoices_InvoiceId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Trains_InvoiceId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarNumber",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "CarNumber",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "TrainId",
                table: "Invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TrainId",
                table: "Invoices",
                column: "TrainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Trains_TrainId",
                table: "Invoices",
                column: "TrainId",
                principalTable: "Trains",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Trains_TrainId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TrainId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TrainId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Trains",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CarNumber",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_InvoiceId",
                table: "Trains",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarNumber",
                table: "Cars",
                column: "CarNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Invoices_InvoiceId",
                table: "Trains",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
