using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarTrain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Trains_TrainId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_FromStationId",
                table: "Trains");

            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_ToStationId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Trains_FromStationId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Trains_ToStationId",
                table: "Trains");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TrainId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FromStationId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "LastOperationName",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "ToStationId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "TrainId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "FromStationId",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastOperationName",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LastStationId",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToStationId",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "WhenLastOperation",
                table: "Cars",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_FromStationId",
                table: "Cars",
                column: "FromStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_InvoiceId",
                table: "Cars",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_LastStationId",
                table: "Cars",
                column: "LastStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_ToStationId",
                table: "Cars",
                column: "ToStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Invoices_InvoiceId",
                table: "Cars",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Stations_FromStationId",
                table: "Cars",
                column: "FromStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Stations_LastStationId",
                table: "Cars",
                column: "LastStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Stations_ToStationId",
                table: "Cars",
                column: "ToStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Invoices_InvoiceId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Stations_FromStationId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Stations_LastStationId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Stations_ToStationId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Cars_FromStationId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_InvoiceId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_LastStationId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_ToStationId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "FromStationId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "LastOperationName",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "LastStationId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ToStationId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "WhenLastOperation",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "FromStationId",
                table: "Trains",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastOperationName",
                table: "Trains",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ToStationId",
                table: "Trains",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrainId",
                table: "Invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trains_FromStationId",
                table: "Trains",
                column: "FromStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_ToStationId",
                table: "Trains",
                column: "ToStationId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_FromStationId",
                table: "Trains",
                column: "FromStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_ToStationId",
                table: "Trains",
                column: "ToStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
