using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0028 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Customers_CustomerId",
                table: "Audits");

            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Users_UserId",
                table: "Audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audits",
                table: "Audits");

            migrationBuilder.DeleteData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"));

            migrationBuilder.RenameTable(
                name: "Audits",
                newName: "Audit");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_UserId",
                table: "Audit",
                newName: "IX_Audit_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_CustomerId",
                table: "Audit",
                newName: "IX_Audit_CustomerId");

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "RoosterAvailable",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit",
                table: "Audit",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LinkVehicleTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoosterTrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vehicle = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkVehicleTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkVehicleTraining_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "Id", "Active", "Code", "CustomerId", "Default", "DeletedOn", "Name", "Order" },
                values: new object[,]
                {
                    { new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"), true, "NWI", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, null, "Nikolaas Wijsenbeek", 10 },
                    { new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"), true, "HZN018", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, null, "Vlet", 30 },
                    { new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"), true, "HZR", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, null, "De Huizer", 20 },
                    { new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"), true, "Wal", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, null, "Wal", 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterAvailable_VehicleId",
                table: "RoosterAvailable",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkVehicleTraining_CustomerId",
                table: "LinkVehicleTraining",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CustomerId",
                table: "Vehicle",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Customers_CustomerId",
                table: "Audit",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Users_UserId",
                table: "Audit",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoosterAvailable_Vehicle_VehicleId",
                table: "RoosterAvailable",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Customers_CustomerId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Users_UserId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_RoosterAvailable_Vehicle_VehicleId",
                table: "RoosterAvailable");

            migrationBuilder.DropTable(
                name: "LinkVehicleTraining");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_RoosterAvailable_VehicleId",
                table: "RoosterAvailable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "RoosterAvailable");

            migrationBuilder.RenameTable(
                name: "Audit",
                newName: "Audits");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_UserId",
                table: "Audits",
                newName: "IX_Audits_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_CustomerId",
                table: "Audits",
                newName: "IX_Audits_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audits",
                table: "Audits",
                column: "Id");

            migrationBuilder.InsertData(
                table: "UserFunctions",
                columns: new[] { "Id", "CustomerId", "Default", "Name", "Order", "TrainingOnly" },
                values: new object[] { new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Wal", 170, true });

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Customers_CustomerId",
                table: "Audits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Users_UserId",
                table: "Audits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
