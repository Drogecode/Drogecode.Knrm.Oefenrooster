using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoosterDefault",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekDay = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterDefault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterDefault_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterDefaultId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterTraining_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterTraining_RoosterDefault_RoosterDefaultId",
                        column: x => x.RoosterDefaultId,
                        principalTable: "RoosterDefault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditType = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ObjectKey = table.Column<Guid>(type: "uuid", nullable: true),
                    ObjectName = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Audits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_RoosterTraining_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "RoosterTraining",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Created", "Name" },
                values: new object[] { new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new DateTime(2022, 10, 12, 18, 12, 5, 0, DateTimeKind.Utc), "KNRM Huizen" });

            migrationBuilder.InsertData(
                table: "RoosterDefault",
                columns: new[] { "Id", "CustomerId", "EndTime", "StartTime", "WeekDay" },
                values: new object[,]
                {
                    { new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(13, 0, 0), new TimeOnly(10, 0, 0), 0 },
                    { new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(13, 0, 0), new TimeOnly(10, 0, 0), 6 },
                    { new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(16, 0, 0), new TimeOnly(13, 0, 0), 6 },
                    { new Guid("4142048e-82dc-4015-aab7-1b519da01238"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), 1 },
                    { new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), 2 },
                    { new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(16, 0, 0), new TimeOnly(13, 0, 0), 0 },
                    { new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), 4 },
                    { new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), new TimeOnly(21, 30, 0), new TimeOnly(19, 30, 0), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CustomerId",
                table: "Audits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_UserId",
                table: "Audits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_CustomerId",
                table: "RoosterAvailable",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_TrainingId",
                table: "RoosterAvailable",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_UserId",
                table: "RoosterAvailable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterDefault_CustomerId",
                table: "RoosterDefault",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_CustomerId",
                table: "RoosterTraining",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_RoosterDefaultId",
                table: "RoosterTraining",
                column: "RoosterDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerId",
                table: "Users",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "RoosterAvailable");

            migrationBuilder.DropTable(
                name: "RoosterTraining");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RoosterDefault");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
