using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemUser",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedOn", "CustomerId", "DeletedBy", "DeletedOn", "Email", "ExternalId", "IsSystemUser", "LastLogin", "Name", "Nr", "RoleFromSharePoint", "SharePointID", "SyncedFromSharePoint", "UserFunctionId" },
                values: new object[] { new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e"), new DateTime(2024, 10, 11, 19, 46, 12, 0, DateTimeKind.Utc), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), null, null, "system@drogecode.nl", null, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, false, null, false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e"));

            migrationBuilder.DropColumn(
                name: "IsSystemUser",
                table: "Users");
        }
    }
}
