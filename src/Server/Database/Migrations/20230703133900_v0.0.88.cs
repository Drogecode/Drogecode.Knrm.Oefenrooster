using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0088 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RoosterTrainingType",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                column: "IsActive",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RoosterTrainingType");
        }
    }
}
