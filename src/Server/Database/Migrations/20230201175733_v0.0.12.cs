using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrainingOnly",
                table: "UserFunctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"),
                columns: new[] { "Order", "TrainingOnly" },
                values: new object[] { 70, true });

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                column: "TrainingOnly",
                value: true);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                column: "TrainingOnly",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                column: "TrainingOnly",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingOnly",
                table: "UserFunctions");

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("44288807-9e8a-48c5-8579-05d9f0f15f34"),
                column: "Order",
                value: 60);
        }
    }
}
