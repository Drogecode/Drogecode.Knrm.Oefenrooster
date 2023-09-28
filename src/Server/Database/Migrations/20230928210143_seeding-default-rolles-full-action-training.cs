using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class seedingdefaultrollesfullactiontraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "Accesses",
                value: "users_counter,users_details,full_training_history,full_action_history");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                column: "Accesses",
                value: "users_counter,users_details");
        }
    }
}
