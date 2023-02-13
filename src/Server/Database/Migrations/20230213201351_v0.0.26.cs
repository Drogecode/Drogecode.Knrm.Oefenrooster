using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "RoosterDefault");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "RoosterTraining",
                newName: "DateStart");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "RoosterDefault",
                newName: "TimeStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "RoosterTraining",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeEnd",
                table: "RoosterDefault",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "TimeEnd",
                value: new TimeOnly(13, 0, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "TimeEnd",
                value: new TimeOnly(13, 0, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "TimeEnd",
                value: new TimeOnly(16, 0, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "TimeEnd",
                value: new TimeOnly(21, 30, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "TimeEnd",
                value: new TimeOnly(21, 30, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "TimeEnd",
                value: new TimeOnly(16, 0, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "TimeEnd",
                value: new TimeOnly(21, 30, 0));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "TimeEnd",
                value: new TimeOnly(21, 30, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "TimeEnd",
                table: "RoosterDefault");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "RoosterTraining",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "TimeStart",
                table: "RoosterDefault",
                newName: "StartTime");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "RoosterTraining",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "RoosterDefault",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "Duration",
                value: 120);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "Duration",
                value: 120);
        }
    }
}
