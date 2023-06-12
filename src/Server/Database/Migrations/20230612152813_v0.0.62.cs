using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0062 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Till",
                table: "UserHolidays",
                newName: "ValidUntil");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "UserHolidays",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "Till",
                table: "UserDefaultAvailable",
                newName: "ValidUntil");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "UserDefaultAvailable",
                newName: "ValidFrom");

            migrationBuilder.AddColumn<bool>(
                name: "Assigned",
                table: "UserDefaultAvailable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "UserDefaultAvailable",
                columns: new[] { "Id", "Assigned", "Available", "CustomerId", "RoosterDefaultId", "UserId", "ValidFrom", "ValidUntil" },
                values: new object[,]
                {
                    { new Guid("edcfed1c-693b-4320-bb2e-6a1b79b6fa57"), false, 1, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2022, 9, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc) },
                    { new Guid("fd320130-5345-4c32-b660-5c0504061345"), true, 3, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 9, 30, 23, 59, 59, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "UserHolidays",
                columns: new[] { "Id", "Available", "CustomerId", "UserId", "ValidFrom", "ValidUntil" },
                values: new object[] { new Guid("20e1f57d-20cb-48d2-8e0c-c4a0f83e4446"), 2, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 10, 17, 23, 59, 59, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserDefaultAvailable",
                keyColumn: "Id",
                keyValue: new Guid("edcfed1c-693b-4320-bb2e-6a1b79b6fa57"));

            migrationBuilder.DeleteData(
                table: "UserDefaultAvailable",
                keyColumn: "Id",
                keyValue: new Guid("fd320130-5345-4c32-b660-5c0504061345"));

            migrationBuilder.DeleteData(
                table: "UserHolidays",
                keyColumn: "Id",
                keyValue: new Guid("20e1f57d-20cb-48d2-8e0c-c4a0f83e4446"));

            migrationBuilder.DropColumn(
                name: "Assigned",
                table: "UserDefaultAvailable");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "UserHolidays",
                newName: "Till");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                table: "UserHolidays",
                newName: "From");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "UserDefaultAvailable",
                newName: "Till");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                table: "UserDefaultAvailable",
                newName: "From");
        }
    }
}
