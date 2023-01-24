using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WeekDay",
                table: "RoosterDefault",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "Available",
                table: "RoosterAvailable",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "WeekDay",
                value: 0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "WeekDay",
                value: 6);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "WeekDay",
                value: 6);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("3d73993f-9935-4ebc-b16d-4d444ea8e93a"),
                column: "WeekDay",
                value: 2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "WeekDay",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "WeekDay",
                value: 2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "WeekDay",
                value: 0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "WeekDay",
                value: 2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "WeekDay",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "WeekDay",
                table: "RoosterDefault",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<short>(
                name: "Available",
                table: "RoosterAvailable",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "WeekDay",
                value: (short)0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "WeekDay",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "WeekDay",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("3d73993f-9935-4ebc-b16d-4d444ea8e93a"),
                column: "WeekDay",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "WeekDay",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "WeekDay",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "WeekDay",
                value: (short)0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "WeekDay",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "WeekDay",
                value: (short)3);
        }
    }
}
