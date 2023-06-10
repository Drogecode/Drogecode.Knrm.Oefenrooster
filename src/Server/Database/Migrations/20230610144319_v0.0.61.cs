using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0061 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextColorDark",
                table: "RoosterTrainingType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextColorLight",
                table: "RoosterTrainingType",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { "#C0C0C0", "#FFFFFF" });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                columns: new[] { "ColorDark", "ColorLight", "TextColorDark", "TextColorLight" },
                values: new object[] { "#3BB9FF", "#ADD8E6", null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { "#C0C0C0", "#FFFFFF" });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                columns: new[] { "TextColorDark", "TextColorLight" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextColorDark",
                table: "RoosterTrainingType");

            migrationBuilder.DropColumn(
                name: "TextColorLight",
                table: "RoosterTrainingType");

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                columns: new[] { "ColorDark", "ColorLight" },
                values: new object[] { "#3b4d42", "#63806f" });
        }
    }
}
