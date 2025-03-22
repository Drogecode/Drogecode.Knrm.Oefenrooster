using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class bugUserLinkedWrongToCustomergetAllResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserCustomer_Users_UserId1",
                table: "LinkUserCustomer");

            migrationBuilder.DropIndex(
                name: "IX_LinkUserCustomer_UserId1",
                table: "LinkUserCustomer");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "LinkUserCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_LinkUserCustomer_LinkUserId",
                table: "LinkUserCustomer",
                column: "LinkUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserCustomer_Users_LinkUserId",
                table: "LinkUserCustomer",
                column: "LinkUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUserCustomer_Users_LinkUserId",
                table: "LinkUserCustomer");

            migrationBuilder.DropIndex(
                name: "IX_LinkUserCustomer_LinkUserId",
                table: "LinkUserCustomer");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "LinkUserCustomer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LinkUserCustomer_UserId1",
                table: "LinkUserCustomer",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUserCustomer_Users_UserId1",
                table: "LinkUserCustomer",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
