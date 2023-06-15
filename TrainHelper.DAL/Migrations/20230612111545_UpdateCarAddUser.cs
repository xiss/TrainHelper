using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarAddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Stations_LastStationId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_LastStationId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Trains_LastStationId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "LastStationId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "WhenLastOperation",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "FreightEtsngName",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "LastOperationName",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "WhenLastOperation",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "LastStationId",
                table: "Cars",
                newName: "FreightId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_LastStationId",
                table: "Cars",
                newName: "IX_Cars_FreightId");

            migrationBuilder.CreateTable(
                name: "CarOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Freights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FreightEtsngName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Freights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefreshTokenId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WayPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StationId = table.Column<int>(type: "integer", nullable: false),
                    OperationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WayPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WayPoints_CarOperations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "CarOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WayPoints_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WayPoints_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarOperations_OperationName",
                table: "CarOperations",
                column: "OperationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Freights_FreightEtsngName",
                table: "Freights",
                column: "FreightEtsngName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WayPoints_CarId_OperationDate_StationId",
                table: "WayPoints",
                columns: new[] { "CarId", "OperationDate", "StationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WayPoints_OperationId",
                table: "WayPoints",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_WayPoints_StationId",
                table: "WayPoints",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Freights_FreightId",
                table: "Cars",
                column: "FreightId",
                principalTable: "Freights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Freights_FreightId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "Freights");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WayPoints");

            migrationBuilder.DropTable(
                name: "CarOperations");

            migrationBuilder.RenameColumn(
                name: "FreightId",
                table: "Cars",
                newName: "LastStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_FreightId",
                table: "Cars",
                newName: "IX_Cars_LastStationId");

            migrationBuilder.AddColumn<int>(
                name: "LastStationId",
                table: "Trains",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "WhenLastOperation",
                table: "Trains",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "FreightEtsngName",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastOperationName",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "WhenLastOperation",
                table: "Cars",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_Trains_LastStationId",
                table: "Trains",
                column: "LastStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Stations_LastStationId",
                table: "Cars",
                column: "LastStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_LastStationId",
                table: "Trains",
                column: "LastStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
