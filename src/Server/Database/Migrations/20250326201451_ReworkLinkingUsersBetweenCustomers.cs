using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReworkLinkingUsersBetweenCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserCustomer_Users_LinkUserId",
                table: "LinkUserCustomer");

            migrationBuilder.RenameColumn(
                name: "LinkUserId",
                table: "LinkUserCustomer",
                newName: "GlobalUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUserCustomer_LinkUserId",
                table: "LinkUserCustomer",
                newName: "IX_LinkUserCustomer_GlobalUserId");

            migrationBuilder.CreateTable(
                name: "UsersGlobal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersGlobal", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UsersGlobal",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Name" },
                values: new object[] { new Guid("588df154-9ef2-4014-a700-02dd69011a4d"), new Guid("b4bcc37b-321a-4027-b02b-30630ad8f75e"), new DateTime(1992, 9, 4, 1, 4, 8, 0, DateTimeKind.Utc), null, null, "Taco Droogers" });

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserCustomer_UsersGlobal_GlobalUserId",
                table: "LinkUserCustomer",
                column: "GlobalUserId",
                principalTable: "UsersGlobal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserCustomer_UsersGlobal_GlobalUserId",
                table: "LinkUserCustomer");

            migrationBuilder.DropTable(
                name: "UsersGlobal");

            migrationBuilder.RenameColumn(
                name: "GlobalUserId",
                table: "LinkUserCustomer",
                newName: "LinkUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUserCustomer_GlobalUserId",
                table: "LinkUserCustomer",
                newName: "IX_LinkUserCustomer_LinkUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserCustomer_Users_LinkUserId",
                table: "LinkUserCustomer",
                column: "LinkUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
