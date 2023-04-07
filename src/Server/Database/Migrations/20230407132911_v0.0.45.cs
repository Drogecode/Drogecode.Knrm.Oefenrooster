using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0045 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SetBy",
                table: "RoosterAvailable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserDefaultAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterDefaultId = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true),
                    From = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Till = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefaultAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_RoosterDefault_RoosterDefaultId",
                        column: x => x.RoosterDefaultId,
                        principalTable: "RoosterDefault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDefaultAvailable_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHolidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true),
                    From = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Till = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHolidays_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHolidays_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                column: "Name",
                value: "HRB oefening");

            migrationBuilder.InsertData(
                table: "RoosterTrainingType",
                columns: new[] { "Id", "ColorDark", "ColorLight", "CountToTrainingTarget", "CustomerId", "IsDefault", "Name", "Order" },
                values: new object[,]
                {
                    { new Guid("6153a297-9486-43de-91e8-22d107da2b21"), "#3b4d42", "#63806f", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Evenement", 60 },
                    { new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"), "#5f6138", "#919454", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Techniek", 70 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_CustomerId",
                table: "UserDefaultAvailable",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_RoosterDefaultId",
                table: "UserDefaultAvailable",
                column: "RoosterDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_UserId",
                table: "UserDefaultAvailable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHolidays_CustomerId",
                table: "UserHolidays",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHolidays_UserId",
                table: "UserHolidays",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDefaultAvailable");

            migrationBuilder.DropTable(
                name: "UserHolidays");

            migrationBuilder.DeleteData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("6153a297-9486-43de-91e8-22d107da2b21"));

            migrationBuilder.DeleteData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"));

            migrationBuilder.DropColumn(
                name: "SetBy",
                table: "RoosterAvailable");

            migrationBuilder.UpdateData(
                table: "RoosterTrainingType",
                keyColumn: "Id",
                keyValue: new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                column: "Name",
                value: "HRB");
        }
    }
}
