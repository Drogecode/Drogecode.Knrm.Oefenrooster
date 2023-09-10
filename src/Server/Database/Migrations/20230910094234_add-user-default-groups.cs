using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class adduserdefaultgroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultGroupId",
                table: "UserDefaultAvailable",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientIdLogin",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientIdServer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecretLogin",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecretServer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instance",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserDefaultGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefaultGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDefaultGroup_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDefaultGroup_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("d9754755-b054-4a9c-a77f-da42a4009365"),
                columns: new[] { "ClientIdLogin", "ClientIdServer", "ClientSecretLogin", "ClientSecretServer", "Domain", "Instance", "TenantId" },
                values: new object[] { "a9c68159-901c-449a-83e0-85243364e3cc", "220e1008-1131-4e82-a388-611cd773ddf8", "", "", "hui.nu", "https://login.microsoftonline.com/", "d9754755-b054-4a9c-a77f-da42a4009365" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultAvailable_DefaultGroupId",
                table: "UserDefaultAvailable",
                column: "DefaultGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultGroup_CustomerId",
                table: "UserDefaultGroup",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDefaultGroup_UserId",
                table: "UserDefaultGroup",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDefaultAvailable_UserDefaultGroup_DefaultGroupId",
                table: "UserDefaultAvailable",
                column: "DefaultGroupId",
                principalTable: "UserDefaultGroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDefaultAvailable_UserDefaultGroup_DefaultGroupId",
                table: "UserDefaultAvailable");

            migrationBuilder.DropTable(
                name: "UserDefaultGroup");

            migrationBuilder.DropIndex(
                name: "IX_UserDefaultAvailable_DefaultGroupId",
                table: "UserDefaultAvailable");

            migrationBuilder.DropColumn(
                name: "DefaultGroupId",
                table: "UserDefaultAvailable");

            migrationBuilder.DropColumn(
                name: "ClientIdLogin",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClientIdServer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClientSecretLogin",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClientSecretServer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Instance",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Customers");
        }
    }
}
