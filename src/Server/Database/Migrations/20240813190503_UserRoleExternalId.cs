using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalId",
                table: "UserRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                column: "ExternalId",
                value: new Guid("2197a054-e81f-4720-9f08-321377398cb6"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                column: "ExternalId",
                value: new Guid("287359b1-2035-435b-97b0-eb260dc497d6"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"),
                column: "ExternalId",
                value: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                column: "ExternalId",
                value: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"),
                column: "ExternalId",
                value: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                column: "ExternalId",
                value: new Guid("afb45395-89ee-413d-9385-21962772dbda"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"),
                column: "ExternalId",
                value: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "ExternalId",
                value: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"),
                column: "ExternalId",
                value: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                column: "ExternalId",
                value: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "ExternalId",
                value: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UserRoles");
        }
    }
}
