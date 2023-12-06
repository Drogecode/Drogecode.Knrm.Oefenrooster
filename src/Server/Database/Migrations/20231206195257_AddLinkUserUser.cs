using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkUserUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserDayItems_RoosterItemDay_DayItemForeignKey",
                table: "LinkUserDayItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserDayItems_Users_UserForeignKey",
                table: "LinkUserDayItems");

            migrationBuilder.RenameColumn(
                name: "UserForeignKey",
                table: "LinkUserDayItems",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "DayItemForeignKey",
                table: "LinkUserDayItems",
                newName: "DayItemId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUserDayItems_UserForeignKey",
                table: "LinkUserDayItems",
                newName: "IX_LinkUserDayItems_UserId");

            migrationBuilder.CreateTable(
                name: "LinkUserUsers",
                columns: table => new
                {
                    UserAId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserBId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkType = table.Column<int>(type: "integer", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkUserUsers", x => new { x.UserAId, x.UserBId });
                    table.ForeignKey(
                        name: "FK_LinkUserUsers_Users_UserAId",
                        column: x => x.UserAId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkUserUsers_Users_UserBId",
                        column: x => x.UserBId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "Accesses",
                value: "configure_training-types,users_settings");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table,scheduler_past,scheduler_dayitem");

            migrationBuilder.CreateIndex(
                name: "IX_LinkUserUsers_UserBId",
                table: "LinkUserUsers",
                column: "UserBId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserDayItems_RoosterItemDay_DayItemId",
                table: "LinkUserDayItems",
                column: "DayItemId",
                principalTable: "RoosterItemDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserDayItems_Users_UserId",
                table: "LinkUserDayItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserDayItems_RoosterItemDay_DayItemId",
                table: "LinkUserDayItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserDayItems_Users_UserId",
                table: "LinkUserDayItems");

            migrationBuilder.DropTable(
                name: "LinkUserUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "LinkUserDayItems",
                newName: "UserForeignKey");

            migrationBuilder.RenameColumn(
                name: "DayItemId",
                table: "LinkUserDayItems",
                newName: "DayItemForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUserDayItems_UserId",
                table: "LinkUserDayItems",
                newName: "IX_LinkUserDayItems_UserForeignKey");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "Accesses",
                value: "configure_training-types");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserDayItems_RoosterItemDay_DayItemForeignKey",
                table: "LinkUserDayItems",
                column: "DayItemForeignKey",
                principalTable: "RoosterItemDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserDayItems_Users_UserForeignKey",
                table: "LinkUserDayItems",
                column: "UserForeignKey",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
