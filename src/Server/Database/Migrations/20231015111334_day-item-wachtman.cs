using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class dayitemwachtman : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "RoosterItemDay",
                type: "uuid",
                nullable: true);

            /* Does not work if database does not exists. (users not synced with azure)
             * 
             * migrationBuilder.InsertData(
                table: "RoosterItemDay",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "DateEnd", "DateStart", "IsFullDay", "Text", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("51da3135-ba68-43c0-bc66-72eba93ccf3d"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2023, 10, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 10, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, "Wachtman", 1, new Guid("8b4d5cfa-3770-4d5d-84dc-1d594dbdcbbf") },
                    { new Guid("bf0b7712-d4d4-423c-b56f-f698a643b580"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Utc), true, "Wachtman", 1, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0") }
                });*/

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterItemDay_UserId",
                table: "RoosterItemDay",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterItemDay_Users_UserId",
                table: "RoosterItemDay",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterItemDay_Users_UserId",
                table: "RoosterItemDay");

            migrationBuilder.DropIndex(
                name: "IX_RoosterItemDay_UserId",
                table: "RoosterItemDay");

            /*migrationBuilder.DeleteData(
                table: "RoosterItemDay",
                keyColumn: "Id",
                keyValue: new Guid("51da3135-ba68-43c0-bc66-72eba93ccf3d"));

            migrationBuilder.DeleteData(
                table: "RoosterItemDay",
                keyColumn: "Id",
                keyValue: new Guid("bf0b7712-d4d4-423c-b56f-f698a643b580"));*/

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RoosterItemDay");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler");
        }
    }
}
