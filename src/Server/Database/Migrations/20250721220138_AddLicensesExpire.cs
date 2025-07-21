using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLicensesExpire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Licenses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntil",
                table: "Licenses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Licenses",
                keyColumn: "Id",
                keyValue: new Guid("f9b05a44-e0bb-42a5-9660-085d82337a60"),
                columns: new[] { "ValidFrom", "ValidUntil" },
                values: new object[] { null, null });

            migrationBuilder.InsertData(
                table: "Licenses",
                columns: new[] { "Id", "CustomerId", "License", "ValidFrom", "ValidUntil" },
                values: new object[] { new Guid("863b743c-a634-48b5-b507-f32edd94f2a5"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), 2, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Licenses",
                keyColumn: "Id",
                keyValue: new Guid("863b743c-a634-48b5-b507-f32edd94f2a5"));

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                table: "Licenses");
        }
    }
}
