using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    AddLoginHint = table.Column<char>(type: "character(1)", nullable: true),
                    IsGroup = table.Column<bool>(type: "boolean", nullable: false),
                    TargetBlank = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menu_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Menu_Menu_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharedActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    LoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_ReportActionShared_SharedActionId",
                        column: x => x.SharedActionId,
                        principalTable: "ReportActionShared",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "AddLoginHint", "CustomerId", "IsGroup", "Order", "ParentId", "TargetBlank", "Text", "Url" },
                values: new object[,]
                {
                    { new Guid("2bf106c9-eae7-4a0d-978d-54af6c4e96a1"), null, new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), true, 10, null, false, "Handige links", "" },
                    { new Guid("953de109-5526-433b-8dc8-61b10fa8fd20"), '&', new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, 30, new Guid("2bf106c9-eae7-4a0d-978d-54af6c4e96a1"), true, "LPLH", "https://dorus1824.sharepoint.com/:b:/r/sites/KNRM/Documenten/EHBO/LPLH/20181115%20LPLH_KNRM_1_1.pdf?csf=1&web=1&e=4L3VPo" },
                    { new Guid("af84e214-7def-45ac-95c9-c8a66d1573a2"), '?', new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), false, 20, new Guid("2bf106c9-eae7-4a0d-978d-54af6c4e96a1"), true, "Sharepoint", "https://dorus1824.sharepoint.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menu_CustomerId",
                table: "Menu",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_ParentId",
                table: "Menu",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_SharedActionId",
                table: "UserLogins",
                column: "SharedActionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "UserLogins");
        }
    }
}
