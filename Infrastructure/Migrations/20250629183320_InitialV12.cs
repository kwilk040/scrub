using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialV12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_LookupKey",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "HashedKey",
                table: "ApiKeys");

            migrationBuilder.RenameColumn(
                name: "LookupKey",
                table: "ApiKeys",
                newName: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "ApiKeys",
                newName: "LookupKey");

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "ApiKeys",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashedKey",
                table: "ApiKeys",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_LookupKey",
                table: "ApiKeys",
                column: "LookupKey");
        }
    }
}
