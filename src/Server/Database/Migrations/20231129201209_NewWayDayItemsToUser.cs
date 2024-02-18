using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewWayDayItemsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterItemDay_Users_UserId",
                table: "RoosterItemDay");

            migrationBuilder.DropIndex(
                name: "IX_RoosterItemDay_UserId",
                table: "RoosterItemDay");

            /* Does not work if database does not exists. (users not synced with azure)
             * 
             * migrationBuilder.DeleteData(
                table: "RoosterItemDay",
                keyColumn: "Id",
                keyValue: new Guid("51da3135-ba68-43c0-bc66-72eba93ccf3d"));

            migrationBuilder.DeleteData(
                table: "RoosterItemDay",
                keyColumn: "Id",
                keyValue: new Guid("bf0b7712-d4d4-423c-b56f-f698a643b580"));*/

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RoosterItemDay",
                newName: "DeletedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "RoosterItemDay",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LinkUserDayItems",
                columns: table => new
                {
                    UserForeignKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DayItemForeignKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkUserDayItems", x => new { x.DayItemForeignKey, x.UserForeignKey });
                    table.ForeignKey(
                        name: "FK_LinkUserDayItems_RoosterItemDay_DayItemForeignKey",
                        column: x => x.DayItemForeignKey,
                        principalTable: "RoosterItemDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkUserDayItems_Users_UserForeignKey",
                        column: x => x.UserForeignKey,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkUserDayItems_UserForeignKey",
                table: "LinkUserDayItems",
                column: "UserForeignKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkUserDayItems");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "RoosterItemDay");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "RoosterItemDay",
                newName: "UserId");

            /*migrationBuilder.InsertData(
                table: "RoosterItemDay",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "CustomerId", "DateEnd", "DateStart", "IsFullDay", "Text", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("51da3135-ba68-43c0-bc66-72eba93ccf3d"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2023, 10, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 10, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, "Wachtman", 1, new Guid("8b4d5cfa-3770-4d5d-84dc-1d594dbdcbbf") },
                    { new Guid("bf0b7712-d4d4-423c-b56f-f698a643b580"), new Guid("04093c7a-11e5-4887-af51-319ecc59efe0"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2023, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 10, 9, 0, 0, 0, 0, DateTimeKind.Utc), true, "Wachtman", 1, new Guid("04093c7a-11e5-4887-af51-319ecc59efe0") }
                });*/

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
    }
}
