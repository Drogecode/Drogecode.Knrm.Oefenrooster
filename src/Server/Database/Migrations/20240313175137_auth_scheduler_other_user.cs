using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class auth_scheduler_other_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                column: "Accesses",
                value: "full_training_history,full_action_history,scheduler_other");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                column: "Accesses",
                value: "full_training_history,full_action_history,scheduler_other");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                column: "Accesses",
                value: "full_training_history,full_action_history,scheduler_other");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table,scheduler_past,scheduler_dayitem,scheduler_other");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Accesses", "CustomerId", "Name" },
                values: new object[] { new Guid("90a40128-183f-408b-aa64-eb3b279a7042"), "scheduler_other", new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), "Basic scheduler" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("90a40128-183f-408b-aa64-eb3b279a7042"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                column: "Accesses",
                value: "full_training_history,full_action_history");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                column: "Accesses",
                value: "full_training_history,full_action_history");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                column: "Accesses",
                value: "full_training_history,full_action_history");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                column: "Accesses",
                value: "scheduler,scheduler_table,scheduler_past,scheduler_dayitem");
        }
    }
}
