using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCarIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_Cars_CarNumber",
                table: "Cars");
        }
    }
}
