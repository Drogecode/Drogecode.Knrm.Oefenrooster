using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0036 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingType",
                table: "RoosterTraining");

            migrationBuilder.RenameColumn(
                name: "Default",
                table: "Vehicle",
                newName: "IsDefault");

            migrationBuilder.AddColumn<Guid>(
                name: "RoosterTrainingTypeId",
                table: "RoosterTraining",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoosterTrainingTypeId",
                table: "RoosterDefault",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoosterTrainingType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ColorLight = table.Column<string>(type: "text", nullable: true),
                    ColorDark = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CountToTrainingTarget = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterTrainingType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterTrainingType_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "RoosterTrainingTypeId",
                value: new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"));

            migrationBuilder.InsertData(
                table: "RoosterTrainingType",
                columns: new[] { "Id", "ColorDark", "ColorLight", "CountToTrainingTarget", "CustomerId", "IsDefault", "Name", "Order" },
                values: new object[,]
                {
                    { new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"), "rgb(244,47,70)", "rgb(242,28,13)", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Brandweer", 40 },
                    { new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"), "", "rgb(25,169,140)", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "een op een", 30 },
                    { new Guid("7dd5bf75-aef4-4cdd-9515-112e9b51f2f0"), "#ffffff4c", "#bdbdbdff", true, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, "Kompas oefening", 10 },
                    { new Guid("80108015-87a7-4453-a1af-d81d15fe3582"), "rgb(214,143,0)", "rgb(214,129,0)", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "EHBO", 20 },
                    { new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"), "rgb(13,222,156)", "rgb(0,235,98)", false, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "HRB", 50 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTraining_RoosterTrainingTypeId",
                table: "RoosterTraining",
                column: "RoosterTrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterDefault_RoosterTrainingTypeId",
                table: "RoosterDefault",
                column: "RoosterTrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterTrainingType_CustomerId",
                table: "RoosterTrainingType",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterDefault_RoosterTrainingType_RoosterTrainingTypeId",
                table: "RoosterDefault",
                column: "RoosterTrainingTypeId",
                principalTable: "RoosterTrainingType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterTraining_RoosterTrainingType_RoosterTrainingTypeId",
                table: "RoosterTraining",
                column: "RoosterTrainingTypeId",
                principalTable: "RoosterTrainingType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoosterDefault_RoosterTrainingType_RoosterTrainingTypeId",
                table: "RoosterDefault");

            migrationBuilder.DropForeignKey(
                name: "FK_RoosterTraining_RoosterTrainingType_RoosterTrainingTypeId",
                table: "RoosterTraining");

            migrationBuilder.DropTable(
                name: "RoosterTrainingType");

            migrationBuilder.DropIndex(
                name: "IX_RoosterTraining_RoosterTrainingTypeId",
                table: "RoosterTraining");

            migrationBuilder.DropIndex(
                name: "IX_RoosterDefault_RoosterTrainingTypeId",
                table: "RoosterDefault");

            migrationBuilder.DropColumn(
                name: "RoosterTrainingTypeId",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "RoosterTrainingTypeId",
                table: "RoosterDefault");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "Vehicle",
                newName: "Default");

            migrationBuilder.AddColumn<int>(
                name: "TrainingType",
                table: "RoosterTraining",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
