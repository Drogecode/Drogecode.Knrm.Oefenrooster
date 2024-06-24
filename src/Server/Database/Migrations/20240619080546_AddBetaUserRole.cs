using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBetaUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Accesses", "CustomerId", "Name" },
                values: new object[] { new Guid("d526e5ed-e838-499d-a96c-62180db28bed"), "full_dashboard_statistics1", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "Beta user" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d526e5ed-e838-499d-a96c-62180db28bed"));
        }
    }
}
