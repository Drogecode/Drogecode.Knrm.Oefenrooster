using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class addroosternotime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NoTime",
                table: "RoosterTraining",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RoosterDefault",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoTime",
                table: "RoosterDefault",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                columns: new[] { "Name", "NoTime", "Order", "ValidUntil" },
                values: new object[] { null, false, 80, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                columns: new[] { "Name", "NoTime", "Order", "ValidUntil" },
                values: new object[] { null, false, 60, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                columns: new[] { "Name", "NoTime", "Order", "ValidUntil" },
                values: new object[] { null, false, 70, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                columns: new[] { "Name", "NoTime", "ValidUntil" },
                values: new object[] { null, false, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                columns: new[] { "Name", "NoTime", "ValidUntil" },
                values: new object[] { null, false, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                columns: new[] { "Name", "NoTime", "Order", "ValidUntil" },
                values: new object[] { null, false, 90, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                columns: new[] { "Name", "NoTime", "ValidUntil" },
                values: new object[] { null, false, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                columns: new[] { "Name", "NoTime", "ValidUntil" },
                values: new object[] { null, false, new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RoosterDefault",
                columns: new[] { "Id", "CountToTrainingTarget", "CustomerId", "Name", "NoTime", "Order", "RoosterTrainingTypeId", "TimeEnd", "TimeStart", "TimeZone", "ValidFrom", "ValidUntil", "WeekDay" },
                values: new object[] { new Guid("860ec129-6b99-4286-b90a-a2d536377f7c"), false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "1:1", true, 50, new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"), new TimeOnly(17, 30, 0), new TimeOnly(15, 0, 0), "Europe/Amsterdam", new DateTime(2023, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("860ec129-6b99-4286-b90a-a2d536377f7c"));

            migrationBuilder.DropColumn(
                name: "NoTime",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RoosterDefault");

            migrationBuilder.DropColumn(
                name: "NoTime",
                table: "RoosterDefault");

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                columns: new[] { "Order", "ValidUntil" },
                values: new object[] { 70, new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                columns: new[] { "Order", "ValidUntil" },
                values: new object[] { 50, new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                columns: new[] { "Order", "ValidUntil" },
                values: new object[] { 60, new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "ValidUntil",
                value: new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "ValidUntil",
                value: new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                columns: new[] { "Order", "ValidUntil" },
                values: new object[] { 80, new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "ValidUntil",
                value: new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "ValidUntil",
                value: new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc));
        }
    }
}
