using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class DirectLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DirectLogin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DirectLogin",
                table: "UserLogins",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "UserLogins",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e"),
                columns: new[] { "DirectLogin", "HashedPassword" },
                values: new object[] { false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DirectLogin",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "UserLogins");
        }
    }
}
