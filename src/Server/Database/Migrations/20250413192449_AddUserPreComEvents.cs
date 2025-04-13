using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreComEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreComId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserPreComEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalendarEventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreComEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreComEvent_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreComEvent_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e"),
                column: "PreComId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreComEvent_CustomerId",
                table: "UserPreComEvent",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreComEvent_UserId",
                table: "UserPreComEvent",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreComEvent");

            migrationBuilder.DropColumn(
                name: "PreComId",
                table: "Users");
        }
    }
}
