using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                column: "Order",
                value: 80);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                column: "Order",
                value: 60);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"),
                column: "Order",
                value: 170);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                column: "Order",
                value: 20);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                column: "Order",
                value: 100);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                column: "Order",
                value: 180);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                column: "Order",
                value: 30);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                column: "Order",
                value: 70);

            migrationBuilder.InsertData(
                table: "UserFunctions",
                columns: new[] { "Id", "CustomerId", "Default", "Name", "Order", "TrainingOnly" },
                values: new object[] { new Guid("d23de705-d950-4833-8b94-aa531022d450"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, "Kompas leider", 10, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("d23de705-d950-4833-8b94-aa531022d450"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                column: "Order",
                value: 50);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                column: "Order",
                value: 30);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"),
                column: "Order",
                value: 70);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                column: "Order",
                value: 10);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                column: "Order",
                value: 60);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                column: "Order",
                value: 80);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                column: "Order",
                value: 20);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                column: "Order",
                value: 40);
        }
    }
}
