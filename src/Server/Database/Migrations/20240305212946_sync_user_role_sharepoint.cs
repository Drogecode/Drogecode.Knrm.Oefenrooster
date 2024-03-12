using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class sync_user_role_sharepoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RoleFromSharePoint",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SyncedFromSharePoint",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "UserFunctions",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"),
                column: "RoleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                column: "RoleId",
                value: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                column: "RoleId",
                value: new Guid("afb45395-89ee-413d-9385-21962772dbda"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                column: "RoleId",
                value: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                column: "RoleId",
                value: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                column: "RoleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("a23f1f39-275e-4e21-901f-4878b73d3ede"),
                column: "RoleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                column: "RoleId",
                value: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"));

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("d23de705-d950-4833-8b94-aa531022d450"),
                column: "RoleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "UserFunctions",
                keyColumn: "Id",
                keyValue: new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                column: "RoleId",
                value: new Guid("2197a054-e81f-4720-9f08-321377398cb6"));

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Accesses", "CustomerId", "Name" },
                values: new object[,]
                {
                    { new Guid("2197a054-e81f-4720-9f08-321377398cb6"), "full_training_history,full_action_history", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "aankomend opstapper" },
                    { new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"), "", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "opstapper op proef" },
                    { new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"), "full_training_history,full_action_history", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "schipper io" },
                    { new Guid("afb45395-89ee-413d-9385-21962772dbda"), "full_training_history,full_action_history", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "opstapper" },
                    { new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"), "full_training_history,full_action_history", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "hrb aankomend opstapper" },
                    { new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"), "full_training_history,full_action_history", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "schipper" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2197a054-e81f-4720-9f08-321377398cb6"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"));

            migrationBuilder.DropColumn(
                name: "RoleFromSharePoint",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SyncedFromSharePoint",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserFunctions");
        }
    }
}
