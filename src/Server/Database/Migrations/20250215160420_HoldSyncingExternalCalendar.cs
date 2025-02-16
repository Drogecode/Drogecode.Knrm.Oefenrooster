using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class HoldSyncingExternalCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncOn",
                table: "RoosterAvailable",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdateBy",
                table: "RoosterAvailable",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateOn",
                table: "RoosterAvailable",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLastCalendarUpdate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLastCalendarUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLastCalendarUpdate_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLastCalendarUpdate_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_DeletedBy",
                table: "RoosterTraining",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_LastUpdateBy",
                table: "RoosterAvailable",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastCalendarUpdate_CustomerId",
                table: "UserLastCalendarUpdate",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastCalendarUpdate_UserId",
                table: "UserLastCalendarUpdate",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterAvailable_Users_LastUpdateBy",
                table: "RoosterAvailable",
                column: "LastUpdateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterTraining_Users_DeletedBy",
                table: "RoosterTraining",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterAvailable_Users_LastUpdateBy",
                table: "RoosterAvailable");

            migrationBuilder.DropForeignKey(
                name: "FK_RoosterTraining_Users_DeletedBy",
                table: "RoosterTraining");

            migrationBuilder.DropTable(
                name: "UserLastCalendarUpdate");

            migrationBuilder.DropIndex(
                name: "IX_RoosterTraining_DeletedBy",
                table: "RoosterTraining");

            migrationBuilder.DropIndex(
                name: "IX_RoosterAvailable_LastUpdateBy",
                table: "RoosterAvailable");

            migrationBuilder.DropColumn(
                name: "LastSyncOn",
                table: "RoosterAvailable");

            migrationBuilder.DropColumn(
                name: "LastUpdateBy",
                table: "RoosterAvailable");

            migrationBuilder.DropColumn(
                name: "LastUpdateOn",
                table: "RoosterAvailable");
        }
    }
}
