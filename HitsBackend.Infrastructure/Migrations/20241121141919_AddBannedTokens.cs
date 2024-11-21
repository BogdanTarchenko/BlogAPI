using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBannedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BannedTokens",
                table: "BannedTokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BannedTokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "BannedTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BannedTokens",
                table: "BannedTokens",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BannedTokens",
                table: "BannedTokens");

            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "BannedTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "BannedTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BannedTokens",
                table: "BannedTokens",
                column: "Id");
        }
    }
}
