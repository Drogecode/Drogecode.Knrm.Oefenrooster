using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class defaultroosterorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "RoosterDefault",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "Order",
                value: 70);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "Order",
                value: 50);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "Order",
                value: 60);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "Order",
                value: 10);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "Order",
                value: 20);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "Order",
                value: 80);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "Order",
                value: 40);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "Order",
                value: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "RoosterDefault");
        }
    }
}
