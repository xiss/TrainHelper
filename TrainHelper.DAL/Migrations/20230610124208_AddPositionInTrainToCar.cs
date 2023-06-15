using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionInTrainToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionInTrain",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionInTrain",
                table: "Cars");
        }
    }
}
