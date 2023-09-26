using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class renamenotimetoshowtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoTime",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "NoTime",
                table: "RoosterDefault");

            migrationBuilder.AddColumn<bool>(
                name: "ShowTime",
                table: "RoosterTraining",
                type: "boolean",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTime",
                table: "RoosterDefault",
                type: "boolean",
                nullable: true,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("860ec129-6b99-4286-b90a-a2d536377f7c"),
                column: "ShowTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "ShowTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "ShowTime",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowTime",
                table: "RoosterTraining");

            migrationBuilder.DropColumn(
                name: "ShowTime",
                table: "RoosterDefault");

            migrationBuilder.AddColumn<bool>(
                name: "NoTime",
                table: "RoosterTraining",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoTime",
                table: "RoosterDefault",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("860ec129-6b99-4286-b90a-a2d536377f7c"),
                column: "NoTime",
                value: true);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                column: "NoTime",
                value: false);

            migrationBuilder.UpdateData(
                table: "RoosterDefault",
                keyColumn: "Id",
                keyValue: new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                column: "NoTime",
                value: false);
        }
    }
}
