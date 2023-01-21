using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
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
                    WeekDay = table.Column<short>(type: "smallint", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterDefault", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoosterTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterTraining", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoosterAvailable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdCustomer = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<short>(type: "smallint", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterAvailable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterAvailable_RoosterTraining_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "RoosterTraining",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "CustomerId", "Email", "Name" },
                values: new object[] { new Guid("46a4ddb6-412b-4329-b48f-ed681c96bc26"), new DateTime(1992, 9, 4, 6, 30, 42, 0, DateTimeKind.Utc), new Guid("c0f17c92-57e5-4ec2-83c4-904160078c38"), "test@drogecode.nl", "from model creating" });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_TrainingId",
                table: "RoosterAvailable",
                column: "TrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "RoosterAvailable");

            migrationBuilder.DropTable(
                name: "RoosterDefault");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RoosterTraining");
        }
    }
}
