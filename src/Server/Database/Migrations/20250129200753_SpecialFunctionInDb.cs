using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class SpecialFunctionInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Accesses",
                table: "UserRoles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                table: "UserFunctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("a23f1f39-275e-4e21-901f-4878b73d3ede"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                column: "IsSpecial",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("d23de705-d950-4833-8b94-aa531022d450"),
                column: "IsSpecial",
                value: true);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                column: "IsSpecial",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecial",
                table: "UserFunctions");

            migrationBuilder.AlterColumn<string>(
                name: "Accesses",
                table: "UserRoles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
