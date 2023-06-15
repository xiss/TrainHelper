using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainHelper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshTokenIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "UserSessions");

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenId",
                table: "UserSessions",
                type: "uuid",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "UserSessions");

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenId",
                table: "UserSessions",
                type: "integer",
                nullable: false);
        }
    }
}
