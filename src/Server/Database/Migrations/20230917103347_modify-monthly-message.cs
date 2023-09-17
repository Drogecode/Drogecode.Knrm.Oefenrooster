using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class modifymonthlymessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"),
                column: "Text",
                value: "KNRM Kompas onderwerp; Communicatie");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"),
                column: "Text",
                value: "KNRM Kompas onderwerp; Algemene kennis");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"),
                column: "Text",
                value: "KNRM Kompas onderwerp; Navigatie");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"),
                column: "Text",
                value: "KNRM Kompas onderwerp; SAR en Hulpverlening");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"),
                column: "Text",
                value: "KNRM Kompas onderwerp; ntb");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"),
                column: "Text",
                value: "KNRM Kompas onderwerp; ntb");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"),
                column: "Text",
                value: "KNRM Kompas onderwerp; ntb");

            migrationBuilder.UpdateData(
                table: "RoosterItemMonth",
                keyColumn: "Id",
                keyValue: new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"),
                column: "Text",
                value: "KNRM Kompas onderwerp; ntb");
        }
    }
}
