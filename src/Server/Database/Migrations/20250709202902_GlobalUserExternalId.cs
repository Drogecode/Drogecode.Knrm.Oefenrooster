using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class GlobalUserExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "UsersGlobal",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SetBySync",
                table: "LinkUserCustomer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UsersGlobal",
                keyColumn: "Id",
                keyValue: new Guid("588df154-9ef2-4014-a700-02dd69011a4d"),
                column: "ExternalId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "UX_UsersGlobal_ExternalId",
                table: "UsersGlobal",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_UsersGlobal_ExternalId",
                table: "UsersGlobal");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UsersGlobal");

            migrationBuilder.DropColumn(
                name: "SetBySync",
                table: "LinkUserCustomer");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
