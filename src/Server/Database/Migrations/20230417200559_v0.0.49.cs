using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class v0049 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoosterItemDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFullDay = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterItemDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterItemDay_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoosterItemMonth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoosterItemMonth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoosterItemMonth_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RoosterItemMonth",
                columns: new[] { "Id", "CustomerId", "Month", "Order", "Severity", "Text", "Type", "Year" },
                values: new object[,]
                {
                    { new Guid("01a17983-9bbe-4bfc-b152-f73c1869393d"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)4, 0, 0, "KNRM Kompas onderwerp; Veiligheid", 1, null },
                    { new Guid("36e33dc3-8bb8-4096-a127-c3ee04a0e694"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)3, 0, 0, "KNRM Kompas onderwerp; SAR & Hulpverlening", 1, null },
                    { new Guid("4a009dd3-db02-4668-bbb0-9a9298c23d58"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)6, 0, 0, "KNRM Kompas onderwerp; EHBO & Procedures", 1, null },
                    { new Guid("5208deef-4529-4a30-a00e-22737cf52183"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)2, 0, 0, "KNRM Kompas onderwerp;  Algemene kennis & Communicatie", 1, null },
                    { new Guid("7696579c-403c-4a98-b30e-b19f1e90ffd0"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)7, 1, 1, "Geen ingeroosterde oefeningen in verband met het hoogseizoen", 1, null },
                    { new Guid("857e2ee9-8f2f-407e-9ec8-a0eaa853b957"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)1, 0, 0, "KNRM Kompas onderwerp; GEEN", 1, null },
                    { new Guid("d7d80ee0-0e73-426f-84b2-2040057c2f7a"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)1, 1, 1, "Geen ingeroosterde oefeningen in verband met het winterseizoen", 1, null },
                    { new Guid("e4c00e6b-14d5-4609-bff3-6a6533557a0b"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)7, 0, 0, "KNRM Kompas onderwerp; Techiek & Varen", 1, null },
                    { new Guid("f9d140f0-58fa-4c9a-a845-0eb5bad2814f"), new Guid("d9754755-b054-4a9c-a77f-da42a4009365"), (short)5, 0, 0, "KNRM Kompas onderwerp; Navigatie", 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoosterItemDay_CustomerId",
                table: "RoosterItemDay",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoosterItemMonth_CustomerId",
                table: "RoosterItemMonth",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoosterItemDay");

            migrationBuilder.DropTable(
                name: "RoosterItemMonth");
        }
    }
}
